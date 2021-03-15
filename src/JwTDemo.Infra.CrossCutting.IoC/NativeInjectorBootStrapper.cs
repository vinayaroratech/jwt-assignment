using JwtDemoApp.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace JwTDemo.Infra.CrossCutting.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            DependencyInjection.AddApplication(services);

            return services;
        }
    }
}