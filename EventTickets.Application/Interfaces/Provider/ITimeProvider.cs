namespace EventTickets.Application.Interfaces.Provider
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
