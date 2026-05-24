using EventTickets.Application.Interfaces.Provider;

namespace EventTickets.Application.Services
{
    public class SystemTimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
