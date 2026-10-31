using HomeServices.Application.DTOs.Admin;
using HomeServices.Application.Interfaces;
using HomeServices.Web.Areas.Admin.Models;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Admin.Controllers;

public class ReviewsController(IReviewModerationService moderationService) : AdminAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(ReviewVisibilityFilter visibility = ReviewVisibilityFilter.All, byte? rating = null, int page = 1, CancellationToken cancellationToken = default)
    {
        var reviews = await moderationService.GetReviewsAsync(visibility, rating, page, cancellationToken);
        var visibilityValue = visibility == ReviewVisibilityFilter.All ? (ReviewVisibilityFilter?)null : visibility;

        return View(new ReviewModerationPageViewModel
        {
            Reviews = reviews,
            Visibility = visibility,
            Rating = rating,
            Pagination = new PaginationViewModel
            {
                Page = reviews.Page,
                TotalPages = reviews.TotalPages,
                PageUrl = p => Url.Action(nameof(Index), new { visibility = visibilityValue, rating, page = p > 1 ? p : (int?)null })!
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> SetHidden(int id, bool isHidden, string? returnUrl, CancellationToken cancellationToken)
    {
        var result = await moderationService.SetHiddenAsync(id, isHidden, cancellationToken);
        Notify(result, isHidden ? "تم إخفاء التقييم وتحديث متوسط تقييم مقدم الخدمة." : "تم إظهار التقييم وتحديث متوسط تقييم مقدم الخدمة.");
        return RedirectBack(returnUrl, nameof(Index));
    }
}
