using Microsoft.EntityFrameworkCore.Migrations;

namespace LE.Web.Migrations
{
    public partial class p1BillingSettingsUniqueKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove duplicate counter rows created by the old racy read-modify-write
            // (keep the row with the highest value so no bill number is reused), then
            // enforce uniqueness so the atomic UPDATE ... RETURNING sequence is safe.
            migrationBuilder.Sql(@"
DELETE FROM billing_settings a
USING billing_settings b
WHERE a.""key"" = b.""key""
  AND a.settings_id > b.settings_id
  AND a.value < b.value;
DELETE FROM billing_settings a
USING billing_settings b
WHERE a.""key"" = b.""key""
  AND a.settings_id > b.settings_id
  AND a.value = b.value;
");
            migrationBuilder.CreateIndex(
                name: "IX_billing_settings_key",
                table: "billing_settings",
                column: "key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_billing_settings_key",
                table: "billing_settings");
        }
    }
}
