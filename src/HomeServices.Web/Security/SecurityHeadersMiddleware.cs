namespace HomeServices.Web.Security;

/// <summary>
/// ترويسات الحماية لكل الاستجابات. كل الملفات (الخطوط، المكتبات، الصور) مستضافة محلياً،
/// لذا تسمح سياسة CSP بالمصدر الذاتي فقط.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    // البصمة الوحيدة المسموحة لسكربت مضمّن: سطر إضافة الصنف js في _Layout قبل رسم الصفحة.
    // عند تعديل ذلك السطر يجب تحديث البصمة (sha256 بترميز base64 لنص السكربت).
    private const string LayoutInlineScriptHash = "'sha256-/x7W7R75k8Roq0WaVRQX9blP4OufE5xbAdzklGxsgpw='";

    private static readonly string ContentSecurityPolicy = string.Join("; ",
        "default-src 'self'",
        $"script-src 'self' {LayoutInlineScriptHash}",
        // style-src يسمح بالمضمّن لأن ألوان الفئات وعروض أشرطة التقدم تُمرَّر كمتغيرات CSS في سمة style
        "style-src 'self' 'unsafe-inline'",
        // data: لأيقونات Bootstrap المضمّنة (أسهم القوائم ومربعات الاختيار)
        "img-src 'self' data:",
        "font-src 'self'",
        "connect-src 'self'",
        "form-action 'self'",
        "frame-ancestors 'none'",
        "base-uri 'self'",
        "object-src 'none'");

    public Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers.ContentSecurityPolicy = ContentSecurityPolicy;
        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
        headers["Cross-Origin-Opener-Policy"] = "same-origin";
        return next(context);
    }
}
