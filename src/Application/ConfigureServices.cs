using System.Reflection;
using Application.Common.Behaviours;
using Application.Common.Workers;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

using Common.Interfaces;
using Common.Queue;

public static class ConfigureServices
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            // cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
        });

        services.AddSingleton<RConsumer>();
        services.AddHostedService<Worker>();
        services.AddTransient<ISendEmail>();
    }
}
