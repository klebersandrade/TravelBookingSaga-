namespace TravelBooking.Common.Events;
public record PaymentCompletedEvent: IntegrationEvent
{
    public decimal TotalAmount { get; init; }
    public string TransactionId { get; init; } = string.Empty;
}
