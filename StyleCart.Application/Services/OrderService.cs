using StyleCart.Application.DTOs.Orders;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Domain.Enums;

namespace StyleCart.Application.Services;

public class OrderService : IOrderService
{
    private readonly ICartRepository _cartRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        ICartRepository cartRepository,
        IInventoryRepository inventoryRepository,
        IOrderRepository orderRepository)
    {
        _cartRepository = cartRepository;
        _inventoryRepository = inventoryRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse> CheckoutAsync(
        string userId,
        CheckoutRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateCheckoutRequest(request);

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Your cart is empty.");
        }

        foreach (var cartItem in cart.Items)
        {
            var inventory = await _inventoryRepository.GetByProductVariantIdAsync(
                cartItem.ProductVariantId,
                cancellationToken);

            if (inventory is null)
            {
                throw new InvalidOperationException(
                    $"Inventory does not exist for {cartItem.ProductVariant.SKU}.");
            }

            var availableQuantity =
                inventory.QuantityInStock - inventory.ReservedQuantity;

            if (availableQuantity < cartItem.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for {cartItem.ProductVariant.Product.Name} " +
                    $"({cartItem.ProductVariant.Color} / {cartItem.ProductVariant.Size}).");
            }
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            OrderStatus = OrderStatus.Processing,
            PaymentStatus = PaymentStatus.Succeeded,
            RecipientName = request.RecipientName.Trim(),
            ShippingAddressLine1 = request.ShippingAddressLine1.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Country = request.Country.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Items = cart.Items.Select(cartItem => new OrderItem
            {
                ProductVariantId = cartItem.ProductVariantId,
                ProductName = cartItem.ProductVariant.Product.Name,
                VariantDescription =
                    $"{cartItem.ProductVariant.Color} / {cartItem.ProductVariant.Size}",
                UnitPrice = cartItem.ProductVariant.Price,
                Quantity = cartItem.Quantity
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(item => item.UnitPrice * item.Quantity);

        order.Payment = new Payment
        {
            Amount = order.TotalAmount,
            Status = PaymentStatus.Succeeded,
            Provider = "MockPayment",
            TransactionReference = $"MOCK-{Guid.NewGuid():N}",
            PaidAt = DateTime.UtcNow
        };

        foreach (var cartItem in cart.Items)
        {
            var inventory = await _inventoryRepository.GetByProductVariantIdAsync(
                cartItem.ProductVariantId,
                cancellationToken);

            inventory!.QuantityInStock -= cartItem.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            _inventoryRepository.Update(inventory);
        }

        await _orderRepository.AddAsync(order, cancellationToken);

        foreach (var cartItem in cart.Items.ToList())
        {
            _cartRepository.RemoveItem(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        _cartRepository.Update(cart);

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetMyOrdersAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        return order is null ? null : MapToResponse(order);
    }

    public async Task<bool> UpdateStatusAsync(
        int orderId,
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<OrderStatus>(
                request.OrderStatus,
                ignoreCase: true,
                out var newStatus))
        {
            throw new InvalidOperationException(
                "Invalid order status. Use Processing, Shipped, Delivered, or Cancelled.");
        }

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order is null)
        {
            return false;
        }

        order.OrderStatus = newStatus;

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateCheckoutRequest(CheckoutRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RecipientName) ||
            string.IsNullOrWhiteSpace(request.ShippingAddressLine1) ||
            string.IsNullOrWhiteSpace(request.City) ||
            string.IsNullOrWhiteSpace(request.State) ||
            string.IsNullOrWhiteSpace(request.PostalCode) ||
            string.IsNullOrWhiteSpace(request.Country) ||
            string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            throw new InvalidOperationException(
                "All shipping details are required.");
        }
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            OrderStatus = order.OrderStatus.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            RecipientName = order.RecipientName,
            ShippingAddressLine1 = order.ShippingAddressLine1,
            City = order.City,
            State = order.State,
            PostalCode = order.PostalCode,
            Country = order.Country,
            PhoneNumber = order.PhoneNumber,
            Items = order.Items.Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductName,
                VariantDescription = item.VariantDescription,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}