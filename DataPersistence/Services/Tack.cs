using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Services
{
    public class Tack : ITack
    {
        public IBoards Boards { get { return _boards; } }
        private IBoards _boards { get; set; }
        public ISkyWatch SkyWatch { get; set; }
        private string _iTackGUID { get; set; }
        private object _thisLock { get; set; }
        private bool _isDisposed { get; set; }
        private const string _FILE_DB_PATH = "fileDB.xml"; //TODO: This path should be in a shared location external to this assembly.

        public Tack(IBoards boards)
        {
            _boards = boards; 
            _iTackGUID = Guid.NewGuid().ToString();
            _thisLock = new object();
            _isDisposed = false;
        }


        public IEnvelope DELETE(IEnvelope envelope)
        {
            lock(_thisLock)
            {
                try
                {
                    if(envelope.GetMyEnvelopeType() == typeof(IChatMessageEnvelope))
                    {
                        return _boards.GetHandle_SQLDataBaseBoardChatMessage().DELTE(envelope);
                    }
                    return envelope;
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public void Dispose()
        {
            try
            {
                if (_isDisposed == false)
                {
                    SkyWatch.Dispose();
                    _boards.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
            
        }

        public IEnvelope GET(IEnvelope envelope)
        {
            lock (_thisLock)
            {
                try
                {
                     
                    if (envelope.GetMyEnvelopeType() == typeof(IChatMessageEnvelope))
                    {
                        return _boards.GetHandle_SQLDataBaseBoardChatMessage().GET(envelope);
                    }
                    return envelope;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }
          
        public IEnvelope POST(IEnvelope envelope)
        {
            lock (_thisLock)
            {
                try
                {
                    if (envelope.GetMyEnvelopeType() == typeof(IChatMessageEnvelope))
                    {
                        return _boards.GetHandle_SQLDataBaseBoardChatMessage().POST(envelope);
                    }
                    return envelope;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public IEnvelope PUT(IEnvelope envelope)
        {
            lock (_thisLock)
            {
                try
                {
                    if (envelope.GetMyEnvelopeType() == typeof(IChatMessageEnvelope))
                    {
                        return _boards.GetHandle_SQLDataBaseBoardChatMessage().PUT(envelope);
                    }
                    return envelope;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        private void SkyWatchEventHandler(ISkyWatchEventTypes skyWatchEventType, string eventKey)
        {
            lock (_thisLock)
            {
                try
                {
                    if (skyWatchEventType == ISkyWatchEventTypes.WriteOccured)
                    {
                        //TODO:  re-new the in memory data cache for this eventKey using the data stored on disk. Each envelope type will
                        //have a specific storage mechanism (file, SQL, NOSQL, etc..).
                        //I may want to cache certain envelope types (for example: user profile information )

                        Type envelopeType = GetIEnvelopeType(eventKey);
                        long ID = GetStorageID(eventKey);

                                        
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
