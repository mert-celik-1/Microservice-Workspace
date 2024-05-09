namespace Saga.Orchestration.Order.API.Models;

public class CreateOrderVM
{
    public int BuyerId { get; set; }
    public List<OrderItemVM> OrderItems { get; set; }
}