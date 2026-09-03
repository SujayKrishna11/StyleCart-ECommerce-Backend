using StyleCart.Application.DTOs.Orders;

namespace StyleCart.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CheckoutAsync(
        string userId,
        CheckoutRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderResponse>> GetMyOrdersAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<OrderResponse?> GetByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(
    int orderId,
    UpdateOrderStatusRequest request,
    CancellationToken cancellationToken = default);
}