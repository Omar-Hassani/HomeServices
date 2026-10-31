using HomeServices.Application.Interfaces;
using HomeServices.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HomeServices.Web.Areas.Provider.Controllers;

public class ReviewsController(IReviewService reviewService) : ProviderAreaController
{
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        var reviews = await reviewService.GetOwnReviewsAsync(CurrentUserId, page, cancellationToken);
        if (reviews is null)
        {
            return NotFound();
        }

        return View(new ReviewsPageViewModel
        {
            Reviews = reviews,
            Pagination = new PaginationViewModel
            {
                Page = reviews.Reviews.Page,
                TotalPages = reviews.Reviews.TotalPages,
                PageUrl = p => Url.Action(nameof(Index), new { page = p > 1 ? p : (int?)null })!
            }
        });
    }
}
