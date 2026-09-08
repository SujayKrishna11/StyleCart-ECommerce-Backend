using StyleCart.Application.DTOs.Products;

namespace StyleCart.Application.Interfaces;

public interface IProductBulkImportService
{
    Task<BulkProductImportResult> ImportAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default);
}