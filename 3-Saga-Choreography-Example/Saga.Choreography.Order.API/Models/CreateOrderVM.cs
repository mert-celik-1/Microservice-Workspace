namespace Saga.Choreography.Order.API.Models
{
    public class CreateOrderVM
    {
        public string BuyerId { get; set; }
        public List<CreateOrderItemVM> OrderItems { get; set; }
    }
}
