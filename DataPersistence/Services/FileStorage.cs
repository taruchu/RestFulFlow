using DataPersistence.Interfaces;
using SharedUtilities.Interfaces.Marshall;
using System;

namespace DataPersistence.Services
{
    public class FileStorage : IFileStorage
    {
        private object _thisLock { get; set; }
        private bool _isDisposed { get; set; } 
        private FileDatabaseIOController<FileDatabase> _fileDatabaseIOController { get; set; }
        private IMarshaller _marshaller { get; set; }
        

        public FileStorage(IMarshaller marshaller)
        {
            _isDisposed = false;
            _thisLock = new object(); 
            _fileDatabaseIOController = new FileDatabaseIOController<FileDatabase>();
            _marshaller = marshaller;
        }

        public void Dispose()
        { 
        }

        public T ReadEnvelope<T>(string filePath, string key)
        {
            lock (_thisLock)
            {
                try
                {
                    FileDatabase database = _fileDatabaseIOController.Deserialize(filePath);
                    string data = database.BlobTable[key];
                    T envelope = _marshaller.UnMarshall<T>(data);
                    return (envelope == null) ? default(T) : envelope;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }  
            }          
        }

        public bool WriteEnvelope<T>(string filePath, string key, T envelope)
        {
            lock(_thisLock)
            {
                try
                {
                    string data = _marshaller.MarshallPayloadJSON(envelope);
                    FileDatabase fileDatabase = _fileDatabaseIOController.Deserialize(filePath);
                    fileDatabase.BlobTable.Add(key, data);
                    _fileDatabaseIOController.Serialize(filePath, fileDatabase);
                    return true;
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public bool DeleteEnvelope<T>(string filePath, string key)
        {
            lock (_thisLock)
            {
                try
                {
                    FileDatabase database = _fileDatabaseIOController.Deserialize(filePath);
                    database.BlobTable.Remove(key);
                    _fileDatabaseIOController.Serialize(filePath, database);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }
    }
}
