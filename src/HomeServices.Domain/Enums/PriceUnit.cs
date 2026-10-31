using System.ComponentModel.DataAnnotations;

namespace HomeServices.Domain.Enums;

public enum PriceUnit
{
    [Display(Name = "للزيارة")] PerVisit = 0,
    [Display(Name = "للساعة")] PerHour = 1,
    [Display(Name = "سعر ثابت")] Fixed = 2,
    [Display(Name = "للمتر المربع")] PerSquareMeter = 3,
    [Display(Name = "لليوم")] PerDay = 4
}
