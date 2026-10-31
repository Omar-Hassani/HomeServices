using HomeServices.Domain.Entities;
using HomeServices.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServices.Infrastructure.Data.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(60);
        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasMany(c => c.Areas)
            .WithOne(a => a.City)
            .HasForeignKey(a => a.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(ReferenceData.Cities);
    }
}

public class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.Property(a => a.Name).IsRequired().HasMaxLength(80);
        builder.HasIndex(a => new { a.CityId, a.Name }).IsUnique();

        builder.HasData(ReferenceData.Areas);
    }
}

public class ProviderServiceAreaConfiguration : IEntityTypeConfiguration<ProviderServiceArea>
{
    public void Configure(EntityTypeBuilder<ProviderServiceArea> builder)
    {
        builder.HasKey(a => new { a.ProviderProfileId, a.AreaId });
        builder.HasIndex(a => a.AreaId);

        builder.HasOne(a => a.Area)
            .WithMany()
            .HasForeignKey(a => a.AreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.Property(a => a.Label).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Street).IsRequired().HasMaxLength(150);
        builder.Property(a => a.Building).HasMaxLength(100);
        builder.Property(a => a.Details).HasMaxLength(300);

        builder.HasOne(a => a.Area)
            .WithMany()
            .HasForeignKey(a => a.AreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
