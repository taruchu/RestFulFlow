using System;

namespace SharedUtilities.Interfaces.Marshall
{
    public interface IImplementationTypeResolver
    {
        Type ResolveImplementationType<T>();
    }
}