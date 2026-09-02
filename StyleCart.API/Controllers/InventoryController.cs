using Microsoft.AspNetCore.Mvc;
using StyleCart.Application.DTOs.Inventory;
using StyleCart.Application.Interfaces;

namespace StyleCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<ActionResult<InventoryResponse>> GetByProductVariantId(
        [FromQuery] int productVariantId,
        CancellationToken cancellationToken)
    {
        var inventory = await _inventoryService.GetByProductVariantIdAsync(
            productVariantId,
            cancellationToken);

        return inventory is null ? NotFound() : Ok(inventory);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryResponse>> Create(
        CreateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var inventory = await _inventoryService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetByProductVariantId),
            new { productVariantId = inventory.ProductVariantId },
            inventory);
    }

    [HttpPut("product-variant/{productVariantId:int}")]
    public async Task<IActionResult> Update(
        int productVariantId,
        UpdateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _inventoryService.UpdateAsync(
            productVariantId,
            request,
            cancellationToken);

        return updated ? NoContent() : NotFound();
    }
}