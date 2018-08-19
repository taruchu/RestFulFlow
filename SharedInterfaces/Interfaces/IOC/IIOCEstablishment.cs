

namespace SharedInterfaces.Interfaces.IOC
{
    public interface IIOCEstablishment
    {
        IIOCContainer EstablishContainer(IIOCFactory fromFactory);
        void StandUp(IIOCContainer container);
    }
}