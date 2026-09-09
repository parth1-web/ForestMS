using Microsoft.EntityFrameworkCore.Migrations;

namespace LE.Web.Migrations
{
    public partial class iscancelledaddedinmemberpunishment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "member_punishment");

            migrationBuilder.AddColumn<string>(
                name: "IsCancelledRemarks",
                table: "member_punishment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelledRemarks",
                table: "member_punishment");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "member_punishment",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
