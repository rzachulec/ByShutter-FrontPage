using ByShutterBackend.Domain.HomeConfig;
using ByShutterBackend.Infrastructure.Common.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByShutterBackend.Infrastructure.Persistance.Configurations;

/// <summary>
/// Config class for entities from HomePage DB
/// </summary>
internal sealed class HomeProfileConfiguration : IEntityTypeConfiguration<HomeProfile>
{
    public void Configure(EntityTypeBuilder<HomeProfile> builder)
    {
        builder.ToTable("HomeProfiles");
        builder.HasKey(hp => hp.Id);
        builder.Property(hp => hp.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(hp => hp.HeroMessage)
            .HasMaxLength(HomeConfigConsts.MaxTitleLength);
        builder.Property(hp => hp.HeroMessage)
            .IsRequired();
        builder.Property(hp => hp.HeroMessage)
            .HasDefaultValue("Welcome to my page!");

        builder.Property(hp => hp.Greetings)
            .HasMaxLength(HomeConfigConsts.MaxTitleLength);
        builder.Property(hp => hp.Greetings)
            .HasDefaultValue("Hello!");

        builder.Property(hp => hp.Quote)
            .HasMaxLength(HomeConfigConsts.MaxQuoteLength);
        builder.Property(hp => hp.Quote)
            .HasDefaultValue("This is a quote!");

        builder.Property(hp => hp.AboutMe)
            .HasMaxLength(HomeConfigConsts.MaxParagraphLength);
        builder.Property(hp => hp.AboutMe)
            .HasDefaultValue("This is a short description about me!");

        builder.Property(hp => hp.HeroImageId)
            .IsRequired();
        builder.HasOne(hp => hp.HeroImage)
            .WithMany();

        builder.Property(hp => hp.ProfileImageId)
            .IsRequired();
        builder.HasOne(hp => hp.ProfileImage)
            .WithMany();

        builder
            .HasMany(hp => hp.PortfolioTiles)
            .WithOne(pt => pt.HomeProfile)
            .HasForeignKey(pt => pt.HomeProfileId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}