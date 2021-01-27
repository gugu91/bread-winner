using Castle.MicroKernel.Lifestyle.Scoped;
using Owin;

namespace WebApi.IoC.Extensions
{
    public static class AppBuilderWindsorExtensions
    {
        /// <summary>
        /// Call this method to setup a scope shared across Owin and Web Api
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IAppBuilder UseWindsorScopeMidddleware(this IAppBuilder appBuilder)
        {
            return appBuilder.Use(async (env, next) =>
            {
                using (new CallContextLifetimeScope())
                {
                    await next();
                }
            });
        }
    }
}