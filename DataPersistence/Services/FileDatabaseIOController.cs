using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace DataPersistence.Services
{
    public class FileDatabaseIOController<T>
    {
        public FileDatabaseIOController() { }

        public void Serialize(string filePath, T data)
        {
            try
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                using (var writer = new FileStream(filePath, FileMode.Truncate, FileAccess.Write, FileShare.Read))
                {
                    dataContractSerializer.WriteObject(writer, data);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public T Deserialize(string filePath)
        {
            try
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                T data;
                using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    data = (T)dataContractSerializer.ReadObject(reader);
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
