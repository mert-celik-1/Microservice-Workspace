namespace Saga.Choreography.Order.API.Models
{
    public class CreateOrderItemVM
    {
        public string ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
