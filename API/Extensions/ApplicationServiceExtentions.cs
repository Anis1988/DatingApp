using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService,TokenService>();
            services.AddDbContext<DataContext>(option => {
                option.UseSqlite(config.GetConnectionString("DefaultConnection"));
            }) ;
            return services;
        }
    }
}
