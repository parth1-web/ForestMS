using Microsoft.EntityFrameworkCore.Migrations;

namespace LE.Web.Migrations
{
    public partial class p2AccountSettingsUniqueKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // P2/UoW: getTransactionSequence() now increments with an atomic
            // UPDATE ... RETURNING (same pattern as billing_settings, P1/B2).
            // Remove duplicate rows possibly created by the old racy
            // read-modify-write (keep the row with the highest value so no
            // voucher number is reused), then enforce uniqueness so
            // INSERT ... ON CONFLICT DO NOTHING is race-free.
            migrationBuilder.Sql(@"
DELETE FROM account_settings a
USING account_settings b
WHERE a.""key"" = b.""key""
  AND a.settings_id > b.settings_id
  AND a.value < b.value;
DELETE FROM account_settings a
USING account_settings b
WHERE a.""key"" = b.""key""
  AND a.settings_id > b.settings_id
  AND a.value = b.value;
");
            migrationBuilder.CreateIndex(
                name: "IX_account_settings_key",
                table: "account_settings",
                column: "key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_account_settings_key",
                table: "account_settings");
        }
    }
}
