using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataPersistence.Interfaces
{
    public interface IFileStorage : IDisposable
    {
        /*
         * This interface will abstract reading and writing JSON objects into a XML file.
         * 
         */
        bool WriteEnvelope<T>(string filePath, string key, T envelope);
        T ReadEnvelope<T>(string filePath, string key);
        bool DeleteEnvelope<T>(string filePath, string key);
    }
}
