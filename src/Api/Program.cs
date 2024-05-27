using Api;
using Application;
using Application.UseCases.Assignor.Commands.CreateAssignor;
using Infrastructure;
using MediatR;
using System.Text.Json.Serialization;
using Api.Handlers;
using Application.Common.Security.Jwt;
using Application.UseCases.Account.Commands.AuthenticateAccount;
using Application.UseCases.Assignor.Commands.DeleteAssignor;
using Application.UseCases.Assignor.Commands.UpdateAssignor;
using Application.UseCases.Assignor.Queries;
using Application.UseCases.Payable.Commands.CreatePayable;
using Application.UseCases.Payable.Commands.CreatePayableBatch;
using Application.UseCases.Payable.Queries;
using Identity;
using Identity.Extensions;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentity();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddApi();

builder.Services.AddSwaggerGenJwt("v1",
    new OpenApiInfo
    {
        Title = "API",
        Description = "",
        Version = "v1",
    });

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddCors();

var tokenConfigurations = new TokenConfigurations();
new ConfigureFromConfigurationOptions<TokenConfigurations>(
        builder.Configuration.GetSection("TokenConfigurations"))
    .Configure(tokenConfigurations);

builder.Services.AddJwtSecurity(tokenConfigurations);
builder.Services.AddScoped<IdentityInitializer>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IdentityInitializer>().Initialize();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
    app.ApplyIdentityMigrations();
}

app.UseHttpsRedirection();

// Authentication
app.MapPost("/integrations/auth", async (AuthenticateAccountCommand command, ISender sender) =>
        await sender.Send(command))
    .WithName("Login")
    .AllowAnonymous()
    .WithOpenApi();

// Batch
app.MapPost("/integrations/payable/batch", async (CreatePayableBatchCommand command, ISender sender) =>
        await sender.Send(command))
    .WithName("CreatePayableBatch")
    .RequireAuthorization()
    .WithOpenApi();

// Assignors
app.MapPost("/integrations/assignor",
        async (CreateAssignorCommand command, ISender sender) => await sender.Send(command))
    .WithName("CreateAssignor")
    .RequireAuthorization()
    .WithOpenApi();

app.MapGet("/integrations/assignor/{assignorId}", async (string assignorId, ISender sender) =>
        await sender.Send(new GetAssignorByIdQuery { Id = Guid.Parse(assignorId) }))
    .WithName("GetAssignorById")
    .RequireAuthorization()
    .WithOpenApi();

app.MapPut("/integrations/assignor/{assignorId}", [Authorize("Bearer")]
        async (UpdateAssignorCommand command, ISender sender, string assignorId) =>
        {
            command.Id = Guid.Parse(assignorId);
            return await sender.Send(command);
        })
    .WithName("UpdateAssignor")
    .RequireAuthorization()
    .WithOpenApi();

app.MapDelete("/integrations/assignor/{assignorId}", async (string assignorId, ISender sender) =>
        await sender.Send(new DeleteAssignorCommand { Id = Guid.Parse(assignorId) }))
    .WithName("DeleteAssignor")
    .RequireAuthorization()
    .WithOpenApi();

// Payables
app.MapPost("/integrations/assignor/{assignorId}/payable", [Authorize("Bearer")]
        async (CreatePayableCommand command, ISender sender, string assignorId) =>
        {
            command.AssignorId = Guid.Parse(assignorId);
            return await sender.Send(command);
        })
    .WithName("CreatePayment")
    .RequireAuthorization()
    .WithOpenApi();

app.MapGet("/integrations/payable/{payableId}", async (string payableId, ISender sender) =>
        await sender.Send(new GetPayableByIdQuery() { Id = Guid.Parse(payableId) }))
    .WithName("GetPayableById")
    .RequireAuthorization()
    .WithOpenApi();

app.Run();