using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Contracts
{
    public interface IDependencyRegistrator
    {
        void RegisterType<TService, TInterface>(SXCore.Common.Enums.DependencyScope scope = SXCore.Common.Enums.DependencyScope.Default) where TService : TInterface;
    }
}
