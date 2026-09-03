using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleCart.Application.DTOs.Cart;
using StyleCart.Application.Interfaces;

namespace StyleCart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetMyCart(
        CancellationToken cancellationToken)
    {
        var cart = await _cartService.GetMyCartAsync(
            GetUserId(),
            cancellationToken);

        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem(
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var cart = await _cartService.AddItemAsync(
            GetUserId(),
            request,
            cancellationToken);

        return Ok(cart);
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateItem(
        int cartItemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _cartService.UpdateItemAsync(
            GetUserId(),
            cartItemId,
            request,
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(
        int cartItemId,
        CancellationToken cancellationToken)
    {
        var deleted = await _cartService.RemoveItemAsync(
            GetUserId(),
            cartItemId,
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID is missing from the token.");
    }
}