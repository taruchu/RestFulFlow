using System;

namespace SharedInterfaces.Interfaces.Marshaller
{
    public interface IImplementationTypeResolver
    {
        Type ResolveImplementationType<T>();
    }
}