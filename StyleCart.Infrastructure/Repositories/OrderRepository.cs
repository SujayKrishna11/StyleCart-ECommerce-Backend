using Microsoft.EntityFrameworkCore;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Data;

namespace StyleCart.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderByDescending(order => order.OrderDate)
            .Where(order => order.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
