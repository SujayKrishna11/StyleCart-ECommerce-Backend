using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleCart.Application.DTOs.Orders;
using StyleCart.Application.Interfaces;

namespace StyleCart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderResponse>> Checkout(
        CheckoutRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.CheckoutAsync(
            GetUserId(),
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { orderId = order.Id },
            order);
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetMyOrders(
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetMyOrdersAsync(
            GetUserId(),
            cancellationToken);

        return Ok(orders);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);

        return Ok(orders);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(
        int orderId,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(orderId, cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException(
                "User ID is missing from the token.");
    }
}