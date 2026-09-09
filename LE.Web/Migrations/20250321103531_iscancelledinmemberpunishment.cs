using Microsoft.EntityFrameworkCore.Migrations;

namespace LE.Web.Migrations
{
    public partial class iscancelledinmemberpunishment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "member_punishment",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "member_punishment");
        }
    }
}
