using _2PC.Coordinator.Models;
using _2PC.Coordinator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TwoPhaseCommitContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

builder.Services.AddHttpClient("OrderAPI", client => client.BaseAddress = new("https://localhost:7120/"));
builder.Services.AddHttpClient("StockAPI", client => client.BaseAddress = new("https://localhost:7159/"));
builder.Services.AddHttpClient("PaymentAPI", client => client.BaseAddress = new("https://localhost:7242/"));

builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1 - Prepare
    var transactionId = await transactionService.CreateTransactionAsync();
    await transactionService.PrepareServicesAsync(transactionId);
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

    if (transactionState)
    {
        //Phase 2 - Commit
        await transactionService.CommitAsync(transactionId);
        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
    }

    if (!transactionState)
        await transactionService.RollbackAsync(transactionId);
});


app.Run();