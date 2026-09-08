namespace StyleCart.Application.DTOs.Products;

public class BulkProductImportResult
{
    public int ImportedCount { get; set; }

    public List<ProductResponse> Products { get; set; } = [];

    public List<string> Errors { get; set; } = [];

    public bool IsSuccessful => Errors.Count == 0;
}