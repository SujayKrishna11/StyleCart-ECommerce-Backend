using StyleCart.Domain.Entities;

namespace StyleCart.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetByIdAsync(
        int cartId,
        CancellationToken cancellationToken = default);

    Task<CartItem?> GetItemByIdAsync(
        int cartItemId,
        CancellationToken cancellationToken = default);

    Task<CartItem?> GetItemAsync(
        int cartId,
        int productVariantId,
        CancellationToken cancellationToken = default);

    Task AddCartAsync(
        Cart cart,
        CancellationToken cancellationToken = default);

    Task AddItemAsync(
        CartItem cartItem,
        CancellationToken cancellationToken = default);

    void Update(Cart cart);

    void UpdateItem(CartItem cartItem);

    void RemoveItem(CartItem cartItem);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}