using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleCart.Application.DTOs.ProductVariants;
using StyleCart.Application.Interfaces;

namespace StyleCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantsController : ControllerBase
{
    private readonly IProductVariantService _variantService;

    public ProductVariantsController(IProductVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductVariantResponse>>> GetByProductId(
        [FromQuery] int productId,
        CancellationToken cancellationToken)
    {
        var variants = await _variantService.GetByProductIdAsync(
            productId,
            cancellationToken);

        return Ok(variants);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductVariantResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var variant = await _variantService.GetByIdAsync(id, cancellationToken);

        return variant is null ? NotFound() : Ok(variant);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductVariantResponse>> Create(
        CreateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var variant = await _variantService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = variant.Id }, variant);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductVariantRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _variantService.UpdateAsync(id, request, cancellationToken);

        return updated ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _variantService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}