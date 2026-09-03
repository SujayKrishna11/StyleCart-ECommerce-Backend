namespace StyleCart.Application.DTOs.Orders;

public class CheckoutRequest
{
    public string RecipientName { get; set; } = string.Empty;

    public string ShippingAddressLine1 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}