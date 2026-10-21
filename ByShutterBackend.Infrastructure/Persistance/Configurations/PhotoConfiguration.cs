using ByShutterBackend.Domain.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByShutterBackend.Infrastructure.Persistance.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable("Photos");

        builder.HasKey(p => p.Id);
        builder
            .Property(p => p.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(p => p.FileName)
            .IsRequired()
            .HasMaxLength(32);
        builder.HasIndex(p => p.FileName)
            .IsUnique();
        
        builder.Property(p => p.DisplayName)
            .IsRequired(false)
            .HasMaxLength(32);
        
        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasMaxLength(256);
        
        builder.Property(p => p.UploadedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");
        
        builder.Property(p => p.ServerPath)
            .IsRequired()
            .HasMaxLength(256)
            .IsUnicode(false);
    }
}