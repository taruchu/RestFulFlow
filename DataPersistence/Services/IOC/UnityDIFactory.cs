using SharedInterfaces.Interfaces.IOC;

namespace DataPersistence.Services.IOC
{
    public class UnityDIFactory : IIOCFactory
    {
        public UnityDIFactory()
        { }

        public IIOCContainer InstantiateContainer()
        {
            return new UnityIOC();
        }
    }
}
