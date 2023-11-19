using Saga.Choreography.Shared.Messages;
namespace Saga.Choreography.Shared.Events
{
    public class PaymentFailedEvent
    {
        public Guid OrderId { get; set; }
        public string Message { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
