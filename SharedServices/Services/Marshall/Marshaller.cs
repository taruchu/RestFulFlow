using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using SharedServices.Interfaces.Marshaller;
using SharedServices.Interfaces.Envelope;
using System;

namespace SharedServices.Services.Marshall
{
    public class Marshaller : IMarshaller
    { 
        private IImplementationTypeResolver _implementationTypeResolver  { get; set; }
        public Marshaller(IImplementationTypeResolver implementationTypeResolver)
        {
            _implementationTypeResolver = implementationTypeResolver;
        }
        public byte[] Marshall(IEnvelope envelope)
        {
            try
            {
                string json = MarshallPayloadJSON<IEnvelope>(envelope);
                return Encoding.ASCII.GetBytes(json);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public byte[] Marshall(string payload)
        {
            return Encoding.ASCII.GetBytes(payload);
        }

        public string MarshallPayloadJSON<T>(T payload)
        {
            try
            {
                return JsonConvert.SerializeObject(payload, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto //NOTE: Will resolve composite interface members
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public IEnvelope UnMarshall(byte[] payload)
        {
            try
            {
                string json = Encoding.UTF8.GetString(payload);
                return UnMarshall(json);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public IEnvelope UnMarshall(string payload)
        {
            try
            {
                return (IEnvelope)UnMarshall<IEnvelope>(payload);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public T UnMarshallPayloadJSON<T>(string payload)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(payload, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto //NOTE: Will resolve composite members 
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public T UnMarshall<T>(string payload)
        {
            try
            {
                //Jesus :-)
                MethodInfo method = typeof(Marshaller).GetMethod("UnMarshallPayloadJSON");
                MethodInfo boundGenericMethod =
                    method.MakeGenericMethod(_implementationTypeResolver.ResolveImplementationType<T>());

                return (T)boundGenericMethod.Invoke(this, new object[] { payload });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
