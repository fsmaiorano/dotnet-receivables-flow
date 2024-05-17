using Application.Common.Interfaces;
using Infrastructure.Context;
using Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureService
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDataContext>(provider => provider.GetRequiredService<DataContext>());
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (AppDomain.CurrentDomain.FriendlyName.Contains("testhost"))
        {
            services.AddDbContext<DataContext>(options => { options.UseInMemoryDatabase("InMemoryDbForTesting"); });
        }
        else
        {
            services.AddDbContext<DataContext>(options =>
            {
                //TODO - move to environment variables
                options.UseNpgsql(
                    "Host=localhost;Port=5432;Database=receivables-flow;Username=postgres;Password=postgres");
            });
        }
    }
}