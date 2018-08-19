

namespace SharedInterfaces.Interfaces.IOC
{
    public interface IIOCContainer
    {
        IIOCContainer Register<Type, ForClass>() where ForClass : Type;
        IIOCContainer RegisterSingleton<Type, ForClass>() where ForClass : Type;
        Type Resolve<Type>();
    }
}