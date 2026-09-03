using StyleCart.Application.DTOs.Cart;
using StyleCart.Application.Interfaces;
using StyleCart.Domain.Entities;

namespace StyleCart.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductVariantRepository _variantRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductVariantRepository variantRepository)
    {
        _cartRepository = cartRepository;
        _variantRepository = variantRepository;
    }

    public async Task<CartResponse> GetMyCartAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

        return cart is null
            ? new CartResponse { UserId = userId }
            : MapToResponse(cart);
    }

    public async Task<CartResponse> AddItemAsync(
        string userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var variant = await _variantRepository.GetByIdAsync(
            request.ProductVariantId,
            cancellationToken);

        if (variant is null || !variant.IsActive)
        {
            throw new InvalidOperationException(
                "Selected product variant does not exist or is inactive.");
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddCartAsync(cart, cancellationToken);
            await _cartRepository.SaveChangesAsync(cancellationToken);
        }

        var existingItem = await _cartRepository.GetItemAsync(
            cart.Id,
            request.ProductVariantId,
            cancellationToken);

        if (existingItem is null)
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity
            };

            await _cartRepository.AddItemAsync(cartItem, cancellationToken);
        }
        else
        {
            existingItem.Quantity += request.Quantity;
            _cartRepository.UpdateItem(existingItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        _cartRepository.Update(cart);

        await _cartRepository.SaveChangesAsync(cancellationToken);

        var updatedCart = await _cartRepository.GetByIdAsync(
            cart.Id,
            cancellationToken);

        return MapToResponse(updatedCart!);
    }

    public async Task<bool> UpdateItemAsync(
        string userId,
        int cartItemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero. Use delete to remove an item.");
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

        if (cart is null)
        {
            return false;
        }

        var item = cart.Items.FirstOrDefault(item => item.Id == cartItemId);

        if (item is null)
        {
            return false;
        }

        item.Quantity = request.Quantity;
        cart.UpdatedAt = DateTime.UtcNow;

        _cartRepository.UpdateItem(item);
        _cartRepository.Update(cart);

        await _cartRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveItemAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

        if (cart is null)
        {
            return false;
        }

        var item = cart.Items.FirstOrDefault(item => item.Id == cartItemId);

        if (item is null)
        {
            return false;
        }

        _cartRepository.RemoveItem(item);

        cart.UpdatedAt = DateTime.UtcNow;
        _cartRepository.Update(cart);

        await _cartRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static CartResponse MapToResponse(Cart cart)
    {
        return new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            UpdatedAt = cart.UpdatedAt,
            Items = cart.Items.Select(item => new CartItemResponse
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                UnitPrice = item.ProductVariant.Price,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}