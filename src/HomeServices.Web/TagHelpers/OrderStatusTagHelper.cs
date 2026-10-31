using HomeServices.Domain.Enums;
using HomeServices.Web.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HomeServices.Web.TagHelpers;

/// <summary>
/// شارة حالة الطلب بلون وأيقونة موحّدين: &lt;order-status value="@order.Status" /&gt;
/// </summary>
[HtmlTargetElement("order-status", TagStructure = TagStructure.WithoutEndTag)]
public class OrderStatusTagHelper : TagHelper
{
    public OrderStatus Value { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var (css, icon) = Value switch
        {
            OrderStatus.Pending => ("is-pending", "fa-hourglass-half"),
            OrderStatus.Accepted => ("is-accepted", "fa-thumbs-up"),
            OrderStatus.InProgress => ("is-progress", "fa-screwdriver-wrench"),
            OrderStatus.Completed => ("is-completed", "fa-circle-check"),
            OrderStatus.Rejected => ("is-rejected", "fa-circle-xmark"),
            _ => ("is-cancelled", "fa-ban")
        };

        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", $"status-badge {css}");
        output.Content.AppendHtml($"<i class=\"fa-solid {icon}\" aria-hidden=\"true\"></i>");
        output.Content.Append(Value.GetDisplayName());
    }
}
