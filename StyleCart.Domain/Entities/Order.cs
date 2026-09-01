using StyleCart.Domain.Enums;

namespace StyleCart.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string RecipientName { get; set; } = string.Empty;

    public string ShippingAddressLine1 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }

}