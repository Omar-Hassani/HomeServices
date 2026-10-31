using System.Security.Claims;
using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Account;
using HomeServices.Application.Interfaces;
using HomeServices.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? type)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new RegisterViewModel
        {
            AccountType = string.Equals(type, "provider", StringComparison.OrdinalIgnoreCase)
                ? AccountType.Provider
                : AccountType.Customer
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await accountService.RegisterAsync(
            new RegisterUserDto(model.FullName, model.Email, model.PhoneNumber, model.Password, model.AccountType),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result);
            return View(model);
        }

        TempData["Success"] = model.AccountType == AccountType.Provider
            ? "تم إنشاء حسابك بنجاح! الخطوة التالية: إكمال ملفك الاحترافي ليُراجع من إدارة المنصة."
            : $"أهلاً بك يا {model.FullName.Split(' ')[0]}! تم إنشاء حسابك بنجاح.";

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var status = await accountService.LoginAsync(model.Email, model.Password, model.RememberMe);
        switch (status)
        {
            case LoginStatus.Succeeded:
                TempData["Success"] = "تم تسجيل الدخول بنجاح.";
                return RedirectToLocal(model.ReturnUrl);

            case LoginStatus.LockedOut:
                ModelState.AddModelError(string.Empty, "تم إيقاف الدخول مؤقتاً بسبب محاولات متكررة خاطئة. حاول مرة أخرى بعد 10 دقائق.");
                break;

            case LoginStatus.Inactive:
                ModelState.AddModelError(string.Empty, "هذا الحساب موقوف. يرجى التواصل مع إدارة المنصة.");
                break;

            default:
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                break;
        }

        model.Password = string.Empty;
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await accountService.LogoutAsync();
        TempData["Success"] = "تم تسجيل الخروج. نراك قريباً!";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var model = await BuildProfileViewModelAsync();
        return model is null ? Challenge() : View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await accountService.UpdateProfileAsync(CurrentUserId, new UpdateProfileDto(model.FullName, model.PhoneNumber));
            if (result.Succeeded)
            {
                TempData["Success"] = "تم حفظ بياناتك بنجاح.";
                return RedirectToAction(nameof(Profile));
            }

            AddErrors(result);
        }

        // إعادة تعبئة حقول العرض فقط مع الإبقاء على ما أدخله المستخدم
        var stored = await BuildProfileViewModelAsync();
        if (stored is null)
        {
            return Challenge();
        }

        stored.FullName = model.FullName;
        stored.PhoneNumber = model.PhoneNumber;
        return View(stored);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(new ChangePasswordViewModel());
        }

        var result = await accountService.ChangePasswordAsync(CurrentUserId, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            AddErrors(result);
            return View(new ChangePasswordViewModel());
        }

        TempData["Success"] = "تم تغيير كلمة المرور بنجاح.";
        return RedirectToAction(nameof(Profile));
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    private async Task<ProfileViewModel?> BuildProfileViewModelAsync()
    {
        var profile = await accountService.GetProfileAsync(CurrentUserId);
        return profile is null
            ? null
            : new ProfileViewModel
            {
                FullName = profile.FullName,
                PhoneNumber = profile.PhoneNumber ?? string.Empty,
                Email = profile.Email,
                ProfileImagePath = profile.ProfileImagePath,
                CreatedAt = profile.CreatedAt,
                Roles = profile.Roles
            };
    }

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction("Index", "Home");

    private void AddErrors(Result result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
    }
}
