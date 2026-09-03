using StyleCart.Application.DTOs.Cart;

namespace StyleCart.Application.Interfaces;

public interface ICartService
{
    Task<CartResponse> GetMyCartAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<CartResponse> AddItemAsync(
        string userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateItemAsync(
        string userId,
        int cartItemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveItemAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default);
}