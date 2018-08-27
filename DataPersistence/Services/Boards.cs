using DataPersistence.Interfaces;
using DataPersistence.Interfaces.Files;
using DataPersistence.Interfaces.SQL;
using DataPersistence.Services.Configuration;
using DataPersistence.Services.SQL;
using SharedInterfaces.Interfaces.Envelope;
using System;

namespace DataPersistence.Services
{
    public class Boards : IBoards
    {
        private IDataInMemoryCache<IEnvelope> _dataInMemoryCache { get; set; }
        private ISQLDataBaseBoardChatMessage _sQLDataBaseBoardChatMessage { get; set; }
        private IFileStorage _fileStorage { get; set; }
        private bool _isDisposed { get; set; }

        public Boards(IFileStorage fileStorage, IDataInMemoryCache<IEnvelope> dataInMemoryCache)
        {
            _fileStorage = fileStorage;
            _dataInMemoryCache = dataInMemoryCache;
            _isDisposed = false;
        }

        public IDataInMemoryCache<IEnvelope> GetHandle_DataInMemoryCache()
        {
            return _dataInMemoryCache;
        }

        public bool InitializeAllBoards()
        { 
            return true;
        }

        public IFileStorage GetHandle_FileStorage()
        {
            return _fileStorage;
        }

        public void Dispose()
        {
            try
            {
                if (_isDisposed == false)
                {
                    _dataInMemoryCache.Dispose();
                    _fileStorage.Dispose();

                    _isDisposed = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public bool InitializeBoard_SQLDataBaseBoardChatMessage()
        {
            try
            {
                if (_sQLDataBaseBoardChatMessage == null)
                    _sQLDataBaseBoardChatMessage = new SQLDataBaseBoardChatMessage(new SQLDBConfigurationProvider());
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public bool InitializeBoard_DataInMemoryCache()
        {
            throw new NotImplementedException();
        }

        public bool InitializeBoard_FileStorage()
        {
            throw new NotImplementedException();
        }

        public ISQLDataBaseBoardChatMessage GetHandle_SQLDataBaseBoardChatMessage()
        {
            throw new NotImplementedException();
        }
    }
}
