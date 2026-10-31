using HomeServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServices.Infrastructure.Data.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.Property(r => r.OrderNumber).IsRequired().HasMaxLength(20).IsUnicode(false);
        builder.Property(r => r.AddressDetails).IsRequired().HasMaxLength(300);
        builder.Property(r => r.Description).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.CancellationReason).HasMaxLength(500);

        builder.HasIndex(r => r.OrderNumber).IsUnique();

        // منع تطبيق انتقالين متزامنين على نفس الطلب (مثلاً إلغاء العميل وقبول مقدم الخدمة في نفس اللحظة)
        builder.Property(r => r.Status).IsConcurrencyToken();
        builder.HasIndex(r => new { r.CustomerId, r.Status });
        builder.HasIndex(r => new { r.ProviderProfileId, r.Status });

        // Restrict في كل العلاقات: سجل الطلبات لا يُحذف بالتتابع
        builder.HasOne(r => r.Customer)
            .WithMany(u => u.CustomerRequests)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Provider)
            .WithMany(p => p.Requests)
            .HasForeignKey(r => r.ProviderProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Service)
            .WithMany()
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Area)
            .WithMany()
            .HasForeignKey(r => r.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.StatusHistory)
            .WithOne(h => h.ServiceRequest)
            .HasForeignKey(h => h.ServiceRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.Property(h => h.Note).HasMaxLength(500);

        builder.HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.Comment).HasMaxLength(1000);
        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] BETWEEN 1 AND 5"));

        builder.HasIndex(r => r.ServiceRequestId).IsUnique();
        builder.HasIndex(r => new { r.ProviderProfileId, r.IsHidden });

        builder.HasOne(r => r.ServiceRequest)
            .WithOne(s => s.Review)
            .HasForeignKey<Review>(r => r.ServiceRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Provider)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProviderProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.Title).IsRequired().HasMaxLength(150);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(500);
        builder.Property(n => n.Link).HasMaxLength(300);

        builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });
    }
}
