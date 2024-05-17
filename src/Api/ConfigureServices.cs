using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        services.Configure<JsonOptions>(options =>
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddHttpContextAccessor();

        return services;
    }
}