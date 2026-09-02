using Microsoft.EntityFrameworkCore;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Data;

namespace StyleCart.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InventoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Inventory?> GetByProductVariantIdAsync(
        int productVariantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Inventories
            .FirstOrDefaultAsync(
                inventory => inventory.ProductVariantId == productVariantId,
                cancellationToken);
    }

    public async Task<Inventory?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Inventories
            .FirstOrDefaultAsync(inventory => inventory.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        Inventory inventory,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Inventories.AddAsync(inventory, cancellationToken);
    }

    public void Update(Inventory inventory)
    {
        _dbContext.Inventories.Update(inventory);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}