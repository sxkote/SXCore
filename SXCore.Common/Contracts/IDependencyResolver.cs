using System;
using System.Collections.Generic;

namespace SXCore.Common.Contracts
{
    public interface IDependencyResolver : IDisposable
    {
        T Resolve<T>();
        T TryResolve<T>();

        IEnumerable<T> ResolveAll<T>();
    }
}
