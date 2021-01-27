using System;
using System.Web.Http;
using Castle.Windsor;
using Microsoft.Owin;
using Owin;
using SamplesShared;
using WebApi;
using WebApi.Extensions;
using WebApi.IoC.Extensions;
using WebApi.IoC.Installer;

[assembly: OwinStartup(typeof(Startup))]

namespace WebApi
{
    public class Startup
    {
        protected static readonly IWindsorContainer Container = new WindsorContainer();

        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            Container.Install(
                new ControllerInstaller(),
                new DependencyInstaller());

            config.UseWindsorContainer(Container);
            config.UseDefaultJsonConverter();
            config.UseDefaultRoutes();

            WorkerPoolExample.StartPool(
                false,
                new TimeSpan(0, 0, 0, 15),
                new TimeSpan(0, 0, 0, 10),
                2,
                1,
                appBuilder.GetOnAppDisposing());

            appBuilder
                .UseWindsorScopeMidddleware()
                .UseWebApi(config);
        }
    }
}