using MassTransit;
using Saga.Choreography.Shared.Events;
using Saga.Choreography.Stock.API.Services;
using MongoDB.Driver;

namespace Saga.Choreography.Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly MongoDBService mongoDBService;

        public PaymentFailedEventConsumer(MongoDBService mongoDBService)
        {
            this.mongoDBService = mongoDBService;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var stocks = mongoDBService.GetCollection<Models.Stock>();
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await (await stocks.FindAsync(s => s.ProductId == orderItem.ProductId.ToString())).FirstOrDefaultAsync();
                if (stock != null)
                {
                    stock.Count += orderItem.Count;
                    await stocks.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId.ToString(), stock);
                }
            }
        }
    }
}
