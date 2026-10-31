using HomeServices.Application.Common;
using HomeServices.Application.DTOs.Files;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HomeServices.Web.Extensions;

public static class ControllerExtensions
{
    public static void AddErrors(this ModelStateDictionary modelState, Result result)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(string.Empty, error);
        }
    }

    public static FileUpload ToFileUpload(this IFormFile file) =>
        new(file.OpenReadStream(), file.FileName, file.Length);
}
