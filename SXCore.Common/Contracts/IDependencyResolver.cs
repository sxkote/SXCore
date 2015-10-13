using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Contracts
{
    public interface IDependencyResolver : IDisposable
    {
        T Resolve<T>();
        T TryResolve<T>();

        IEnumerable<T> ResolveAll<T>();
    }
}
