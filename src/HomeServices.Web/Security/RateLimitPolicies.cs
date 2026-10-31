using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace HomeServices.Web.Security;

/// <summary>
/// سياسات تحديد معدل الطلبات للعمليات الحساسة أو المكلفة. التجاوز يعيد 429 وتعرضه صفحة الحالة العربية.
/// </summary>
public static class RateLimitPolicies
{
    /// <summary>تسجيل الدخول وإنشاء الحساب: 10 محاولات في الدقيقة لكل عنوان IP (حماية من التخمين والتسجيل الآلي).</summary>
    public const string Auth = "auth";

    /// <summary>رفع الصور: 20 ملفاً كل 10 دقائق لكل مستخدم.</summary>
    public const string Uploads = "uploads";

    /// <summary>إرسال الطلبات والتقييمات: 15 كل 10 دقائق لكل مستخدم.</summary>
    public const string Submissions = "submissions";

    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(Auth, context => RateLimitPartition.GetFixedWindowLimiter(
                ClientIp(context),
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));

            options.AddPolicy(Uploads, context => RateLimitPartition.GetFixedWindowLimiter(
                UserOrIp(context),
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(10) }));

            options.AddPolicy(Submissions, context => RateLimitPartition.GetFixedWindowLimiter(
                UserOrIp(context),
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 15, Window = TimeSpan.FromMinutes(10) }));
        });

    private static string ClientIp(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private static string UserOrIp(HttpContext context) =>
        context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ClientIp(context);
}
