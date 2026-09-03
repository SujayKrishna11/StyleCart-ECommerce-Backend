namespace StyleCart.Application.DTOs.Orders;

public class OrderResponse
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public string RecipientName { get; set; } = string.Empty;

    public string ShippingAddressLine1 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public List<OrderItemResponse> Items { get; set; } = [];
}