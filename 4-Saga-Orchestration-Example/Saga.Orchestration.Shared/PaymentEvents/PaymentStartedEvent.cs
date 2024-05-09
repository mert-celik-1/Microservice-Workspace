using MassTransit;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.PaymentEvents;

public class PaymentStartedEvent : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; }
    public PaymentStartedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public decimal TotalPrice { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
}