using HomeServices.Domain.Entities;
using HomeServices.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServices.Infrastructure.Data.Configurations;

public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(80);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(80).IsUnicode(false);
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.IconClass).IsRequired().HasMaxLength(60).IsUnicode(false);
        builder.Property(c => c.AccentColor).IsRequired().HasMaxLength(7).IsUnicode(false);
        builder.Property(c => c.ImagePath).HasMaxLength(300);

        builder.HasIndex(c => c.Slug).IsUnique();

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(ReferenceData.Categories);
    }
}

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Slug).IsRequired().HasMaxLength(100).IsUnicode(false);
        builder.Property(s => s.Description).HasMaxLength(500);

        builder.HasIndex(s => s.Slug).IsUnique();

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Services)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(ReferenceData.Services);
    }
}

public class ProviderServiceConfiguration : IEntityTypeConfiguration<ProviderService>
{
    public void Configure(EntityTypeBuilder<ProviderService> builder)
    {
        builder.Property(s => s.Notes).HasMaxLength(300);

        builder.HasIndex(s => new { s.ProviderProfileId, s.ServiceId }).IsUnique();
        builder.HasIndex(s => s.Price);

        builder.HasOne(s => s.Service)
            .WithMany(s => s.ProviderServices)
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProviderPortfolioImageConfiguration : IEntityTypeConfiguration<ProviderPortfolioImage>
{
    public void Configure(EntityTypeBuilder<ProviderPortfolioImage> builder)
    {
        builder.Property(i => i.ImagePath).IsRequired().HasMaxLength(300);
        builder.Property(i => i.Caption).HasMaxLength(150);
    }
}
