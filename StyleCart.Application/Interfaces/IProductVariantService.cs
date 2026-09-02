using StyleCart.Application.DTOs.ProductVariants;

namespace StyleCart.Application.Interfaces;

public interface IProductVariantService
{
    Task<IReadOnlyList<ProductVariantResponse>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<ProductVariantResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductVariantResponse> CreateAsync(
        CreateProductVariantRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductVariantRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}