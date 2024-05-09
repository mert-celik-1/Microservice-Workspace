using MassTransit;

namespace Saga.Orchestration.Shared.PaymentEvents;

public class PaymentCompletedEvent : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; }

    public PaymentCompletedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
}