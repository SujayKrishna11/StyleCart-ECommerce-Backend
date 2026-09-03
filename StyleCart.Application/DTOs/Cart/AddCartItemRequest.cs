namespace StyleCart.Application.DTOs.Cart;

public class AddCartItemRequest
{
    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }
}
