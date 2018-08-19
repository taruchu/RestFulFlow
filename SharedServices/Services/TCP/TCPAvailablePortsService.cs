using SharedInterfaces.Interfaces.TCP;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace SharedServices.Services.TCP
{
    public class TCPAvailablePortsService : ITCPAvailablePortsService
    {
        private IPGlobalProperties _iPGlobalProperties { get { return IPGlobalProperties.GetIPGlobalProperties(); } }
        private IPEndPoint[] _activeIPEndPoints { get { return _iPGlobalProperties.GetActiveTcpListeners(); } }

        public TCPAvailablePortsService()
        { }
         
        public int GetNextAvailablePortOnThisMachine(int minPortNum = 49152, int maxPortNum = 65535)
        { 
            try
            {
                int maxActivePort = _activeIPEndPoints.Select(endpt => endpt.Port).ToList<int>().Max();
                if (minPortNum > maxActivePort)
                    return minPortNum;
                else if (maxPortNum > maxActivePort)
                    return maxActivePort += 1;
                else
                    throw new InvalidOperationException("TCPAvailablePortsService.GetNextAvailablePortOnThisMachine() - No available ports in this range.");
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
}
