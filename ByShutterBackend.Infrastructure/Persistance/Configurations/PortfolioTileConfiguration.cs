using ByShutterBackend.Domain.HomeConfig;
using ByShutterBackend.Infrastructure.Common.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByShutterBackend.Infrastructure.Persistance.Configurations;

internal sealed class PortfolioTileConfiguration : IEntityTypeConfiguration<PortfolioTile>
{
    public void Configure(EntityTypeBuilder<PortfolioTile> builder)
    {
        builder.ToTable("PortfolioTiles");
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Id)
            .ValueGeneratedNever();

        builder.Property(pt => pt.Name)
            .HasMaxLength(HomeConfigConsts.MaxTitleLength)
            .IsRequired();
        
        builder.Property(pt => pt.Description)
            .HasMaxLength(HomeConfigConsts.MaxParagraphLength);

        builder.Property(pt => pt.HomeProfileId)
            .IsRequired(false);
        
        builder.Property(pt => pt.ImageId)
            .IsRequired();

        builder
            .HasOne(pt => pt.Photo)
            .WithMany()
            .HasForeignKey(pt => pt.ImageId);
    }
}