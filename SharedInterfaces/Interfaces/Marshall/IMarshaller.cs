
using SharedInterfaces.Interfaces.Envelope;

namespace SharedInterfaces.Interfaces.Marshaller
{
    public interface IMarshaller
    {
        byte[] Marshall(IEnvelope envelope);
        byte[] Marshall(string payload);
        string MarshallPayloadJSON<T>(T payload);
        IEnvelope UnMarshall(byte[] payload);
        IEnvelope UnMarshall(string payload);
        T UnMarshall<T>(string payload);
        T UnMarshallPayloadJSON<T>(string payload);
    }
}