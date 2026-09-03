namespace StyleCart.Application.DTOs.Cart;

public class CartResponse
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }

    public List<CartItemResponse> Items { get; set; } = [];
}