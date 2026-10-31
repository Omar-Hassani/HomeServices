using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeServices.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorySearchKeywords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchKeywords",
                table: "ServiceCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "SearchKeywords",
                value: "كهربائي كهربجي كهرباء تمديدات إنارة ثريات طاقة شمسية ألواح بطاريات انفرتر إنفرتر أمبيرات");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "SearchKeywords",
                value: "سباك سمكري سباكة صحية تسريب تسريبات مياه مجاري صرف حنفية خزان سخان");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "SearchKeywords",
                value: "دهان دهين طلاء بوية ديكور جبس جبسين ورق جدران");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "SearchKeywords",
                value: "مكيف مكيفات تكييف تبريد براد برادات ثلاجة فريزر مجمدة غاز");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "SearchKeywords",
                value: "نقل عفش أثاث موبيليا ترحيل تغليف فك تركيب شاحنة سوزوكي");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "SearchKeywords",
                value: "طباخ طباخة طبخ أكل ولائم عزائم مونة حلويات مطبخ");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "SearchKeywords",
                value: "تنظيف منظف منظفة عاملة سجاد كنب جلي تعقيم");

            migrationBuilder.UpdateData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "SearchKeywords",
                value: "نجار نجارة ألمنيوم المنيوم زجاج ستائر حداد حدادة صيانة تصليح");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchKeywords",
                table: "ServiceCategories");
        }
    }
}
