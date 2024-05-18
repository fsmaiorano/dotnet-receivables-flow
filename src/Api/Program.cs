using Api;
using Application;
using Application.UseCases.Assignor.Commands.CreateAssignor;
using Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
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

//Routes
app.MapPost("/integrations/assignor",
        async (CreateAssignorCommand command, ISender sender) => await sender.Send(command))
    .WithName("CreateAssignor")
    .WithOpenApi();

app.Run();