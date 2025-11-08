namespace MesEnterprise.Infrastructure.Identity;

public class LdapSettings
{
    public string Server { get; init; } = string.Empty;
    public int Port { get; init; } = 389;
    public string BaseDn { get; init; } = string.Empty;
    public string? BindUserFormat { get; init; } = "{0}";
}
