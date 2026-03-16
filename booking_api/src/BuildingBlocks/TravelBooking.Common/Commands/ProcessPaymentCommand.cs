using TravelBooking.Common.Events;

namespace TravelBooking.Common.Commands;
public record ProcessPaymentCommand: IntegrationEvent
{
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
}
