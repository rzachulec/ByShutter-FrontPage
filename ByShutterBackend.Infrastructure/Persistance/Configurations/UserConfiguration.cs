using ByShutterBackend.Domain.ClientAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByShutterBackend.Infrastructure.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.HasIndex(user => user.Username).IsUnique();
        builder.Property(user => user.Username).HasMaxLength(100).IsRequired();
        builder.Property(user => user.PasswordHash).IsRequired();
        builder.Property(user => user.PasswordSalt).IsRequired();
    }
}
