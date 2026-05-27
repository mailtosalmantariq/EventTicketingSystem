using EventTickets.Application.Interfaces.Provider;
using EventTickets.Application.Interfaces.Services;
using EventTickets.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventTickets.Application.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<ITimeProvider, SystemTimeProvider>();

            return services;
        }
    }

}
