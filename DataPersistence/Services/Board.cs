using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using SharedUtilities.Implementation.Marshall;
using SharedUtilities.Interfaces.Marshall;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Services
{
    public class Board : IBoard
    {
        private IDataInMemoryCache<IEnvelope> _dataInMemoryCache { get; set; }
        private IFileStorage _fileStorage { get; set; }
        private bool _isDisposed { get; set; }

        public Board(IFileStorage fileStorage, IDataInMemoryCache<IEnvelope> dataInMemoryCache)
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
    }
}
