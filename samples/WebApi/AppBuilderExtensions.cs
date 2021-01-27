using System.Threading;
using Microsoft.Owin.BuilderProperties;
using Owin;

namespace WebApi
{
    public static class AppBuilderExtensions
    {
        public static CancellationToken GetOnAppDisposing(
            this IAppBuilder appBuilder)
        {
            return new AppProperties(appBuilder.Properties).OnAppDisposing;
        }
    }
}