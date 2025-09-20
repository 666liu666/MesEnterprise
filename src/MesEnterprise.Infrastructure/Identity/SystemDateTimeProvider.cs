using MesEnterprise.Application.Common.Interfaces;

namespace MesEnterprise.Infrastructure.Identity;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
