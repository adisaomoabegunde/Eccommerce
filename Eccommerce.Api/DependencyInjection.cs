

using Eccommerce.Infrastructure;
using Eccormmerce.Application;

namespace Eccommerce.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI();

            return services;
        }
    }
}
