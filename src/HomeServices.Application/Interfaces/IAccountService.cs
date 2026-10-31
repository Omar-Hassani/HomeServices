using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Account;
using HomeServices.Application.DTOs.Files;

namespace HomeServices.Application.Interfaces;

public interface IAccountService
{
    /// <summary>
    /// ينشئ الحساب ويسند الدور المناسب، وينشئ ملف مقدم الخدمة بحالة "قيد المراجعة" إن لزم، ثم يسجّل الدخول.
    /// </summary>
    Task<Result> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);

    Task<LoginStatus> LoginAsync(string email, string password, bool rememberMe);

    Task LogoutAsync();

    Task<UserProfileDto?> GetProfileAsync(string userId);

    Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto);

    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    Task<Result> UpdateProfileImageAsync(string userId, FileUpload image, CancellationToken cancellationToken = default);

    Task<Result> RemoveProfileImageAsync(string userId, CancellationToken cancellationToken = default);
}
