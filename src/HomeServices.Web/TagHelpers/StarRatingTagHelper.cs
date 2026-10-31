using System.Globalization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HomeServices.Web.TagHelpers;

/// <summary>
/// يعرض تقييماً من 5 نجوم مع أنصاف النجوم: &lt;star-rating value="4.5" /&gt;
/// </summary>
[HtmlTargetElement("star-rating", TagStructure = TagStructure.WithoutEndTag)]
public class StarRatingTagHelper : TagHelper
{
    public decimal Value { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var rating = Math.Clamp(Value, 0, 5);
        var label = rating.ToString("0.0", CultureInfo.InvariantCulture);

        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "star-rating");
        output.Attributes.SetAttribute("role", "img");
        output.Attributes.SetAttribute("aria-label", $"التقييم {label} من 5");

        for (var i = 1; i <= 5; i++)
        {
            var icon = rating >= i ? "fa-solid fa-star"
                : rating >= i - 0.5m ? "fa-solid fa-star-half-stroke"
                : "fa-regular fa-star";

            output.Content.AppendHtml($"<i class=\"{icon}\" aria-hidden=\"true\"></i>");
        }
    }
}
