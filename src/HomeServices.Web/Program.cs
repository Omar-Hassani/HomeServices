using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.WebEncoders;
using HomeServices.Infrastructure;
using HomeServices.Infrastructure.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

    var messages = options.ModelBindingMessageProvider;
    messages.SetValueIsInvalidAccessor(value => $"القيمة '{value}' غير صالحة.");
    messages.SetValueMustBeANumberAccessor(field => $"يجب أن يكون الحقل {field} رقماً.");
    messages.SetValueMustNotBeNullAccessor(field => $"الحقل {field} مطلوب.");
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"القيمة '{value}' غير صالحة للحقل {field}.");
    messages.SetMissingBindRequiredValueAccessor(field => $"لم يتم إدخال قيمة للحقل {field}.");
    messages.SetMissingKeyOrValueAccessor(() => "القيمة مطلوبة.");
    messages.SetMissingRequestBodyRequiredValueAccessor(() => "محتوى الطلب مطلوب.");
    messages.SetNonPropertyAttemptedValueIsInvalidAccessor(value => $"القيمة '{value}' غير صالحة.");
    messages.SetNonPropertyUnknownValueIsInvalidAccessor(() => "القيمة المدخلة غير صالحة.");
    messages.SetNonPropertyValueMustBeANumberAccessor(() => "يجب أن تكون القيمة رقماً.");
    messages.SetUnknownValueIsInvalidAccessor(field => $"القيمة المدخلة للحقل {field} غير صالحة.");
});

// إخراج النصوص العربية كما هي بدل ترميزها ككيانات &#x...; (يبقى ترميز الرموز الخطرة كما هو)
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var culture = CreateArabicCulture();
    options.DefaultRequestCulture = new RequestCulture(culture);
    options.SupportedCultures = [culture];
    options.SupportedUICultures = [culture];
    options.RequestCultureProviders.Clear();
});

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCode/{0}");
app.UseHttpsRedirection();

// الملفات المرفوعة وقت التشغيل (wwwroot/uploads) لا يخدمها MapStaticAssets لأنها غير موجودة وقت البناء
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        if (context.Context.Request.Path.StartsWithSegments("/uploads"))
        {
            context.Context.Response.Headers.XContentTypeOptions = "nosniff";
        }
    }
});

app.UseRequestLocalization();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

// العربية بإعدادات سوريا مع التقويم الميلادي والأرقام اللاتينية
static CultureInfo CreateArabicCulture()
{
    var culture = (CultureInfo)CultureInfo.GetCultureInfo("ar-SY").Clone();
    var gregorian = culture.OptionalCalendars.OfType<GregorianCalendar>().FirstOrDefault();
    if (gregorian is not null)
    {
        culture.DateTimeFormat.Calendar = gregorian;
    }

    culture.NumberFormat.NativeDigits = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    culture.NumberFormat.DigitSubstitution = DigitShapes.None;
    culture.NumberFormat.NumberDecimalSeparator = ".";
    culture.NumberFormat.NumberGroupSeparator = ",";
    culture.NumberFormat.CurrencyDecimalSeparator = ".";
    culture.NumberFormat.CurrencyGroupSeparator = ",";
    culture.NumberFormat.PercentDecimalSeparator = ".";
    culture.NumberFormat.PercentGroupSeparator = ",";
    return culture;
}
