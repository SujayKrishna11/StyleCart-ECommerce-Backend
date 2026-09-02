using StyleCart.Domain.Entities;

namespace StyleCart.Application.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductVariantIdAsync(
        int productVariantId,
        CancellationToken cancellationToken = default);

    Task<Inventory?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Inventory inventory,
        CancellationToken cancellationToken = default);

    void Update(Inventory inventory);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}