using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.Windsor;

namespace WebApi.IoC
{
    /// <summary>
    /// This class won't create or dispose scope as it assumes this is done at Owin level, so that scope can be
    /// unique between middlewares and web api
    /// </summary>
    internal class OwinWindsorDependencyScope : IDependencyScope
    {
        private readonly IWindsorContainer _container;

        public OwinWindsorDependencyScope(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            _container = container;
        }

        public object GetService(Type t)
        {
            return _container.Kernel.HasComponent(t)
                ? _container.Resolve(t) : null;
        }

        public IEnumerable<object> GetServices(Type t)
        {
            return _container.ResolveAll(t)
                .Cast<object>().ToArray();
        }

        public void Dispose()
        {
        }
    }
}