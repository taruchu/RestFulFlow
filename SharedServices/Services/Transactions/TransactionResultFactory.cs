using System;
using SharedServices.Interfaces.Transactions;
using SharedServices.Models.Transactions;

namespace SharedServices.Services.Transaction
{
    class TransactionResultFactory : ITransactionResultFactory
    {
        public ITransactionResult InstantiateITransactionResult()
        {
            return new TransactionResult();
        }

        public Type ResolveImplementationType()
        {
            return typeof(TransactionResult);
        }
    }
}
