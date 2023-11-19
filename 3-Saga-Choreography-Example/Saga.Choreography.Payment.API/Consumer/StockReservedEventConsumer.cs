using MassTransit;
using Saga.Choreography.Shared.Events;

namespace Saga.Choreography.Payment.API.Consumer
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            publishEndpoint = publishEndpoint;
        }


        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            if (false)
            {
                //Ödeme başarılı...
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };
                await publishEndpoint.Publish(paymentCompletedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarılı...");
            }
            else
            {
                //Ödeme başarısız...
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Yetersiz bakiye...",
                    OrderItems = context.Message.OrderItems
                };
                await publishEndpoint.Publish(paymentFailedEvent);
                await Console.Out.WriteLineAsync("Ödeme başarısız...");
            }
        }
    }
}
