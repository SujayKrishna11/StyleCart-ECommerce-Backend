using StyleCart.Application.DTOs.Inventory;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;

namespace StyleCart.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductVariantRepository _variantRepository;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IProductVariantRepository variantRepository)
    {
        _inventoryRepository = inventoryRepository;
        _variantRepository = variantRepository;
    }

    public async Task<InventoryResponse?> GetByProductVariantIdAsync(
        int productVariantId,
        CancellationToken cancellationToken = default)
    {
        var inventory = await _inventoryRepository.GetByProductVariantIdAsync(
            productVariantId,
            cancellationToken);

        return inventory is null ? null : MapToResponse(inventory);
    }

    public async Task<InventoryResponse> CreateAsync(
        CreateInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.QuantityInStock < 0)
        {
            throw new InvalidOperationException(
                "Stock quantity cannot be negative.");
        }

        var variant = await _variantRepository.GetByIdAsync(
            request.ProductVariantId,
            cancellationToken);

        if (variant is null)
        {
            throw new InvalidOperationException("Selected product variant does not exist.");
        }

        var existingInventory = await _inventoryRepository.GetByProductVariantIdAsync(
            request.ProductVariantId,
            cancellationToken);

        if (existingInventory is not null)
        {
            throw new InvalidOperationException(
                "Inventory already exists for this product variant.");
        }

        var inventory = new Inventory
        {
            ProductVariantId = request.ProductVariantId,
            QuantityInStock = request.QuantityInStock,
            ReservedQuantity = 0
        };

        await _inventoryRepository.AddAsync(inventory, cancellationToken);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(inventory);
    }

    public async Task<bool> UpdateAsync(
        int productVariantId,
        UpdateInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.QuantityInStock < 0)
        {
            throw new InvalidOperationException(
                "Stock quantity cannot be negative.");
        }

        var inventory = await _inventoryRepository.GetByProductVariantIdAsync(
            productVariantId,
            cancellationToken);

        if (inventory is null)
        {
            return false;
        }

        if (request.QuantityInStock < inventory.ReservedQuantity)
        {
            throw new InvalidOperationException(
                "Stock quantity cannot be lower than reserved quantity.");
        }

        inventory.QuantityInStock = request.QuantityInStock;
        inventory.UpdatedAt = DateTime.UtcNow;

        _inventoryRepository.Update(inventory);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static InventoryResponse MapToResponse(Inventory inventory)
    {
        return new InventoryResponse
        {
            Id = inventory.Id,
            ProductVariantId = inventory.ProductVariantId,
            QuantityInStock = inventory.QuantityInStock,
            ReservedQuantity = inventory.ReservedQuantity,
            UpdatedAt = inventory.UpdatedAt
        };
    }
}