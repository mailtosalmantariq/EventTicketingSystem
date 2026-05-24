using EventTickets.Application.Interfaces.Repos;
using EventTickets.Application.Interfaces.Repositories;
using EventTickets.Data.DataContext;
using EventTickets.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventTickets.Data.Extensions;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string databaseName)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={databaseName}.db"));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();

        return services;
    }
}
