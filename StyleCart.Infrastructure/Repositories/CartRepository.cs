using Microsoft.EntityFrameworkCore;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;
using StyleCart.Infrastructure.Data;

namespace StyleCart.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CartRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cart?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Carts
            .Include(cart => cart.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
            .FirstOrDefaultAsync(cart => cart.UserId == userId, cancellationToken);
    }

    public async Task<Cart?> GetByIdAsync(
        int cartId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Carts
            .Include(cart => cart.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
            .FirstOrDefaultAsync(cart => cart.Id == cartId, cancellationToken);
    }

    public async Task<CartItem?> GetItemByIdAsync(
        int cartItemId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CartItems
            .FirstOrDefaultAsync(item => item.Id == cartItemId, cancellationToken);
    }

    public async Task<CartItem?> GetItemAsync(
        int cartId,
        int productVariantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CartItems
            .FirstOrDefaultAsync(
                item => item.CartId == cartId &&
                        item.ProductVariantId == productVariantId,
                cancellationToken);
    }

    public async Task AddCartAsync(
        Cart cart,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Carts.AddAsync(cart, cancellationToken);
    }

    public async Task AddItemAsync(
        CartItem cartItem,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.CartItems.AddAsync(cartItem, cancellationToken);
    }

    public void Update(Cart cart)
    {
        _dbContext.Carts.Update(cart);
    }

    public void UpdateItem(CartItem cartItem)
    {
        _dbContext.CartItems.Update(cartItem);
    }

    public void RemoveItem(CartItem cartItem)
    {
        _dbContext.CartItems.Remove(cartItem);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}