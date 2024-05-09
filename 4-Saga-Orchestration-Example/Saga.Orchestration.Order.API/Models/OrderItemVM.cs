namespace Saga.Orchestration.Order.API.Models;

public class OrderItemVM
{
    public int ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}