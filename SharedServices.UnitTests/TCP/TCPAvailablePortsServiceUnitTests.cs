using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedServices.Services.TCP;

namespace SharedServices.UnitTests.TCP
{
    [TestClass]
    public class TCPAvailablePortsServiceUnitTests
    {
        private const int _MAX_PORT = 65535;
        private const int _MIN_PORT = 49152;
        private const int _TEST_PORT = 49155;
        private const string _TEST_HOSTNAME = "127.0.0.1";
        private IPAddress _TEST_IPADDRESS { get { return IPAddress.Parse(_TEST_HOSTNAME); } }

        bool SetUpAConnectionOnTestPort()
        {                    
            TcpListener testListener = new TcpListener(_TEST_IPADDRESS, _TEST_PORT);
            testListener.Start();
            
            TcpClient testClient = new TcpClient(_TEST_HOSTNAME, _TEST_PORT);
            return testClient.Connected;            
        }

        [TestMethod]
        public void TestGetNextAvailablePortOnThisMachine()
        {
            //Get any port in range
            TCPAvailablePortsService _portService = new TCPAvailablePortsService();
            int newPort = _portService.GetNextAvailablePortOnThisMachine(_MIN_PORT, _MAX_PORT);
            Assert.IsNotNull(newPort);
            Assert.IsTrue(newPort >= _MIN_PORT && newPort <= _MAX_PORT);

            newPort = 0;
            //No available ports if only range is the test port.
            bool ready = SetUpAConnectionOnTestPort();
            try
            {
                newPort = _portService.GetNextAvailablePortOnThisMachine(_TEST_PORT, _TEST_PORT);
            }
            catch (InvalidOperationException ex)
            {
                string message = ex.Message;
                Assert.IsTrue(String.IsNullOrEmpty(message) == false);
            }

        }
    }
}
