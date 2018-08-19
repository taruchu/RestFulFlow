using System;


namespace SharedInterfaces.Interfaces.Transactions
{
    public interface ITransactionResult
    {
        TransactionOutcome outcome { get; set; }
        SeverityLevel severity { get; set; }
        String errorMessage { get; set; }
        String extraDetails { get; set; }
    }
}

 