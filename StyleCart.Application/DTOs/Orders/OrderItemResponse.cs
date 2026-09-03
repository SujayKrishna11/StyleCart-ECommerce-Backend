namespace StyleCart.Application.DTOs.Orders;

public class OrderItemResponse
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string VariantDescription { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}