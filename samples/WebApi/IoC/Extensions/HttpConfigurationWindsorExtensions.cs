using System;
using System.Linq;
using System.Text;
using System.Web.Http;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;

namespace WebApi.IoC.Extensions
{
    public static class HttpConfigurationWindsorExtensions
    {
        public static IWindsorContainer UseWindsorContainer(this HttpConfiguration configuration, IWindsorContainer container)
        {
            configuration.DependencyResolver = new WindsorHttpDependencyResolver(container);
            CheckForPotentiallyMisconfiguredComponents(container);

            return container;
        }

        private static void CheckForPotentiallyMisconfiguredComponents(IWindsorContainer container)
        {
            var host = (IDiagnosticsHost)container.Kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey);
            var diagnostics = host.GetDiagnostic<IPotentiallyMisconfiguredComponentsDiagnostic>();

            var handlers = diagnostics.Inspect();

            if (!handlers.Any()) return;

            var message = new StringBuilder();
            var inspector = new DependencyInspector(message);

            foreach (var handler in handlers)
            {
                var exposeDependencyInfo = (IExposeDependencyInfo) handler;
                exposeDependencyInfo.ObtainDependencyDetails(inspector);
            }

            throw new ApplicationException(message.ToString());
        }
    }
}