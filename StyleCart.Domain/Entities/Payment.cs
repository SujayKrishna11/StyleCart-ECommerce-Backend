using StyleCart.Domain.Enums;

namespace StyleCart.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string Provider { get; set; } = "MockPayment";

    public string? TransactionReference { get; set; }

    public DateTime? PaidAt { get; set; }

    public Order Order { get; set; } = null!;
}