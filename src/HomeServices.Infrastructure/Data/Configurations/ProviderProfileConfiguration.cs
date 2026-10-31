using HomeServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServices.Infrastructure.Data.Configurations;

public class ProviderProfileConfiguration : IEntityTypeConfiguration<ProviderProfile>
{
    public void Configure(EntityTypeBuilder<ProviderProfile> builder)
    {
        builder.Property(p => p.Headline).IsRequired().HasMaxLength(120);
        builder.Property(p => p.Bio).HasMaxLength(2000);
        builder.Property(p => p.VerificationNote).HasMaxLength(500);
        builder.Property(p => p.AverageRating).HasPrecision(3, 2);

        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasIndex(p => new { p.VerificationStatus, p.AverageRating });

        builder.HasOne(p => p.User)
            .WithOne(u => u.ProviderProfile)
            .HasForeignKey<ProviderProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.City)
            .WithMany()
            .HasForeignKey(p => p.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Services)
            .WithOne(s => s.Provider)
            .HasForeignKey(s => s.ProviderProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ServiceAreas)
            .WithOne(a => a.Provider)
            .HasForeignKey(a => a.ProviderProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.PortfolioImages)
            .WithOne(i => i.Provider)
            .HasForeignKey(i => i.ProviderProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
