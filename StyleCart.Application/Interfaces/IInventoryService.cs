using StyleCart.Application.DTOs.Inventory;

namespace StyleCart.Application.Interfaces;

public interface IInventoryService
{
    Task<InventoryResponse?> GetByProductVariantIdAsync(
        int productVariantId,
        CancellationToken cancellationToken = default);

    Task<InventoryResponse> CreateAsync(
        CreateInventoryRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int productVariantId,
        UpdateInventoryRequest request,
        CancellationToken cancellationToken = default);
}