namespace MesEnterprise.Domain.Identity;

public class UserClaim
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public required string Type { get; set; }
    public required string Value { get; set; }
}
