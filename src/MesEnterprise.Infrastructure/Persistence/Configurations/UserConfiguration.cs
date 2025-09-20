using MesEnterprise.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MesEnterprise.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("SYS_USERS");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.UserName).HasMaxLength(128).IsRequired();
        builder.Property(u => u.DisplayName).HasMaxLength(128).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();

        builder.HasMany(u => u.Roles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        builder.HasMany(u => u.Claims)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);
    }
}
