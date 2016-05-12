using Autofac;
using SXCore.Common.Contracts;
using System.Collections.Generic;

namespace SXCore.Infrastructure.Services.Dependency
{
    public class AutofacResolver : IDependencyResolver
    {
        private IContainer _container;
        private ILifetimeScope _scope;

        public AutofacResolver(IContainer container)
        {
            _container = container;
        }

        private ILifetimeScope ActiveScope
        {
            get
            {
                // If there is an active HttpContext, retrieve the lifetime scope by resolving
                // the ILifetimeScopeProvider from Funq.  Otherwise, use the application (root) container.

                //return HttpContext.Current == null
                //            ? _container
                //            : _funqContainer.Resolve<ILifetimeScopeProvider>().GetLifetimeScope(null);

                if (_scope == null)
                    _scope = _container.BeginLifetimeScope();

                return _scope;
            }
        }

        public T Resolve<T>()
        {
            return this.ActiveScope.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return this.ActiveScope.Resolve<IEnumerable<T>>();
        }

        public T TryResolve<T>()
        {
            T result;

            if (this.ActiveScope.TryResolve<T>(out result))
            {
                return result;
            }

            return default(T);
        }

        public void Dispose()
        {
            if (_scope != null)
                _scope.Dispose();

            if (_container != null)
                _container.Dispose();
        }
    }
}
