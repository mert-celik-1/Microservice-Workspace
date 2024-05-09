using MassTransit;
using Saga.Orchestration.Order.API.Models;
using Saga.Orchestration.Shared.OrderEvents;

namespace Saga.Orchestration.Order.API.Consumers;

public class OrderFailedEventConsumer(OrderDbContext orderDbContext) : IConsumer<OrderFailedEvent>
{
    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        Order.API.Models.Order order = await orderDbContext.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.OrderStatus = OrderStatus.Fail;
            await orderDbContext.SaveChangesAsync();
        }
    }
}