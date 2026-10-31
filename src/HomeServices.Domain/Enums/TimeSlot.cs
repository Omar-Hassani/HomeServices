using System.ComponentModel.DataAnnotations;

namespace HomeServices.Domain.Enums;

public enum TimeSlot
{
    [Display(Name = "صباحاً (8 - 12)")] Morning = 0,
    [Display(Name = "ظهراً (12 - 4)")] Afternoon = 1,
    [Display(Name = "مساءً (4 - 8)")] Evening = 2
}
