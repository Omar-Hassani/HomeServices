using HomeServices.Application.Common;
using HomeServices.Application.Constants;
using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.Interfaces;
using HomeServices.Domain.Entities;
using HomeServices.Infrastructure.Caching;
using HomeServices.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeServices.Infrastructure.Services.Admin;

internal sealed class UserAdminService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    IMemoryCache cache,
    ILogger<UserAdminService> logger) : IUserAdminService
{
    private const int PageSize = 20;

    public async Task<PagedResult<AdminUserRowDto>> GetUsersAsync(string? query, string? role, UserStatusFilter status, int page, CancellationToken cancellationToken = default)
    {
        var users = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            users = users.Where(u => u.FullName.Contains(term) || u.Email!.Contains(term) || (u.PhoneNumber != null && u.PhoneNumber.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(role) && Roles.All.Contains(role))
        {
            users = users.Where(u => db.UserRoles.Any(ur => ur.UserId == u.Id && db.Roles.Any(r => r.Id == ur.RoleId && r.Name == role)));
        }

        users = status switch
        {
            UserStatusFilter.Active => users.Where(u => u.IsActive),
            UserStatusFilter.Inactive => users.Where(u => !u.IsActive),
            _ => users
        };

        var total = await users.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await users
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(u => new AdminUserRowDto(
                u.Id,
                u.FullName,
                u.Email!,
                u.PhoneNumber,
                u.ProfileImagePath,
                (from ur in db.UserRoles
                 join r in db.Roles on ur.RoleId equals r.Id
                 where ur.UserId == u.Id
                 select r.Name!).ToList(),
                u.IsActive,
                u.CreatedAt,
                u.LastLoginAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminUserRowDto>(items, total, page, PageSize);
    }

    public async Task<Result> SetActiveAsync(string adminUserId, string userId, bool isActive, CancellationToken cancellationToken = default)
    {
        if (adminUserId == userId)
        {
            return Result.Failure("لا يمكنك إيقاف حسابك الخاص.");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure("المستخدم غير موجود.");
        }

        if (await userManager.IsInRoleAsync(user, Roles.Admin))
        {
            return Result.Failure("لا يمكن إيقاف حساب مدير من لوحة الإدارة.");
        }

        if (user.IsActive == isActive)
        {
            return Result.Success();
        }

        user.IsActive = isActive;
        var updated = await userManager.UpdateAsync(user);
        if (!updated.Succeeded)
        {
            return Result.Failure(updated.Errors.Select(e => e.Description));
        }

        if (!isActive)
        {
            // تغيير SecurityStamp يُبطل كوكيز الجلسات المفتوحة عند التحقق الدوري التالي
            await userManager.UpdateSecurityStampAsync(user);
        }

        // إيقاف حساب مقدم خدمة يخفيه من الكتالوج، فتتغير أعداد مقدمي الخدمات في الفئات
        CatalogCache.Invalidate(cache);

        logger.LogInformation("User {UserId} active state set to {IsActive} by {AdminId}", userId, isActive, adminUserId);
        return Result.Success();
    }
}
