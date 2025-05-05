namespace Hourly.Core.Interfaces.Services
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
}