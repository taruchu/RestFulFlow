using SharedServices.Interfaces.Transactions;

namespace SharedServices.Models.Transactions
{
    public class TransactionResult : ITransactionResult
    {
        public TransactionOutcome outcome { get; set; }
        public SeverityLevel severity { get; set; }
        public string errorMessage { get; set; }
        public string extraDetails { get; set; }
    }
}
