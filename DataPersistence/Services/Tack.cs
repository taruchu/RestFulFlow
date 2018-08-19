using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Services
{
    public class Tack : ITack
    {
        private IBoard _board { get; set; }
        public ISkyWatch SkyWatch { get; set; }
        private string _iTackGUID { get; set; }
        private object _thisLock { get; set; }

        public Tack(IBoard board)
        {
            _board = board;
            _board.InitializeAllBoards();
            _iTackGUID = Guid.NewGuid().ToString();
            _thisLock = new object();
        }


        public IEnvelope DELETE(IEnvelope envelope)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnvelope GET(IEnvelope envelope)
        {
            throw new NotImplementedException();
        }
          
        public IEnvelope POST(IEnvelope envelope)
        {
            throw new NotImplementedException();
        }

        public IEnvelope PUT(IEnvelope envelope)
        {
            throw new NotImplementedException();
        }

        private void SkyWatchEventHandler(ISkyWatchEventTypes skyWatchEventType, string eventKey)
        {
            lock (_thisLock)
            {
                try
                {
                    if (skyWatchEventType == ISkyWatchEventTypes.WriteOccured)
                    {
                        //TODO:  re-new the in memory data cache for this eventKey using the data stored on disk

                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                } 
            }
        }

        private void DeclareEventToSkyWatch(ISkyWatchEventTypes skyWatchEventType, Type envelopeType, long storageID)
        {
            lock (_thisLock)
            {
                try
                {

                    string eventKey = CreateEventKey(envelopeType, storageID); 
                    SkyWatch.Declare(skyWatchEventType, eventKey);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        string CreateEventKey(Type envelopeType, long storageID)
        {
            //NOTE: eventKey = {IEnvelope type}.{Storage ID}  for example:  IChatMessageEnvelope.323
            return String.Format("{0}.{1}", envelopeType.ToString(), storageID.ToString());
        }

        long GetStorageID(string eventKey)
        {
            //NOTE: eventKey = {IEnvelope type}.{Storage ID}  for example:  IChatMessageEnvelope.323
            long storageID;
            if (Int64.TryParse(eventKey.Split('.')[1], out storageID) == false)
                throw new InvalidCastException("Tack - GetStorageID() could not Parse the eventKey.");

            return storageID;
        }

        Type GetIEnvelopeType(string eventKey)
        {
            //NOTE: eventKey = {IEnvelope type}.{Storage ID}  for example:  IChatMessageEnvelope.323
            return Type.GetType(eventKey.Split('.')[0]); 
        }

        private void SubscribeToSkyWatch(Type envelopeType, long storageID)
        { 
            try
            { 
                string eventKey = CreateEventKey(envelopeType, storageID);
                SkyWatch.Watch(eventKey, _iTackGUID, SkyWatchEventHandler);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }  
        }

        private void UnsubscribeToSkyWatch(Type envelopeType, long storageID)
        { 
            try
            {
                string eventKey = CreateEventKey(envelopeType, storageID); 
                SkyWatch.UnWatch(eventKey, _iTackGUID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }  
        }
        
    }
}
