using System;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Envelope;
using SharedServices.Interfaces.Transactions;


namespace SharedServices.Services.Marshall
{
    public class ImplementationTypeResolver : IImplementationTypeResolver
    { 
        private ITransactionResultFactory _transactionResultFactory { get; set; }
        private IEnvelopeFactory _envelopeFactory { get; set; }


        public ImplementationTypeResolver( 
            ITransactionResultFactory transactionResultFactory,
            IEnvelopeFactory envelope
            )
        { 
            _transactionResultFactory = transactionResultFactory;
            _envelopeFactory = envelope;
        }

        public Type ResolveImplementationType<T>()
        {
            try
            {
                Type incomingType = typeof(T);

                 
                if (incomingType == typeof(ITransactionResult))
                    return _transactionResultFactory.ResolveImplementationType();  
                else if (incomingType == typeof(IEnvelope))
                    return _envelopeFactory.ResolveImplementationType();
                else
                    throw new ApplicationException("ImplementationTypeResolver.ResolveImplementationType: The type " + incomingType.ToString() + " is not supported");

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
