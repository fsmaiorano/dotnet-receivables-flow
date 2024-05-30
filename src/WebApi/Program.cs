using System.Text.Json.Serialization;
using Application;
using Application.Common.Security.Jwt;
using Application.Common.Workers;
using Infrastructure;
using Identity;
using Identity.Extensions;
using Infrastructure.Database;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WebApi;
using WebApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity();
builder.Services.AddInfrastructure();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddWebApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors();

builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGenJwt("v1",
    new OpenApiInfo { Title = "API", Description = "", Version = "v1", });

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddCors();

var tokenConfigurations = new TokenConfigurations();
new ConfigureFromConfigurationOptions<TokenConfigurations>(
        builder.Configuration.GetSection("TokenConfigurations"))
    .Configure(tokenConfigurations);

builder.Services.AddJwtSecurity(tokenConfigurations);
builder.Services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });

var app = builder.Build();

app.ApplyMigrations();
app.ApplyIdentityMigrations();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IdentityInitializer>().Initialize();
scope.ServiceProvider.GetRequiredService<Seed>().ExecuteAsync(scope.ServiceProvider);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
