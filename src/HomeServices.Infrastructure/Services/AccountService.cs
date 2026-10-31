using HomeServices.Application.Common;
using HomeServices.Application.Constants;
using HomeServices.Application.DTOs.Account;
using HomeServices.Application.DTOs.Files;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Domain.Enums;
using HomeServices.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Services;

internal sealed class AccountService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IFileStorageService fileStorage,
    ILogger<AccountService> logger) : IAccountService
{
    private const string InvalidPhoneMessage = "رقم الهاتف غير صالح. أدخل رقم جوال سوري مثل 0912345678.";

    public async Task<Result> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(dto.AccountType))
        {
            return Result.Failure("نوع الحساب غير صالح.");
        }

        if (!SyrianPhoneNumber.TryNormalize(dto.PhoneNumber, out var phone))
        {
            return Result.Failure(InvalidPhoneMessage);
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email.Trim(),
            Email = dto.Email.Trim(),
            FullName = dto.FullName.Trim(),
            PhoneNumber = phone
        };

        var role = dto.AccountType == AccountType.Provider ? Roles.Provider : Roles.Customer;

        // إنشاء المستخدم والدور وملف مقدم الخدمة في معاملة واحدة: إما أن تنجح كلها أو لا شيء
        var strategy = db.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            var created = await userManager.CreateAsync(user, dto.Password);
            if (!created.Succeeded)
            {
                return Result.Failure(created.Errors.Select(e => e.Description));
            }

            var roleAdded = await userManager.AddToRoleAsync(user, role);
            if (!roleAdded.Succeeded)
            {
                return Result.Failure(roleAdded.Errors.Select(e => e.Description));
            }

            if (dto.AccountType == AccountType.Provider)
            {
                db.ProviderProfiles.Add(new ProviderProfile
                {
                    UserId = user.Id,
                    VerificationStatus = VerificationStatus.Pending,
                    IsAvailable = true
                });
                await db.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        });

        if (!result.Succeeded)
        {
            return result;
        }

        logger.LogInformation("New {Role} account registered: {UserId}", role, user.Id);
        await signInManager.SignInAsync(user, isPersistent: false);
        return Result.Success();
    }

    public async Task<LoginStatus> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await userManager.FindByEmailAsync(email.Trim());
        if (user is null)
        {
            return LoginStatus.InvalidCredentials;
        }

        // التحقق من كلمة المرور أولاً (مع احتساب المحاولات الفاشلة للقفل) قبل كشف حالة الحساب
        var check = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (check.IsLockedOut)
        {
            return LoginStatus.LockedOut;
        }

        if (!check.Succeeded)
        {
            return LoginStatus.InvalidCredentials;
        }

        if (!user.IsActive)
        {
            return LoginStatus.Inactive;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        await signInManager.SignInAsync(user, rememberMe);
        return LoginStatus.Succeeded;
    }

    public Task LogoutAsync() => signInManager.SignOutAsync();

    public async Task<UserProfileDto?> GetProfileAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        return new UserProfileDto(user.FullName, user.Email!, user.PhoneNumber, user.ProfileImagePath, user.CreatedAt, roles.ToList());
    }

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure("الحساب غير موجود.");
        }

        if (!SyrianPhoneNumber.TryNormalize(dto.PhoneNumber, out var phone))
        {
            return Result.Failure(InvalidPhoneMessage);
        }

        user.FullName = dto.FullName.Trim();
        user.PhoneNumber = phone;

        var updated = await userManager.UpdateAsync(user);
        return updated.Succeeded
            ? Result.Success()
            : Result.Failure(updated.Errors.Select(e => e.Description));
    }

    public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure("الحساب غير موجود.");
        }

        var changed = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changed.Succeeded)
        {
            return Result.Failure(changed.Errors.Select(e => e.Description));
        }

        // تغيير كلمة المرور يغيّر SecurityStamp، فنجدد الكوكي للجلسة الحالية
        await signInManager.RefreshSignInAsync(user);
        return Result.Success();
    }

    public async Task<Result> UpdateProfileImageAsync(string userId, FileUpload image, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure("الحساب غير موجود.");
        }

        var saved = await fileStorage.SaveImageAsync(image, StorageFolders.ProfileImages, cancellationToken);
        if (!saved.Succeeded)
        {
            return Result.Failure(saved.Errors);
        }

        var previous = user.ProfileImagePath;
        user.ProfileImagePath = saved.Value;

        var updated = await userManager.UpdateAsync(user);
        if (!updated.Succeeded)
        {
            await fileStorage.DeleteAsync(saved.Value, cancellationToken);
            return Result.Failure(updated.Errors.Select(e => e.Description));
        }

        await fileStorage.DeleteAsync(previous, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveProfileImageAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure("الحساب غير موجود.");
        }

        var previous = user.ProfileImagePath;
        if (previous is null)
        {
            return Result.Success();
        }

        user.ProfileImagePath = null;
        var updated = await userManager.UpdateAsync(user);
        if (!updated.Succeeded)
        {
            return Result.Failure(updated.Errors.Select(e => e.Description));
        }

        await fileStorage.DeleteAsync(previous, cancellationToken);
        return Result.Success();
    }
}
