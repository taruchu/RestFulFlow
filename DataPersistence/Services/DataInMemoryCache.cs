using DataPersistence.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Services
{
    public class DataInMemoryCache<T> : IDataInMemoryCache<T>
    {
        private ConcurrentDictionary<long, T> _dataCache { get; set; }
        private bool _isDisposed { get; set; }
        private object _thisLock { get; set; }

        public string ExceptionMessage_DataCannotBeNull
        {
            get
            {
                return "DataInMemoryCache - Data parameter cannot be null.";
            }
        }

        public string ExceptionMessage_IDCannotBeNegative
        {
            get
            {
                return "DataInMemoryCache - ID cannot be negative.";
            }
        }

        public DataInMemoryCache()
        {
            _thisLock = new object();
            _isDisposed = false;
            _dataCache = new ConcurrentDictionary<long, T>();
        }

        public bool DELETE(long ID)
        {
            lock (_thisLock)
            {
                try
                {
                    if (ID < 0)
                        throw new InvalidOperationException(ExceptionMessage_IDCannotBeNegative);

                    T removed;
                    return _dataCache.TryRemove(ID, out removed);
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

        public T GET(long ID)
        {
            lock (_thisLock)
            {
                try
                {
                    if (ID < 0)
                        throw new InvalidOperationException(ExceptionMessage_IDCannotBeNegative);

                    T getData;
                    return (_dataCache.TryGetValue(ID, out getData)) ? getData : default(T);                
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
        }

        public bool POST(long ID, T data)
        {
            lock (_thisLock)
            {
                try
                {
                    if (data == null)
                        throw new InvalidOperationException(ExceptionMessage_DataCannotBeNull);
                    if (data == null)
                        throw new InvalidOperationException(ExceptionMessage_DataCannotBeNull);

                    return _dataCache.TryAdd(ID, data);
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
        }

        public bool PUT(long ID, T data)
        {
            lock (_thisLock)
            {
                try
                {
                    if (ID < 0)
                        throw new InvalidOperationException(ExceptionMessage_IDCannotBeNegative);
                    if (data == null)
                        throw new InvalidOperationException(ExceptionMessage_DataCannotBeNull);

                    _dataCache.AddOrUpdate(ID, data, (key, val) => data);
                    return true;
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
        }

        public void Dispose()
        {
            if(_isDisposed == false)
            {
                _dataCache.Clear();
                _isDisposed = true;
            }
        }
    }
}
