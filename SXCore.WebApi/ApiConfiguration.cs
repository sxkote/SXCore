using SXCore.Common.Contracts;
using SXCore.Common.Enums;

namespace SXCore.WebApi
{
    public abstract class ApiConfiguration
    {
        public const DependencyScope DefaultDependencyScope = DependencyScope.Scope;

        public abstract void Config(IDependencyRegistrator container, DependencyScope scope = DefaultDependencyScope);
    }
}
