namespace StyleCart.Application.DTOs.Inventory;

public class InventoryResponse
{
    public int Id { get; set; }

    public int ProductVariantId { get; set; }

    public int QuantityInStock { get; set; }

    public int ReservedQuantity { get; set; }

    public DateTime UpdatedAt { get; set; }
}
