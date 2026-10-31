using HomeServices.Domain.Entities;

namespace HomeServices.Infrastructure.Data.Seed;

/// <summary>
/// بيانات مرجعية ثابتة (الفئات والخدمات والمحافظات والمناطق) تُدرج عبر Migrations.
/// المعرّفات تُحسب من ترتيب القوائم، لذا لا يُعدَّل ترتيبها بعد إنشاء أول Migration؛
/// الإضافات اللاحقة (مناطق، خدمات) تتم من لوحة الإدارة وليس من هنا.
/// </summary>
internal static class ReferenceData
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly ServiceCategory[] Categories =
    [
        Category(1, "الكهرباء", "electricity", "fa-solid fa-bolt", "#E8A317",
            "تمديدات كهربائية، إصلاح الأعطال، الإنارة وأنظمة الطاقة الشمسية.",
            "كهربائي كهربجي كهرباء تمديدات إنارة ثريات طاقة شمسية ألواح بطاريات انفرتر إنفرتر أمبيرات"),
        Category(2, "السباكة", "plumbing", "fa-solid fa-faucet-drip", "#2D7FF9",
            "كشف التسريبات، التمديدات الصحية وتركيب الأدوات الصحية.",
            "سباك سمكري سباكة صحية تسريب تسريبات مياه مجاري صرف حنفية خزان سخان"),
        Category(3, "الدهان والديكور", "painting", "fa-solid fa-paint-roller", "#E0567A",
            "دهان داخلي وخارجي، ديكورات جبسية وورق جدران.",
            "دهان دهين طلاء بوية ديكور جبس جبسين ورق جدران"),
        Category(4, "التكييف والتبريد", "ac-refrigeration", "fa-solid fa-snowflake", "#17A2B8",
            "تركيب وصيانة المكيفات وإصلاح البرادات والمجمدات.",
            "مكيف مكيفات تكييف تبريد براد برادات ثلاجة فريزر مجمدة غاز"),
        Category(5, "نقل العفش", "furniture-moving", "fa-solid fa-truck-moving", "#7B61FF",
            "نقل الأثاث وفكه وتركيبه وتغليفه بأمان.",
            "نقل عفش أثاث موبيليا ترحيل تغليف فك تركيب شاحنة سوزوكي"),
        Category(6, "الطبخ", "cooking", "fa-solid fa-utensils", "#F2703D",
            "طباخون وطباخات للطبخ اليومي والولائم وتحضير المونة.",
            "طباخ طباخة طبخ أكل ولائم عزائم مونة حلويات مطبخ"),
        Category(7, "تنظيف المنازل", "home-cleaning", "fa-solid fa-broom", "#20A779",
            "تنظيف شامل ودوري، تنظيف السجاد والكنب وما بعد الإكساء.",
            "تنظيف منظف منظفة عاملة سجاد كنب جلي تعقيم"),
        Category(8, "الصيانة العامة", "general-maintenance", "fa-solid fa-screwdriver-wrench", "#5B6B7F",
            "نجارة، ألمنيوم وزجاج، تركيب ستائر وأعمال صيانة متنوعة.",
            "نجار نجارة ألمنيوم المنيوم زجاج ستائر حداد حدادة صيانة تصليح")
    ];

    public static readonly Service[] Services = BuildServices(
        (1, ["تمديدات كهربائية جديدة|electrical-wiring", "إصلاح الأعطال الكهربائية|electrical-repair",
             "تركيب الإنارة والثريات|lighting-installation", "أنظمة الطاقة الشمسية والبطاريات|solar-systems"]),
        (2, ["كشف وإصلاح التسريبات|leak-repair", "تمديدات صحية|plumbing-installation",
             "تركيب الأدوات الصحية|sanitary-fixtures", "تسليك المجاري|drain-cleaning"]),
        (3, ["دهان داخلي|interior-painting", "دهان خارجي|exterior-painting",
             "ديكورات جبسية|gypsum-decor", "ورق جدران|wallpaper"]),
        (4, ["تركيب المكيفات|ac-installation", "صيانة وتعبئة غاز المكيفات|ac-maintenance",
             "إصلاح البرادات والمجمدات|fridge-repair"]),
        (5, ["نقل عفش المنازل|home-moving", "فك وتركيب الأثاث|furniture-assembly",
             "تغليف الأثاث|furniture-packing", "نقل المكاتب|office-moving"]),
        (6, ["طبخ يومي منزلي|daily-cooking", "طبخ الولائم والمناسبات|event-cooking",
             "تحضير المونة|pantry-preparation", "حلويات منزلية|homemade-sweets"]),
        (7, ["تنظيف شامل للمنزل|deep-cleaning", "تنظيف دوري|regular-cleaning",
             "تنظيف السجاد والكنب|carpet-sofa-cleaning", "تنظيف بعد الإكساء|post-construction-cleaning"]),
        (8, ["نجارة وإصلاح الأبواب|carpentry", "ألمنيوم وزجاج|aluminum-glass",
             "تركيب الستائر والإكسسوارات|curtains-installation", "صيانة عامة للمنزل|handyman"]));

    private static readonly (string City, string[] Areas)[] Locations =
    [
        ("دمشق", ["المزة", "المالكي", "أبو رمانة", "كفرسوسة", "المهاجرين", "الشعلان", "باب توما", "القصاع",
                  "ركن الدين", "الميدان", "المزرعة", "برزة", "مشروع دمر", "الشاغور", "الصالحية", "القصور", "التجارة"]),
        ("ريف دمشق", ["جرمانا", "صحنايا", "أشرفية صحنايا", "قدسيا", "التل", "دوما", "يعفور", "الكسوة",
                      "جديدة عرطوز", "الزبداني", "داريا", "معضمية الشام"]),
        ("حلب", ["الفرقان", "الشهباء", "الموكامبو", "السبيل", "الحمدانية", "الجميلية", "العزيزية",
                 "حلب الجديدة", "السليمانية", "المحافظة", "السريان", "الأشرفية"]),
        ("حمص", ["الإنشاءات", "الوعر", "الغوطة", "عكرمة", "الحمراء", "الزهراء", "الخالدية", "باب السباع"]),
        ("حماة", ["مركز المدينة", "الحاضر", "جنوب الملعب", "الشريعة", "الأربعين", "طريق حلب", "القصور"]),
        ("اللاذقية", ["الزراعة", "الصليبة", "الأمريكان", "الرمل الشمالي", "الرمل الجنوبي", "الشيخ ضاهر",
                      "الطابيات", "الشاطئ الأزرق"]),
        ("طرطوس", ["مركز المدينة", "الكورنيش البحري", "بانياس", "صافيتا", "الدريكيش"]),
        ("إدلب", ["مركز المدينة", "أريحا", "جسر الشغور", "معرة النعمان"]),
        ("درعا", ["درعا المحطة", "درعا البلد", "إزرع"]),
        ("السويداء", ["مركز المدينة", "شهبا", "صلخد"]),
        ("القنيطرة", ["خان أرنبة"]),
        ("دير الزور", ["مركز المدينة", "الميادين", "البوكمال"]),
        ("الرقة", ["مركز المدينة", "الطبقة"]),
        ("الحسكة", ["مركز المدينة", "القامشلي"])
    ];

    public static readonly City[] Cities = Locations
        .Select((l, i) => new City { Id = i + 1, Name = l.City, DisplayOrder = i + 1, IsActive = true })
        .ToArray();

    public static readonly Area[] Areas = BuildAreas();

    private static ServiceCategory Category(int id, string name, string slug, string icon, string color, string description, string keywords) => new()
    {
        Id = id,
        Name = name,
        Slug = slug,
        IconClass = icon,
        AccentColor = color,
        Description = description,
        SearchKeywords = keywords,
        DisplayOrder = id,
        IsActive = true,
        CreatedAt = SeedDate
    };

    private static Service[] BuildServices(params (int CategoryId, string[] Items)[] groups)
    {
        var id = 1;
        var services = new List<Service>();

        foreach (var (categoryId, items) in groups)
        {
            for (var i = 0; i < items.Length; i++)
            {
                var parts = items[i].Split('|');
                services.Add(new Service
                {
                    Id = id++,
                    CategoryId = categoryId,
                    Name = parts[0],
                    Slug = parts[1],
                    DisplayOrder = i + 1,
                    IsActive = true,
                    CreatedAt = SeedDate
                });
            }
        }

        return services.ToArray();
    }

    private static Area[] BuildAreas()
    {
        var id = 1;
        var areas = new List<Area>();

        for (var cityIndex = 0; cityIndex < Locations.Length; cityIndex++)
        {
            foreach (var areaName in Locations[cityIndex].Areas)
            {
                areas.Add(new Area { Id = id++, CityId = cityIndex + 1, Name = areaName, IsActive = true });
            }
        }

        return areas.ToArray();
    }
}
