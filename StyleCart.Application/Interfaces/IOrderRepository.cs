using StyleCart.Domain.Entities;

namespace StyleCart.Application.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}