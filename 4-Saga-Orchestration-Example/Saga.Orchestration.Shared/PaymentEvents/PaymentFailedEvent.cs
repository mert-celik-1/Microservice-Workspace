using MassTransit;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.PaymentEvents;

public class PaymentFailedEvent : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; }

    public PaymentFailedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public string Message { get; set; }
    public List<OrderItemMessage> OrderItems { get; set; }
}