using Microsoft.AspNetCore.Identity;

namespace HomeServices.Infrastructure.Identity;

/// <summary>
/// تعريب رسائل أخطاء ASP.NET Core Identity.
/// </summary>
public sealed class ArabicIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError() =>
        Error(nameof(DefaultError), "حدث خطأ غير متوقع، يرجى المحاولة مرة أخرى.");

    public override IdentityError ConcurrencyFailure() =>
        Error(nameof(ConcurrencyFailure), "تم تعديل البيانات من جهة أخرى، يرجى إعادة المحاولة.");

    public override IdentityError PasswordMismatch() =>
        Error(nameof(PasswordMismatch), "كلمة المرور غير صحيحة.");

    public override IdentityError InvalidToken() =>
        Error(nameof(InvalidToken), "الرابط غير صالح أو منتهي الصلاحية.");

    public override IdentityError LoginAlreadyAssociated() =>
        Error(nameof(LoginAlreadyAssociated), "هذا الحساب مرتبط بمستخدم آخر.");

    public override IdentityError InvalidUserName(string? userName) =>
        Error(nameof(InvalidUserName), "اسم المستخدم يحتوي على أحرف غير مسموحة.");

    public override IdentityError InvalidEmail(string? email) =>
        Error(nameof(InvalidEmail), "البريد الإلكتروني غير صالح.");

    public override IdentityError DuplicateUserName(string userName) =>
        Error(nameof(DuplicateUserName), "هذا البريد الإلكتروني مسجّل مسبقاً.");

    public override IdentityError DuplicateEmail(string email) =>
        Error(nameof(DuplicateEmail), "هذا البريد الإلكتروني مسجّل مسبقاً.");

    public override IdentityError InvalidRoleName(string? role) =>
        Error(nameof(InvalidRoleName), "اسم الدور غير صالح.");

    public override IdentityError DuplicateRoleName(string role) =>
        Error(nameof(DuplicateRoleName), "اسم الدور مستخدم مسبقاً.");

    public override IdentityError UserAlreadyHasPassword() =>
        Error(nameof(UserAlreadyHasPassword), "لدى المستخدم كلمة مرور مسبقاً.");

    public override IdentityError UserLockoutNotEnabled() =>
        Error(nameof(UserLockoutNotEnabled), "خاصية القفل غير مفعّلة لهذا المستخدم.");

    public override IdentityError UserAlreadyInRole(string role) =>
        Error(nameof(UserAlreadyInRole), "المستخدم يملك هذا الدور مسبقاً.");

    public override IdentityError UserNotInRole(string role) =>
        Error(nameof(UserNotInRole), "المستخدم لا يملك هذا الدور.");

    public override IdentityError PasswordTooShort(int length) =>
        Error(nameof(PasswordTooShort), $"يجب ألا تقل كلمة المرور عن {length} أحرف.");

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        Error(nameof(PasswordRequiresUniqueChars), $"يجب أن تحتوي كلمة المرور على {uniqueChars} أحرف مختلفة على الأقل.");

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        Error(nameof(PasswordRequiresNonAlphanumeric), "يجب أن تحتوي كلمة المرور على رمز واحد على الأقل مثل @ أو #.");

    public override IdentityError PasswordRequiresDigit() =>
        Error(nameof(PasswordRequiresDigit), "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل.");

    public override IdentityError PasswordRequiresLower() =>
        Error(nameof(PasswordRequiresLower), "يجب أن تحتوي كلمة المرور على حرف إنجليزي صغير واحد على الأقل.");

    public override IdentityError PasswordRequiresUpper() =>
        Error(nameof(PasswordRequiresUpper), "يجب أن تحتوي كلمة المرور على حرف إنجليزي كبير واحد على الأقل.");

    public override IdentityError RecoveryCodeRedemptionFailed() =>
        Error(nameof(RecoveryCodeRedemptionFailed), "رمز الاسترداد غير صحيح.");

    private static IdentityError Error(string code, string description) => new() { Code = code, Description = description };
}
