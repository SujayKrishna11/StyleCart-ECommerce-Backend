namespace StyleCart.Application.DTOs.Cart;

public class CartItemResponse
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}