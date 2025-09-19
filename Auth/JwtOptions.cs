namespace MesEnterprise.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = "MesEnterprise";
    public string Audience { get; set; } = "MesEnterprise";
    public string Key { get; set; } = string.Empty;
    public int ExpiryHours { get; set; } = 8;
}
