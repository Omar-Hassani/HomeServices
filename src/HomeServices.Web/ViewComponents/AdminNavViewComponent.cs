using HomeServices.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.ViewComponents;

/// <summary>
/// القائمة الجانبية للوحة الإدارة مع عدد الملفات بانتظار المراجعة.
/// </summary>
public class AdminNavViewComponent(IProviderVerificationService verificationService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await verificationService.GetPendingCountAsync(HttpContext.RequestAborted));
}
