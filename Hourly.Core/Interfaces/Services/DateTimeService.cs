using Hourly.Core.Interfaces.Services;

namespace Hourly.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}