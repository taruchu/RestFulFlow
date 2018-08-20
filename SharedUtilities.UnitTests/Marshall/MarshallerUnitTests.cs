using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Text;
using SharedServices.Services.IOC; 
using SharedUtilities.Interfaces.Marshall;
using SharedInterfaces.Interfaces.Envelope; 

namespace SharedServices.UnitTests.Marhshall
{
    [TestClass]
    public class MarshallerUnitTests
    {   
        private ErectDIContainer _erector { get; set; }

        public MarshallerUnitTests()
        {
            _erector = new ErectDIContainer();
        }

        private IEnvelope GenerateTestEnvelope()
        {
            IEnvelope envelope = _erector.Container.Resolve<IEnvelope>(); 
            return envelope;
        }

        [TestMethod]
        public void MarshallIEnvelopeToByteArrayTest()
        {
            try
            {
                IEnvelope envelopeBefore = GenerateTestEnvelope();
                byte[] bytesActual = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(envelopeBefore, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                }));

                IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
                byte[] bytesMarshalled = marshaller.Marshall(envelopeBefore);

                Assert.AreEqual(bytesActual.Length, bytesMarshalled.Length);

                for (int i = 0; i < bytesActual.Length; i++)
                {
                    Assert.AreEqual<byte>(bytesActual[i], bytesMarshalled[i]);
                }
            }
            catch(Exception ex)
            {
                throw new AssertFailedException(ex.Message);
            }
        }

        [TestMethod]
        public void MarshallStringToByteArrayTest()
        {
            try
            {
                IEnvelope envelope = GenerateTestEnvelope();
                string JSONenvelope = JsonConvert.SerializeObject(envelope, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                byte[] bytesActual = Encoding.ASCII.GetBytes(JSONenvelope);

                IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
                byte[] bytesMarshalled = marshaller.Marshall(JSONenvelope);

                Assert.AreEqual(bytesActual.Length, bytesMarshalled.Length);

                for (int i = 0; i < bytesMarshalled.Length; i++)
                {
                    Assert.AreEqual<byte>(bytesActual[i], bytesMarshalled[i]);
                }
            }
            catch(Exception ex)
            {
                throw new AssertFailedException(ex.Message);
            } 
        }

        [TestMethod]
        public void MarshallPayloadJSONTest()
        {
            try
            {
                IEnvelope envelope = GenerateTestEnvelope();
                string JSONenvelope = JsonConvert.SerializeObject(envelope, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
                string JSONmarshalled = marshaller.MarshallPayloadJSON(envelope);

                Assert.AreEqual<string>(JSONenvelope, JSONmarshalled);
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(ex.Message);
            }  
        }

        [TestMethod]
        public void UnMarshallByteArrayToIEnvelopeTest()
        {
            try
            {
                IEnvelope envelope = GenerateTestEnvelope();
                string JSONenvelope = JsonConvert.SerializeObject(envelope, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });


                byte[] bytesActual = Encoding.ASCII.GetBytes(JSONenvelope);
                IMarshaller marshaller = _erector.Container.Resolve<IMarshaller>();
                IEnvelope unmarshalledBytes = marshaller.UnMarshall(bytesActual);
                string JSONmarshalled = JsonConvert.SerializeObject(unmarshalledBytes, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                Assert.AreEqual(JSONenvelope, JSONmarshalled);
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(ex.Message);
            } 
        }   
    }
}
