using MassTransit;
using MongoDB.Driver;
using Saga.Orchestration.Shared.Messages;
using Saga.Orchestration.Stock.API.Services;

namespace Saga.Orchestration.Stock.API.Consumers;

public class StockRollbackMessageConsumer(MongoDbService mongoDbService) : IConsumer<StockRollbackMessage>
{
    public async Task Consume(ConsumeContext<StockRollbackMessage> context)
    {
        var stockCollection = mongoDbService.GetCollection<Stock.API.Models.Stock>();

        foreach (var orderItem in context.Message.OrderItems)
        {
            var stock = await (await stockCollection.FindAsync(x => x.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

            stock.Count += orderItem.Count;
            await stockCollection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stock);
        }
    }
}