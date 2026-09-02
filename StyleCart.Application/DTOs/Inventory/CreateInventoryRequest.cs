namespace StyleCart.Application.DTOs.Inventory;

public class CreateInventoryRequest
{
    public int ProductVariantId { get; set; }

    public int QuantityInStock { get; set; }
}