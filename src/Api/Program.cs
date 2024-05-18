using Api;
using Application;
using Application.UseCases.Assignor.Commands.CreateAssignor;
using Infrastructure;
using MediatR;
using System.Text.Json.Serialization;
using Api.Handlers;
using Application.UseCases.Assignor.Commands.DeleteAssignor;
using Application.UseCases.Assignor.Commands.UpdateAssignor;
using Application.UseCases.Assignor.Queries;
using Application.UseCases.Payable.Commands.CreatePayable;
using Application.UseCases.Payable.Queries;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddApi();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Assignors
app.MapPost("/integrations/assignor",
        async (CreateAssignorCommand command, ISender sender) => await sender.Send(command))
    .WithName("CreateAssignor")
    .WithOpenApi();

app.MapGet("/integrations/assignor/{assignorId}",
        async (string assignorId, ISender sender) =>
            await sender.Send(new GetAssignorByIdQuery { Id = Guid.Parse(assignorId) }))
    .WithName("GetAssignorById")
    .WithOpenApi();

app.MapPut("/integrations/assignor/{assignorId}",
        async (UpdateAssignorCommand command, ISender sender, string assignorId) =>
        {
            command.Id = Guid.Parse(assignorId);
            return await sender.Send(command);
        })
    .WithName("UpdateAssignor")
    .WithOpenApi();

app.MapDelete("/integrations/assignor/{assignorId}",
        async (string assignorId, ISender sender) =>
            await sender.Send(new DeleteAssignorCommand { Id = Guid.Parse(assignorId) }))
    .WithName("DeleteAssignor")
    .WithOpenApi();


// Payables
app.MapPost("/integrations/assignor/{assignorId}/payable",
        async (CreatePayableCommand command, ISender sender, string assignorId) =>
        {
            command.AssignorId = Guid.Parse(assignorId);
            return await sender.Send(command);
        })
    .WithName("CreatePayment")
    .WithOpenApi();

app.MapGet("/integrations/payable/{payableId}",
        async (string payableId, ISender sender) =>
            await sender.Send(new GetPayableByIdQuery() { Id = Guid.Parse(payableId) }))
    .WithName("GetPayableById")
    .WithOpenApi();

app.Run();