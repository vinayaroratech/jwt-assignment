using JwtDemoApp.Application.JwtAuth;
using JwtDemoApp.Application.Services;
using JwtDemoApp.Infrastructure.JwtAuth;
using JwtDemoApp.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JwtDemoApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddHostedService<JwtRefreshTokenCache>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}