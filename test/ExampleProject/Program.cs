using System.Reflection;
using ExampleProject.Feature;
using FlowPipe;
using FlowPipe.Extensions;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddFlowPipe(flowPipeConfig => { flowPipeConfig.AddAssembly(Assembly.GetExecutingAssembly()); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapPost("/weatherforecast", async (
        [FromBody] PingRequest request,
        [FromServices] IMessageDispatcher dispatcher) =>
    {
        var response = await dispatcher.SendAsync(request);
        return response;
    })
    .WithName("Ping Service");

app.Run();