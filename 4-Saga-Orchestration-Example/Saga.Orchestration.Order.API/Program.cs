using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Order.API.Consumers;
using Saga.Orchestration.Order.API.Models;
using Saga.Orchestration.Shared.Messages;
using Saga.Orchestration.Shared.OrderEvents;
using Saga.Orchestration.Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCompletedEventConsumer>();
    configurator.AddConsumer<OrderFailedEventConsumer>();

    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);

        _configure.ReceiveEndpoint(RabbitMQSettings.Order_OrderCompletedEventQueue, e => e.ConfigureConsumer<OrderCompletedEventConsumer>(context));

        _configure.ReceiveEndpoint(RabbitMQSettings.Order_OrderFailedEventQueue, e => e.ConfigureConsumer<OrderFailedEventConsumer>(context));
    });
});

builder.Services.AddDbContext<OrderDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderVM model, OrderDbContext context, ISendEndpointProvider sendEndpointProvider) =>
{
    Order order = new()
    {
        BuyerId = model.BuyerId,
        CreatedDate = DateTime.UtcNow,
        OrderStatus = OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(oi => oi.Count * oi.Price),
        OrderItems = model.OrderItems.Select(oi => new OrderItem
        {
            Price = oi.Price,
            Count = oi.Count,
            ProductId = oi.ProductId,
        }).ToList(),
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderStartedEvent orderStartedEvent = new()
    {
        BuyerId = model.BuyerId,
        OrderId = order.Id,
        TotalPrice = model.OrderItems.Sum(oi => oi.Count * oi.Price),
        OrderItems = model.OrderItems.Select(oi => new OrderItemMessage
        {
            Price = oi.Price,
            Count = oi.Count,
            ProductId = oi.ProductId
        }).ToList()
    };

    var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
    await sendEndpoint.Send<OrderStartedEvent>(orderStartedEvent);

});

app.Run();