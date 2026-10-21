using ByShutterBackend.Domain.Portfolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByShutterBackend.Infrastructure.Persistance.Configurations;

public class AlbumPhotoConfiguration : IEntityTypeConfiguration<AlbumPhoto>
{
    public void Configure(EntityTypeBuilder<AlbumPhoto> builder)
    {
        builder.ToTable("AlbumPhotos");
        builder.HasKey(ap => ap.Id);
        
        builder.Property(ap => ap.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(ap => ap.AlbumId)
            .IsRequired();
        
        builder.Property(ap => ap.PhotoId)
            .IsRequired();
        
        builder.HasOne(ap => ap.Album)
            .WithMany(a => a.Photos)
            .HasForeignKey(ap => ap.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ap => ap.Photo)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}