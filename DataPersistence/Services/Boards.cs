using DataPersistence.Interfaces;
using DataPersistence.Interfaces.Files;
using DataPersistence.Interfaces.SQL;
using DataPersistence.Services.Configuration;
using DataPersistence.Services.Files;
using DataPersistence.Services.SQL;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Models.Envelope;
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

        public IFileStorage GetHandle_FileStorage()
        {
            return _fileStorage;
        }

        public ISQLDataBaseBoardChatMessage GetHandle_SQLDataBaseBoardChatMessage()
        {
            return _sQLDataBaseBoardChatMessage;
        }

        public void Dispose()
        {
            try
            {
                if (_isDisposed == false)
                {
                    _dataInMemoryCache.Dispose();
                    _fileStorage.Dispose();
                    _sQLDataBaseBoardChatMessage.Dispose();

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
                    _sQLDataBaseBoardChatMessage = new SQLDataBaseBoardChatMessage(new SQLDBConfigurationProvider(), new ChatMessageEnvelopeFactory());
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        } 
    }
}
