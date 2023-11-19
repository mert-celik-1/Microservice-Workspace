using Saga.Choreography.Shared.Messages;

namespace Saga.Choreography.Shared.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
