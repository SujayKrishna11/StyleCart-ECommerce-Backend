using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using StyleCart.Application.DTOs.Products;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Caching;
using StyleCart.Infrastructure.Data;


namespace StyleCart.Infrastructure.Services;

public class ExcelProductBulkImportService : IProductBulkImportService
{
    private const string ProductsCacheKey = "products:all";

    private readonly ApplicationDbContext _dbContext;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    public ExcelProductBulkImportService(
        ApplicationDbContext dbContext,
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        ICacheService cacheService)
    {
        _dbContext = dbContext;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<BulkProductImportResult> ImportAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        var result = new BulkProductImportResult();

        using var workbook = new XLWorkbook(fileStream);

        var worksheet = workbook.Worksheets
            .FirstOrDefault(sheet => sheet.Name == "Products");

        if (worksheet is null)
        {
            result.Errors.Add("The Excel file must contain a worksheet named 'Products'.");
            return result;
        }

        var expectedHeaders = new[]
        {
            "CategoryName",
            "Name",
            "Description",
            "Brand",
            "BasePrice"
        };

        for (var column = 1; column <= expectedHeaders.Length; column++)
        {
            var header = worksheet.Cell(1, column).GetString().Trim();

            if (!string.Equals(header, expectedHeaders[column - 1],
                    StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add(
                    $"Column {column} must be named '{expectedHeaders[column - 1]}'.");
            }
        }

        if (!result.IsSuccessful)
        {
            return result;
        }

        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        var categoriesByName = categories.ToDictionary(
            category => category.Name,
            category => category,
            StringComparer.OrdinalIgnoreCase);

        var productsToCreate = new List<Product>();
        var namesInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        if (lastRow == 1)
        {
            result.Errors.Add("The Products worksheet has no product rows.");
            return result;
        }

        if (lastRow - 1 > 100)
        {
            result.Errors.Add("A bulk import can contain a maximum of 100 products.");
            return result;
        }

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var categoryName = worksheet.Cell(rowNumber, 1).GetString().Trim();
            var name = worksheet.Cell(rowNumber, 2).GetString().Trim();
            var description = worksheet.Cell(rowNumber, 3).GetString().Trim();
            var brand = worksheet.Cell(rowNumber, 4).GetString().Trim();

            var isEmptyRow = string.IsNullOrWhiteSpace(categoryName)
                && string.IsNullOrWhiteSpace(name)
                && string.IsNullOrWhiteSpace(description)
                && string.IsNullOrWhiteSpace(brand)
                && worksheet.Cell(rowNumber, 5).IsEmpty();

            if (isEmptyRow)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                result.Errors.Add($"Row {rowNumber}: CategoryName is required.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                result.Errors.Add($"Row {rowNumber}: Name is required.");
            }

            if (!worksheet.Cell(rowNumber, 5)
                    .TryGetValue<decimal>(out var basePrice)
                || basePrice <= 0)
            {
                result.Errors.Add(
                    $"Row {rowNumber}: BasePrice must be a number greater than zero.");
            }

            if (!string.IsNullOrWhiteSpace(categoryName)
                && !categoriesByName.ContainsKey(categoryName))
            {
                result.Errors.Add(
                    $"Row {rowNumber}: Category '{categoryName}' does not exist.");
            }

            if (!string.IsNullOrWhiteSpace(name)
                && !namesInFile.Add(name))
            {
                result.Errors.Add(
                    $"Row {rowNumber}: Product name '{name}' is duplicated in this file.");
            }

            if (!string.IsNullOrWhiteSpace(name)
                && await _productRepository.ExistsByNameAsync(
                    name, null, cancellationToken))
            {
                result.Errors.Add(
                    $"Row {rowNumber}: A product named '{name}' already exists.");
            }

            if (result.Errors.Any(error => error.StartsWith($"Row {rowNumber}:")))
            {
                continue;
            }

            productsToCreate.Add(new Product
            {
                CategoryId = categoriesByName[categoryName].Id,
                Name = name,
                Description = string.IsNullOrWhiteSpace(description)
                    ? null
                    : description,
                Brand = string.IsNullOrWhiteSpace(brand)
                    ? null
                    : brand,
                BasePrice = basePrice,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!result.IsSuccessful)
        {
            return result;
        }

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        await _dbContext.Products.AddRangeAsync(productsToCreate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        await _cacheService.RemoveAsync(ProductsCacheKey, cancellationToken);

        result.ImportedCount = productsToCreate.Count;
        result.Products = productsToCreate.Select(product => new ProductResponse
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            BasePrice = product.BasePrice,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        }).ToList();

        return result;
    }
}
