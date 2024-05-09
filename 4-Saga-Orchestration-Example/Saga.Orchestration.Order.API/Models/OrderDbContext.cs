using Microsoft.EntityFrameworkCore;

namespace Saga.Orchestration.Order.API.Models;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderITems { get; set; }
}