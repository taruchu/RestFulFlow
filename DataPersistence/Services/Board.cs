using DataPersistence.Interfaces;
using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Services
{
    public class Board : IBoard
    {
        private IDataInMemoryCache<IEnvelope> _dataInMemoryCache { get; set; }

        public Board()
        {
            
        }

        public IDataInMemoryCache<IEnvelope> GetHandle_DataInMemoryCache()
        {
            return _dataInMemoryCache;
        }

        public bool InitializeAllBoards()
        {
            _dataInMemoryCache = new DataInMemoryCache<IEnvelope>();
            return true;
        }
    }
}
