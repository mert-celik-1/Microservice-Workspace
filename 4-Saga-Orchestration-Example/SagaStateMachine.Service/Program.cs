using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Shared.Settings;
using SagaStateMachine.Service;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
        .EntityFrameworkRepository(options =>
        {
            options.AddDbContext<DbContext, OrderStateDbContext>((provider, _builder) =>
            {
                _builder.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer"));
            });
        });

    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);

        _configure.ReceiveEndpoint(RabbitMQSettings.StateMachineQueue, e => e.ConfigureSaga<OrderStateInstance>(context));
    });
});

var host = builder.Build();
host.Run();