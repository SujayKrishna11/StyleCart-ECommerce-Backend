namespace StyleCart.Domain.Entities;

public class Inventory
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }

    public int QuantityInStock { get; set; }

    public int ReservedQuantity { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ProductVariant ProductVariant { get; set; } = null!;
}