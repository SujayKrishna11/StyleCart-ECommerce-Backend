using StyleCart.Domain.Entities;

namespace StyleCart.Application.Interfaces;

public interface IProductVariantRepository
{
    Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<ProductVariant?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySkuAsync(
        string sku,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductVariant productVariant,
        CancellationToken cancellationToken = default);

    void Update(ProductVariant productVariant);

    void Remove(ProductVariant productVariant);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}