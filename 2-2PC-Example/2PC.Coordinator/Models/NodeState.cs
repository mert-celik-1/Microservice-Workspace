namespace _2PC.Coordinator.Models
{
    public record NodeState(Guid TransactionId)
    {
        public Guid Id { get; set; }
        public Node Node { get; set; }
        public ReadyType IsReady { get; set; } // 1. aşama
        public TransactionState TransactionState { get; set; } // 2. aşama
    }
}
