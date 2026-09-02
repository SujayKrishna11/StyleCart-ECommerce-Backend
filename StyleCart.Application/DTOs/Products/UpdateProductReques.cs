namespace StyleCart.Application.DTOs.Products;

public class UpdateProductRequest
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Brand { get; set; }

    public decimal BasePrice { get; set; }

    public bool IsActive { get; set; }
}