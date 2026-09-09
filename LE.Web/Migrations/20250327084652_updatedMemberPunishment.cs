using Microsoft.EntityFrameworkCore.Migrations;

namespace LE.Web.Migrations
{
    public partial class updatedMemberPunishment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_member_punishment_members_MemberId",
                table: "member_punishment");

            migrationBuilder.DropIndex(
                name: "IX_member_punishment_MemberId",
                table: "member_punishment");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "member_punishment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MemberId",
                table: "member_punishment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_member_punishment_MemberId",
                table: "member_punishment",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_member_punishment_members_MemberId",
                table: "member_punishment",
                column: "MemberId",
                principalTable: "members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
