using MassTransit;
using Saga.Orchestration.Order.API.Models;
using Saga.Orchestration.Shared.OrderEvents;

namespace Saga.Orchestration.Order.API.Consumers;

public class OrderCompletedEventConsumer(OrderDbContext orderDbContext) : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        Order.API.Models.Order order = await orderDbContext.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.OrderStatus = OrderStatus.Completed;
            await orderDbContext.SaveChangesAsync();
        }
    }
}