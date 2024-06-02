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

        // services.AddSingleton<RConsumer>();

        services.AddSingleton<PayableQueueConsumer>();
        services.AddSingleton<PayableDeadQueueConsumer>();
        services.AddSingleton<PayableRetryQueueConsumer1>();
        services.AddSingleton<PayableRetryQueueConsumer2>();
        services.AddSingleton<PayableRetryQueueConsumer3>();
        services.AddSingleton<PayableRetryQueueConsumer4>();

        services.AddHostedService<PayableBatchWorker>();
    }
}
