using MassTransit;
using Saga.Orchestration.Shared.PaymentEvents;
using Saga.Orchestration.Shared.Settings;

namespace Saga.Orchestration.Payment.API;

public class PaymentStartedEventConsumer(ISendEndpointProvider sendEndpointProvider) : IConsumer<PaymentStartedEvent>
{
    public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
    {
        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
        if (true)
        {

            PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId)
            {

            };

            await sendEndpoint.Send(paymentCompletedEvent);
        }
        else
        {
            PaymentFailedEvent paymentFailedEvent = new(context.Message.CorrelationId)
            {
                Message = "Yetersiz bakiye...",
                OrderItems = context.Message.OrderItems
            };

            await sendEndpoint.Send(paymentFailedEvent);
        }
    }
}