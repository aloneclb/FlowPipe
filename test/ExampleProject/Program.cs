using ExampleProject.Features;
using FlowPipe;
using FlowPipe.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFlowPipe(c => { c.AddAssembly(typeof(Program).Assembly); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/ping", async ([FromServices] IMessageDispatcher messageDispatcher) =>
    {
        var response = await messageDispatcher.SendAsync(new PingRequest()
        {
            UserId = 1
        });

        Console.WriteLine($"Endpoint receive message {response.ServerEndpoint}");

        return "pong";
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}