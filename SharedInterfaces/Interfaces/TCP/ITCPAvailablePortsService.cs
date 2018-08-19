
namespace SharedInterfaces.Interfaces.TCP
{
    public interface ITCPAvailablePortsService
    {  
        //NOTE: This port must be used ASAP before another
        //process grabs it from the Operating System.
        int GetNextAvailablePortOnThisMachine(int minPortNum = 49152, int maxPortNum = 65535); 
    }
}
