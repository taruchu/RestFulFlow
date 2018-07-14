using System;


namespace SharedServices.Interfaces.Transactions
{
    public interface ITransactionResultFactory
    {
        ITransactionResult InstantiateITransactionResult();
        Type ResolveImplementationType();
    }
}

 