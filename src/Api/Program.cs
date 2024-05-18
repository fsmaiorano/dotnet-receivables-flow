using Api;
using Application;
using Application.UseCases.Assignor.Commands.CreateAssignor;
using Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddApi();

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

var mediator = app.Services.GetService(typeof(ISender)) as ISender ??
               throw new NullReferenceException("Mediator is null");

//Routes

app.MapPost("/integrations/assignor", async (CreateAssignorCommand command) => await mediator.Send(command))
    .WithName("CreateAssignor")
    .WithOpenApi();

app.Run();