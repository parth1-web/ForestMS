using Microsoft.EntityFrameworkCore.Migrations;

namespace LE.Web.Migrations
{
    public partial class AddMenuIdToRolePermissionMaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE role_permission_maps ADD COLUMN IF NOT EXISTS menu_id BIGINT;");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS IX_role_permission_maps_menu_id ON role_permission_maps(menu_id);");
            migrationBuilder.Sql(@"ALTER TABLE role_permission_maps ADD CONSTRAINT FK_role_permission_maps_dynamic_menus_menu_id FOREIGN KEY (menu_id) REFERENCES dynamic_menus(dynamic_menu_id) ON DELETE RESTRICT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE role_permission_maps DROP CONSTRAINT IF EXISTS FK_role_permission_maps_dynamic_menus_menu_id;");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_role_permission_maps_menu_id;");
            migrationBuilder.Sql(@"ALTER TABLE role_permission_maps DROP COLUMN IF EXISTS menu_id;");
        }
    }
}