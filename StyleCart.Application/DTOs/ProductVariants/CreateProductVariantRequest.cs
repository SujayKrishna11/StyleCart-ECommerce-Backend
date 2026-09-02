namespace StyleCart.Application.DTOs.ProductVariants;

public class CreateProductVariantRequest
{
    public int ProductId { get; set; }

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public decimal Price { get; set; }
}