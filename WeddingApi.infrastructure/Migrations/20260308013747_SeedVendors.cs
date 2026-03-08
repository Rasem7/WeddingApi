using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WeddingApi.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedVendors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceProviders",
                columns: new[] { "Id", "Category", "Description", "IsActive", "Location", "Name", "Phone", "PriceFrom", "Rating", "ReviewCount" },
                values: new object[,]
                {
                    { 1, "قاعات الأفراح", "أفخم قاعات الأفراح في القاهرة", true, "مدينة نصر، القاهرة", "بالاس ويدينج هول", "01001234567", 30000m, 4.9000000000000004, 234 },
                    { 2, "التصوير والفيديو", "تصوير احترافي لحفلات الزفاف", true, "المعادي، القاهرة", "ستوديو ليلى فوتو", "01112345678", 8000m, 4.7999999999999998, 189 },
                    { 3, "تنسيق الزهور", "تنسيق زهور وديكور للأفراح", true, "الزمالك، القاهرة", "فلاورز باي نور", "01223456789", 5000m, 4.7000000000000002, 156 },
                    { 4, "الكيترينج", "أشهى المأكولات لحفل زفافك", true, "التجمع الخامس", "لافوشيه كيترينج", "01334567890", 120m, 4.5999999999999996, 312 },
                    { 5, "فساتين الزفاف", "أجمل فساتين الزفاف", true, "المهندسين، القاهرة", "جلوري برايدال", "01445678901", 5000m, 4.9000000000000004, 421 },
                    { 6, "الموسيقى", "فرقة موسيقية متخصصة في حفلات الزفاف", true, "وسط البلد، القاهرة", "موسيقار بند", "01556789012", 8000m, 4.7000000000000002, 98 },
                    { 7, "كوافير ومكياج", "متخصصون في إطلالات العرايس", true, "المعادي، القاهرة", "جلام بيوتي", "01667890123", 3000m, 4.7999999999999998, 203 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ServiceProviders",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
