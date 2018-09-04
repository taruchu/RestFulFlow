using SharedInterfaces.Interfaces.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharedServices.Services.Routing
{
    public class MessageBusReaderBank<T> : IMessageBusReaderBank<T>
    {
        private class ReaderTask<T>
        {
            public CancellationTokenSource TokenSource { get; set; }
            public Task CachedReaderTask { get; set; }
            private Action<T> _performedOnEachMessageRead { get; set; }
            private IMessageBus<T> _messageBus { get; set; }
            private void ReadMessageBus()
            {
                while(TokenSource.IsCancellationRequested == false)
                {
                    while(_messageBus.IsEmpty()) { }
                    T message = _messageBus.ReceiveMessage();
                    if(message != null)
                        _performedOnEachMessageRead(message);
                } 
            }
            public ReaderTask(Action<T> performedOnEachMessageRead, IMessageBus<T> messageBus)
            {
                if (messageBus != null && performedOnEachMessageRead != null)
                {
                    _performedOnEachMessageRead = performedOnEachMessageRead;
                    _messageBus = messageBus;
                    TokenSource = new CancellationTokenSource();
                    Action readerTaskAction = ReadMessageBus;
                    CachedReaderTask = Task.Run(readerTaskAction, TokenSource.Token); 
                }
            }
        }
        private IMessageBus<T> _messageBus { get; set; }
        private Queue<ReaderTask<T>> _bank { get; set; }
        private bool isDisposed { get; set; }
        public string ExceptionMessage_MessageBusCannotBeNull
        {
            get
            {
                return "MessageBusReaderBank<T> - Message bus cannot be null.";
            }
        } 
        public string ExceptionMessage_ActionPerformedOnEachMessageReadCannotBeNull
        {
            get
            {
                return "MessageBusReaderBank<T> - Action performedOnEachMessageRead cannot be null.";
            }
        }

        public MessageBusReaderBank()
        {
            _bank = new Queue<ReaderTask<T>>();
            isDisposed = false;
        }

        public int AddAnotherReader(Action<T> performedOnEachMessageRead)
        {
            try
            {
                if (_messageBus == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusCannotBeNull);
                else if (performedOnEachMessageRead == null)
                    throw new InvalidOperationException(ExceptionMessage_ActionPerformedOnEachMessageReadCannotBeNull);
                else
                {
                    ReaderTask<T> readerTask = new ReaderTask<T>(performedOnEachMessageRead, _messageBus);
                    _bank.Enqueue(readerTask);
                    return _bank.Count();
                } 
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public int DecreaseReaderBank(int byAmount)
        {
            try
            {
                if(byAmount > 0 && byAmount <= _bank.Count())
                {
                    ReaderTask<T> readerTask = _bank.Dequeue();
                    readerTask.TokenSource.Cancel();
                    readerTask.CachedReaderTask.Wait(); 
                }
                return _bank.Count();
            } 
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public void Dispose()
        {
            if(isDisposed == false)
            {
                if(StopReading())
                    _bank.Clear();
                _messageBus = null;
                isDisposed = true;
            }
        }
          
        public string SpecifyTheMessageBus(IMessageBus<T> toRead)
        {
            try
            {
                if (toRead == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusCannotBeNull);
                else
                { 
                    _messageBus = toRead;
                    return _messageBus.MessageBusGUID;
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
               
        public bool StopReading()
        {
            foreach (ReaderTask<T> task in _bank)
            {
                task.TokenSource.Cancel();
                task.CachedReaderTask.Wait(); 
            }
            return true;
        }

        public async Task<T> PollMessageBusForSingleMessage(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                if (_messageBus == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusCannotBeNull);

                while (_messageBus.IsEmpty() && cancellationTokenSource.IsCancellationRequested == false) { }
                if (cancellationTokenSource.IsCancellationRequested)
                    return default(T);
                else
                    return _messageBus.ReceiveMessage(); 
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
