using System;


namespace SharedInterfaces.Interfaces.Transactions
{
    public interface ITransactionResultFactory
    {
        ITransactionResult InstantiateITransactionResult();
        Type ResolveImplementationType();
    }
}

 