using System;

namespace SharedServices.Interfaces.Marshaller
{
    public interface IImplementationTypeResolver
    {
        Type ResolveImplementationType<T>();
    }
}