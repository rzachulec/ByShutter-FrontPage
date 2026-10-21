using ByShutterBackend.Domain.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByShutterBackend.Infrastructure.Persistance.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.ToTable("Albums");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(32);
        builder.HasIndex(a => a.Name)
            .IsUnique();

        builder.Property(a => a.Description)
            .IsRequired(false)
            .HasMaxLength(256);
    }
}