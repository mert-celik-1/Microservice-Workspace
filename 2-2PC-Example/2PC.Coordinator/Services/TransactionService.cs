using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System;
using _2PC.Coordinator.Models;

namespace _2PC.Coordinator.Services
{
    public class TransactionService : ITransactionService
    {
        TwoPhaseCommitContext _context;
        IHttpClientFactory _httpClientFactory;
        HttpClient _orderHttpClient;
        HttpClient _stockHttpClient;
        HttpClient _paymentHttpClient;

        public TransactionService(TwoPhaseCommitContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _orderHttpClient = _httpClientFactory.CreateClient("OrderAPI");
            _stockHttpClient = _httpClientFactory.CreateClient("StockAPI");
            _paymentHttpClient = _httpClientFactory.CreateClient("PaymentAPI");
        }



    public async Task<Guid> CreateTransactionAsync()
    {
        Guid transactionId = Guid.NewGuid();

        var nodes = await _context.Nodes.ToListAsync();
        nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = ReadyType.Pending,
                    TransactionState = TransactionState.Pending
                }
            });

        await _context.SaveChangesAsync();
        return transactionId;
    }
    public async Task PrepareServicesAsync(Guid transactionId)
    {
        var transactionNodes = await _context.NodeStates
               .Include(ns => ns.Node)
               .Where(ns => ns.TransactionId == transactionId)
               .ToListAsync();

        foreach (var transactionNode in transactionNodes)
        {
            try
            {
                var response = await (transactionNode.Node.Name switch
                {
                    "Order.API" => _orderHttpClient.GetAsync("ready"),
                    "Stock.API" => _stockHttpClient.GetAsync("ready"),
                    "Payment.API" => _paymentHttpClient.GetAsync("ready"),
                });

                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                transactionNode.IsReady = result ? ReadyType.Ready : ReadyType.Unready;
            }
            catch (Exception)
            {
                transactionNode.IsReady = ReadyType.Unready;
            }
        }

        await _context.SaveChangesAsync();
    }
    public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
        => (await _context.NodeStates
                     .Where(ns => ns.TransactionId == transactionId)
                     .ToListAsync()).TrueForAll(ns => ns.IsReady == ReadyType.Ready);
    public async Task CommitAsync(Guid transactionId)
    {
        var transactionNodes = await _context.NodeStates
                                .Where(ns => ns.TransactionId == transactionId)
                                .Include(ns => ns.Node)
                                .ToListAsync();

        foreach (var transactionNode in transactionNodes)
        {
            try
            {
                var response = await (transactionNode.Node.Name switch
                {
                    "Order.API" => _orderHttpClient.GetAsync("commit"),
                    "Stock.API" => _stockHttpClient.GetAsync("commit"),
                    "Payment.API" => _paymentHttpClient.GetAsync("commit")
                });

                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                transactionNode.TransactionState = result ? TransactionState.Done : TransactionState.Abort;
            }
            catch
            {
                transactionNode.TransactionState = TransactionState.Abort;
            }
        }

        await _context.SaveChangesAsync();
    }
    public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
        => (await _context.NodeStates
        .Where(ns => ns.TransactionId == transactionId)
        .ToListAsync()).TrueForAll(ns => ns.TransactionState == TransactionState.Done);
    public async Task RollbackAsync(Guid transactionId)
    {
        var transactionNodes = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .Include(ns => ns.Node)
            .ToListAsync();

        foreach (var transactionNode in transactionNodes)
        {
            try
            {
                if (transactionNode.TransactionState == TransactionState.Done)
                    _ = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("rollback"),
                        "Stock.API" => _stockHttpClient.GetAsync("rollback"),
                        "Payment.API" => _paymentHttpClient.GetAsync("rollback"),
                    });

                transactionNode.TransactionState = TransactionState.Abort;
            }
            catch
            {
                transactionNode.TransactionState = TransactionState.Abort;
            }
        }

        await _context.SaveChangesAsync();
    }
}
}
