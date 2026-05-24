using EventTickets.Application.Extensions;
using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Services;
using EventTickets.Data.DataContext;
using EventTickets.Data.Extensions;
using EventTickets.Data.Seed;
using EventTickets.Middleware.Extensions;
using EventTickets.Middleware.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application + Data layers
builder.Services.AddApplicationLayer();
builder.Services.AddDataLayer("EventTicketsDb");

// Middleware services
builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Routing
app.UseRouting();

// Your global exception handler (handles thrown exceptions)
app.UseGlobalExceptionHandling();

// Map controllers
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.Run();
