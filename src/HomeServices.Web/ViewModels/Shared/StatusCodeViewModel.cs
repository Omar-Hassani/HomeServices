namespace HomeServices.Web.ViewModels.Shared;

public class StatusCodeViewModel
{
    public int Code { get; init; }

    public string Title => Code switch
    {
        404 => "الصفحة غير موجودة",
        403 => "لا تملك صلاحية الوصول",
        _ => "حدث خطأ ما"
    };

    public string Message => Code switch
    {
        404 => "ربما تم نقل الصفحة أو أن الرابط غير صحيح. يمكنك العودة للرئيسية أو البحث عن الخدمة التي تحتاجها.",
        403 => "هذه الصفحة غير متاحة لحسابك.",
        _ => "تعذّر إكمال طلبك حالياً، يرجى المحاولة بعد قليل."
    };
}
