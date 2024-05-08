using MassTransit;
using MongoDB.Driver;
using Saga.Orchestration.Shared.OrderEvents;
using Saga.Orchestration.Shared.Settings;
using Saga.Orchestration.Shared.StockEvents;
using Saga.Orchestration.Stock.API.Services;

namespace Saga.Orchestration.Stock.API.Consumers;

public class OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        List<bool> stockResults = new();
        var stockCollection = mongoDbService.GetCollection<Stock.API.Models.Stock>();

        foreach (var orderItem in context.Message.OrderItems)
            stockResults.Add(await (await stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >=
                (long)orderItem.Count)).AnyAsync());

        var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
        if (stockResults.TrueForAll(s => s.Equals(true)))
        {
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await (await stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                stock.Count -= orderItem.Count;

                await stockCollection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stock);
            }

            StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
            {
                OrderItems = context.Message.OrderItems
            };

            await sendEndpoint.Send(stockReservedEvent);
        }
        else
        {
            StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
            {
                Message = "Stok yetersiz...."
            };

            await sendEndpoint.Send(stockNotReservedEvent);
        }
    }
}