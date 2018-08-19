using System;
using SharedInterfaces.Interfaces.Marshaller;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Interfaces.Transactions;


namespace SharedServices.Services.Marshall
{
    public class ImplementationTypeResolver : IImplementationTypeResolver
    { 
        private ITransactionResultFactory _transactionResultFactory { get; set; }
        private IEnvelopeFactory _envelopeFactory { get; set; }
        private IChatMessageEnvelopeFactory _chatMessageEnvelopeFactory { get; set; }


        public ImplementationTypeResolver( 
            ITransactionResultFactory transactionResultFactory,
            IEnvelopeFactory envelopeFactory , IChatMessageEnvelopeFactory chatMessageEnvelopeFactory
            )
        { 
            _transactionResultFactory = transactionResultFactory;
            _envelopeFactory = envelopeFactory;
            _chatMessageEnvelopeFactory = chatMessageEnvelopeFactory;
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
                else if (incomingType == typeof(IChatMessageEnvelope))
                    return _chatMessageEnvelopeFactory.ResolveImplementationType();
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
