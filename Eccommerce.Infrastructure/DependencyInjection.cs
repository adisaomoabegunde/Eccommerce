using Eccommerce.Infrastructure.Auth;
using Eccommerce.Infrastructure.Persistence;
using Eccommerce.Infrastructure.Repositories;
using Eccommerce.Infrastructure.Services;
using Eccormmerce.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccommerce.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext> (options => options.UseNpgsql(configuration.GetConnectionString("Default"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
