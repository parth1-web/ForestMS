using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LE.Web.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_settings",
                columns: table => new
                {
                    settings_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(nullable: false),
                    value = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_settings", x => x.settings_id);
                });

            migrationBuilder.CreateTable(
                name: "authentications",
                columns: table => new
                {
                    authentication_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_id = table.Column<long>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    username = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authentications", x => x.authentication_id);
                });

            migrationBuilder.CreateTable(
                name: "billing_settings",
                columns: table => new
                {
                    settings_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(nullable: false),
                    value = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_settings", x => x.settings_id);
                });

            migrationBuilder.CreateTable(
                name: "chiran_sales",
                columns: table => new
                {
                    chiran_sales_id = table.Column<long>(nullable: false),
                    sales_date = table.Column<DateTime>(nullable: false),
                    entry_date = table.Column<DateTime>(nullable: false),
                    nep_sales_date = table.Column<string>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    remarks = table.Column<string>(maxLength: 150, nullable: true),
                    name = table.Column<string>(nullable: true),
                    tax_amount = table.Column<decimal>(nullable: false),
                    address = table.Column<string>(nullable: true),
                    sales_type = table.Column<int>(nullable: false),
                    member_id = table.Column<long>(nullable: true),
                    user_id = table.Column<long>(nullable: false),
                    is_cancelled = table.Column<bool>(nullable: false),
                    cancelled_date = table.Column<DateTime>(nullable: false),
                    cancelled_by = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chiran_sales", x => x.chiran_sales_id);
                });

            migrationBuilder.CreateTable(
                name: "day_close",
                columns: table => new
                {
                    day_close_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nep_close_date = table.Column<string>(nullable: false),
                    eng_close_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_day_close", x => x.day_close_id);
                });

            migrationBuilder.CreateTable(
                name: "firewood_sales",
                columns: table => new
                {
                    firewood_sales_id = table.Column<long>(nullable: false),
                    sales_date = table.Column<DateTime>(nullable: false),
                    nep_sales_date = table.Column<string>(nullable: false),
                    entry_date = table.Column<DateTime>(nullable: false),
                    sales_type = table.Column<int>(nullable: false),
                    type_id = table.Column<long>(nullable: true),
                    others_name = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    total_amount = table.Column<decimal>(nullable: false),
                    remarks = table.Column<string>(maxLength: 150, nullable: true),
                    user_id = table.Column<long>(nullable: false),
                    is_cancelled = table.Column<bool>(nullable: false),
                    cancelled_date = table.Column<DateTime>(nullable: false),
                    cancelled_by = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_firewood_sales", x => x.firewood_sales_id);
                });

            migrationBuilder.CreateTable(
                name: "fiscal_year",
                columns: table => new
                {
                    fiscal_year_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    month = table.Column<string>(nullable: false),
                    day = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fiscal_year", x => x.fiscal_year_id);
                });

            migrationBuilder.CreateTable(
                name: "furniture_category",
                columns: table => new
                {
                    furniture_category_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    description = table.Column<string>(nullable: true),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_furniture_category", x => x.furniture_category_id);
                });

            migrationBuilder.CreateTable(
                name: "furniture_sales",
                columns: table => new
                {
                    furniture_sales_id = table.Column<long>(nullable: false),
                    sales_date = table.Column<DateTime>(nullable: false),
                    nep_sales_date = table.Column<string>(nullable: false),
                    entry_date = table.Column<DateTime>(nullable: false),
                    sales_type = table.Column<int>(nullable: false),
                    type_id = table.Column<long>(nullable: true),
                    others_name = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    total_amount = table.Column<decimal>(nullable: false),
                    remarks = table.Column<string>(maxLength: 150, nullable: true),
                    user_id = table.Column<long>(nullable: false),
                    is_cancelled = table.Column<bool>(nullable: false),
                    cancelled_date = table.Column<DateTime>(nullable: false),
                    cancelled_by = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_furniture_sales", x => x.furniture_sales_id);
                });

            migrationBuilder.CreateTable(
                name: "ledger_group",
                columns: table => new
                {
                    ledger_group_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: false),
                    code = table.Column<string>(nullable: false),
                    ledger_group_type = table.Column<int>(nullable: false),
                    is_custom = table.Column<bool>(nullable: false),
                    parent_ledger_group_id = table.Column<long>(nullable: false),
                    ledgers_count = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_group", x => x.ledger_group_id);
                });

            migrationBuilder.CreateTable(
                name: "ledger_setup",
                columns: table => new
                {
                    ledger_setup_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(maxLength: 70, nullable: false),
                    value = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_setup", x => x.ledger_setup_id);
                });

            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    module_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_name = table.Column<string>(maxLength: 30, nullable: false),
                    module_code = table.Column<string>(maxLength: 15, nullable: true),
                    display_icon = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modules", x => x.module_id);
                });

            migrationBuilder.CreateTable(
                name: "organization_setup",
                columns: table => new
                {
                    organization_setup_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(maxLength: 70, nullable: false),
                    value = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_setup", x => x.organization_setup_id);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    payment_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    payment_to = table.Column<long>(nullable: false),
                    payment_from = table.Column<long>(nullable: false),
                    transaction_date = table.Column<DateTime>(nullable: false),
                    nep_transaction_date = table.Column<string>(nullable: true),
                    entry_date = table.Column<DateTime>(nullable: false),
                    nep_entry_date = table.Column<string>(nullable: true),
                    amount = table.Column<decimal>(nullable: false),
                    discount = table.Column<decimal>(nullable: false),
                    remarks = table.Column<string>(nullable: false),
                    user_id = table.Column<long>(nullable: false),
                    cheque_no = table.Column<string>(nullable: true),
                    cheque_date = table.Column<string>(nullable: true),
                    is_cancelled = table.Column<bool>(nullable: false),
                    cancelled_by = table.Column<long>(nullable: false),
                    cancelled_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.payment_id);
                });

            migrationBuilder.CreateTable(
                name: "piling",
                columns: table => new
                {
                    piling_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_piling", x => x.piling_id);
                });

            migrationBuilder.CreateTable(
                name: "purpose_category",
                columns: table => new
                {
                    stock_category_purpose_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purpose_category", x => x.stock_category_purpose_id);
                });

            migrationBuilder.CreateTable(
                name: "receipt",
                columns: table => new
                {
                    receipt_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    receipt_to = table.Column<long>(nullable: false),
                    receipt_from = table.Column<long>(nullable: false),
                    transaction_date = table.Column<DateTime>(nullable: false),
                    nep_transaction_date = table.Column<string>(nullable: true),
                    entry_date = table.Column<DateTime>(nullable: false),
                    nep_entry_date = table.Column<string>(nullable: true),
                    amount = table.Column<decimal>(nullable: false),
                    discount = table.Column<decimal>(nullable: false),
                    remarks = table.Column<string>(nullable: true),
                    user_id = table.Column<long>(nullable: false),
                    cheque_date = table.Column<string>(nullable: true),
                    cheque_no = table.Column<string>(nullable: true),
                    customer_name = table.Column<string>(nullable: true),
                    is_cancelled = table.Column<bool>(nullable: false),
                    cancelled_by = table.Column<long>(nullable: false),
                    cancelled_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt", x => x.receipt_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "service_categories",
                columns: table => new
                {
                    category_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "stock_item_availability",
                columns: table => new
                {
                    stock_item_availability_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stock_item_id = table.Column<long>(nullable: false),
                    qty = table.Column<decimal>(nullable: false),
                    last_updated_date = table.Column<DateTime>(nullable: false),
                    nep_last_updated_date = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_item_availability", x => x.stock_item_availability_id);
                });

            migrationBuilder.CreateTable(
                name: "stock_movement",
                columns: table => new
                {
                    stock_movement_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    movement_date = table.Column<DateTime>(nullable: false),
                    nep_movement_date = table.Column<string>(maxLength: 15, nullable: false),
                    movement_type = table.Column<int>(nullable: false),
                    movement_type_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_movement", x => x.stock_movement_id);
                });

            migrationBuilder.CreateTable(
                name: "stock_unit",
                columns: table => new
                {
                    stock_unit_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    short_name = table.Column<string>(maxLength: 10, nullable: false),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_unit", x => x.stock_unit_id);
                });

            migrationBuilder.CreateTable(
                name: "tole",
                columns: table => new
                {
                    tole_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tole_no = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tole", x => x.tole_id);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    transaction_id = table.Column<long>(nullable: false),
                    transaction_date = table.Column<DateTime>(nullable: false),
                    nep_transaction_date = table.Column<string>(nullable: true),
                    entry_date = table.Column<DateTime>(nullable: false),
                    nep_entry_date = table.Column<string>(nullable: true),
                    remarks = table.Column<string>(nullable: true),
                    voucher_type = table.Column<int>(nullable: false),
                    voucher_no = table.Column<long>(nullable: false),
                    amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.transaction_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(nullable: false),
                    address_line_1 = table.Column<string>(nullable: false),
                    address_line_2 = table.Column<string>(nullable: true),
                    primary_contact = table.Column<string>(nullable: false),
                    secondary_contact = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    is_active = table.Column<bool>(nullable: false),
                    created_by = table.Column<long>(nullable: false),
                    created_date = table.Column<DateTime>(nullable: false),
                    image_path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "wood_bill",
                columns: table => new
                {
                    wood_bill_id = table.Column<long>(nullable: false),
                    bill_date = table.Column<DateTime>(nullable: false),
                    nep_bill_date = table.Column<string>(nullable: false),
                    entry_date = table.Column<DateTime>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    remarks = table.Column<string>(maxLength: 150, nullable: true),
                    name = table.Column<string>(nullable: true),
                    tax_amount = table.Column<decimal>(nullable: false),
                    address = table.Column<string>(nullable: true),
                    sales_type = table.Column<int>(nullable: false),
                    user_id = table.Column<long>(nullable: false),
                    is_cancelled = table.Column<bool>(nullable: false),
                    cancelled_by = table.Column<long>(nullable: false),
                    cancelled_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wood_bill", x => x.wood_bill_id);
                });

            migrationBuilder.CreateTable(
                name: "wood_type",
                columns: table => new
                {
                    wood_type_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    default_sales_rate = table.Column<decimal>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wood_type", x => x.wood_type_id);
                });

            migrationBuilder.CreateTable(
                name: "login_sessions",
                columns: table => new
                {
                    login_session_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    authentication_id = table.Column<long>(nullable: false),
                    date_time = table.Column<DateTime>(nullable: false),
                    type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_sessions", x => x.login_session_id);
                    table.ForeignKey(
                        name: "FK_login_sessions_authentications_authentication_id",
                        column: x => x.authentication_id,
                        principalTable: "authentications",
                        principalColumn: "authentication_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "furniture",
                columns: table => new
                {
                    furniture_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: false),
                    furniture_category_id = table.Column<long>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_furniture", x => x.furniture_id);
                    table.ForeignKey(
                        name: "FK_furniture_furniture_category_furniture_category_id",
                        column: x => x.furniture_category_id,
                        principalTable: "furniture_category",
                        principalColumn: "furniture_category_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ledger",
                columns: table => new
                {
                    ledger_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: false),
                    ledger_group_id = table.Column<long>(nullable: false),
                    code = table.Column<string>(nullable: false),
                    created_date = table.Column<DateTime>(nullable: false),
                    nep_created_date = table.Column<string>(nullable: true),
                    user_id = table.Column<long>(nullable: false),
                    is_currently_used = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger", x => x.ledger_id);
                    table.ForeignKey(
                        name: "FK_ledger_ledger_group_ledger_group_id",
                        column: x => x.ledger_group_id,
                        principalTable: "ledger_group",
                        principalColumn: "ledger_group_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dynamic_menus",
                columns: table => new
                {
                    dynamic_menu_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module_id = table.Column<long>(nullable: false),
                    parent_menu_id = table.Column<long>(nullable: true),
                    menu_name = table.Column<string>(maxLength: 30, nullable: false),
                    icon = table.Column<string>(maxLength: 20, nullable: true),
                    web_url = table.Column<string>(maxLength: 50, nullable: true),
                    api_url = table.Column<string>(maxLength: 50, nullable: true),
                    display_order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dynamic_menus", x => x.dynamic_menu_id);
                    table.ForeignKey(
                        name: "FK_dynamic_menus_modules_module_id",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "module_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dynamic_menus_dynamic_menus_parent_menu_id",
                        column: x => x.parent_menu_id,
                        principalTable: "dynamic_menus",
                        principalColumn: "dynamic_menu_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permission_maps",
                columns: table => new
                {
                    role_permission_map_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<long>(nullable: false),
                    module_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission_maps", x => x.role_permission_map_id);
                    table.ForeignKey(
                        name: "FK_role_permission_maps_modules_module_id",
                        column: x => x.module_id,
                        principalTable: "modules",
                        principalColumn: "module_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_permission_maps_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "counter_sales",
                columns: table => new
                {
                    sales_id = table.Column<long>(nullable: false),
                    user_id = table.Column<long>(nullable: false),
                    sales_date = table.Column<DateTime>(nullable: false),
                    nep_sales_date = table.Column<string>(maxLength: 15, nullable: false),
                    entry_date = table.Column<DateTime>(nullable: false),
                    bill_amount = table.Column<decimal>(nullable: false),
                    discount_amount = table.Column<decimal>(nullable: false),
                    tax_amount = table.Column<decimal>(nullable: false),
                    net_total = table.Column<decimal>(nullable: false),
                    return_amount = table.Column<decimal>(nullable: false),
                    remarks = table.Column<string>(maxLength: 120, nullable: true),
                    is_cancelled = table.Column<bool>(nullable: false),
                    cancelled_by = table.Column<long>(nullable: false),
                    cancelled_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counter_sales", x => x.sales_id);
                    table.ForeignKey(
                        name: "FK_counter_sales_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_role_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(nullable: false),
                    type_id = table.Column<long>(nullable: false),
                    role_id = table.Column<long>(nullable: false),
                    user_id = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.user_role_id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chiran_sales_detail",
                columns: table => new
                {
                    chiran_sales_detail_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chiran_sales_id = table.Column<long>(nullable: false),
                    wood_type_id = table.Column<long>(nullable: false),
                    circle_size = table.Column<decimal>(nullable: false),
                    breadth = table.Column<decimal>(nullable: false),
                    length = table.Column<decimal>(nullable: false),
                    quantity = table.Column<decimal>(nullable: false),
                    rate = table.Column<decimal>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    total_size = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chiran_sales_detail", x => x.chiran_sales_detail_id);
                    table.ForeignKey(
                        name: "FK_chiran_sales_detail_chiran_sales_chiran_sales_id",
                        column: x => x.chiran_sales_id,
                        principalTable: "chiran_sales",
                        principalColumn: "chiran_sales_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chiran_sales_detail_wood_type_wood_type_id",
                        column: x => x.wood_type_id,
                        principalTable: "wood_type",
                        principalColumn: "wood_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_item",
                columns: table => new
                {
                    stock_item_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(maxLength: 70, nullable: false),
                    wood_type_id = table.Column<long>(nullable: false),
                    stock_unit_id = table.Column<long>(nullable: false),
                    threshold = table.Column<decimal>(nullable: false),
                    default_sales_rate = table.Column<decimal>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    stock_item_availability_id = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_item", x => x.stock_item_id);
                    table.ForeignKey(
                        name: "FK_stock_item_stock_item_availability_stock_item_availability_~",
                        column: x => x.stock_item_availability_id,
                        principalTable: "stock_item_availability",
                        principalColumn: "stock_item_availability_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_item_stock_unit_stock_unit_id",
                        column: x => x.stock_unit_id,
                        principalTable: "stock_unit",
                        principalColumn: "stock_unit_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_item_wood_type_wood_type_id",
                        column: x => x.wood_type_id,
                        principalTable: "wood_type",
                        principalColumn: "wood_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wood_details",
                columns: table => new
                {
                    wood_details_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stock_category_purpose_id = table.Column<long>(nullable: false),
                    piling_id = table.Column<long>(nullable: false),
                    stock_type_id = table.Column<long>(nullable: false),
                    balla_balli_category_id = table.Column<long>(nullable: false),
                    tuna_no = table.Column<string>(maxLength: 10, nullable: false),
                    goliya_number = table.Column<string>(maxLength: 10, nullable: false),
                    wood_type_id = table.Column<long>(nullable: false),
                    circle_size = table.Column<decimal>(nullable: false),
                    length = table.Column<decimal>(nullable: false),
                    year = table.Column<string>(nullable: false),
                    grade = table.Column<int>(nullable: false),
                    created_date = table.Column<DateTime>(nullable: false),
                    is_sold = table.Column<bool>(nullable: false),
                    sales_id = table.Column<long>(nullable: true),
                    fresh_total_size = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wood_details", x => x.wood_details_id);
                    table.ForeignKey(
                        name: "FK_wood_details_piling_piling_id",
                        column: x => x.piling_id,
                        principalTable: "piling",
                        principalColumn: "piling_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wood_details_purpose_category_stock_category_purpose_id",
                        column: x => x.stock_category_purpose_id,
                        principalTable: "purpose_category",
                        principalColumn: "stock_category_purpose_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wood_details_wood_type_wood_type_id",
                        column: x => x.wood_type_id,
                        principalTable: "wood_type",
                        principalColumn: "wood_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "furniture_sales_detail",
                columns: table => new
                {
                    furniture_sales_detail_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    furniture_sales_id = table.Column<long>(nullable: false),
                    furniture_id = table.Column<long>(nullable: false),
                    rate = table.Column<decimal>(nullable: false),
                    quantity = table.Column<decimal>(nullable: false),
                    amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_furniture_sales_detail", x => x.furniture_sales_detail_id);
                    table.ForeignKey(
                        name: "FK_furniture_sales_detail_furniture_furniture_id",
                        column: x => x.furniture_id,
                        principalTable: "furniture",
                        principalColumn: "furniture_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_furniture_sales_detail_furniture_sales_furniture_sales_id",
                        column: x => x.furniture_sales_id,
                        principalTable: "furniture_sales",
                        principalColumn: "furniture_sales_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ledger_balance",
                columns: table => new
                {
                    ledger_balance_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ledger_id = table.Column<long>(nullable: false),
                    ledger_group_id = table.Column<long>(nullable: false),
                    balance = table.Column<decimal>(nullable: false),
                    updated_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_balance", x => x.ledger_balance_id);
                    table.ForeignKey(
                        name: "FK_ledger_balance_ledger_group_ledger_group_id",
                        column: x => x.ledger_group_id,
                        principalTable: "ledger_group",
                        principalColumn: "ledger_group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ledger_balance_ledger_ledger_id",
                        column: x => x.ledger_id,
                        principalTable: "ledger",
                        principalColumn: "ledger_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "membership",
                columns: table => new
                {
                    MembershipId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MembershipCode = table.Column<string>(maxLength: 25, nullable: false),
                    Area = table.Column<string>(nullable: true),
                    ToleNo = table.Column<string>(nullable: true),
                    Religion = table.Column<int>(nullable: false),
                    HasBioGas = table.Column<bool>(nullable: false),
                    HasLPG = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    IsCancelled = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    LedgerId = table.Column<long>(nullable: false),
                    CancelledDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership", x => x.MembershipId);
                    table.ForeignKey(
                        name: "FK_membership_ledger_LedgerId",
                        column: x => x.LedgerId,
                        principalTable: "ledger",
                        principalColumn: "ledger_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    service_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<long>(nullable: false),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    rate = table.Column<decimal>(nullable: false),
                    created_date = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<long>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    tax = table.Column<decimal>(nullable: false),
                    ledger_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.service_id);
                    table.ForeignKey(
                        name: "FK_services_service_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "service_categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_services_ledger_category_id",
                        column: x => x.category_id,
                        principalTable: "ledger",
                        principalColumn: "ledger_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transaction_detail",
                columns: table => new
                {
                    transaction_detail_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    transaction_id = table.Column<long>(nullable: false),
                    ledger_id = table.Column<long>(nullable: false),
                    ref_ledger_id = table.Column<long>(nullable: false),
                    transaction_date = table.Column<DateTime>(nullable: false),
                    dr_amount = table.Column<decimal>(nullable: false),
                    cr_amount = table.Column<decimal>(nullable: false),
                    balance = table.Column<decimal>(nullable: false),
                    FiscalYearId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_detail", x => x.transaction_detail_id);
                    table.ForeignKey(
                        name: "FK_transaction_detail_ledger_ledger_id",
                        column: x => x.ledger_id,
                        principalTable: "ledger",
                        principalColumn: "ledger_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transaction_detail_transaction_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transaction",
                        principalColumn: "transaction_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "firewood_sales_detail",
                columns: table => new
                {
                    firewood_sales_detail_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    firewood_sales_id = table.Column<long>(nullable: false),
                    stock_item_id = table.Column<long>(nullable: false),
                    rate = table.Column<decimal>(nullable: false),
                    quantity = table.Column<decimal>(nullable: false),
                    amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_firewood_sales_detail", x => x.firewood_sales_detail_id);
                    table.ForeignKey(
                        name: "FK_firewood_sales_detail_firewood_sales_firewood_sales_id",
                        column: x => x.firewood_sales_id,
                        principalTable: "firewood_sales",
                        principalColumn: "firewood_sales_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_firewood_sales_detail_stock_item_stock_item_id",
                        column: x => x.stock_item_id,
                        principalTable: "stock_item",
                        principalColumn: "stock_item_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchase",
                columns: table => new
                {
                    purchase_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(nullable: false),
                    stock_item_id = table.Column<long>(nullable: false),
                    qty = table.Column<decimal>(nullable: false),
                    purchase_date = table.Column<DateTime>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    nep_purchase_date = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase", x => x.purchase_id);
                    table.ForeignKey(
                        name: "FK_purchase_stock_item_stock_item_id",
                        column: x => x.stock_item_id,
                        principalTable: "stock_item",
                        principalColumn: "stock_item_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "damaged_wood_detail",
                columns: table => new
                {
                    damaged_wood_details_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wood_details_id = table.Column<long>(nullable: false),
                    damaged_first_size = table.Column<decimal>(nullable: false),
                    damaged_second_size = table.Column<decimal>(nullable: false),
                    damaged_third_size = table.Column<decimal>(nullable: false),
                    damaged_fourth_size = table.Column<decimal>(nullable: false),
                    damaged_fifth_size = table.Column<decimal>(nullable: false),
                    dividor_value = table.Column<long>(nullable: false),
                    damaged_feet_size = table.Column<decimal>(nullable: false),
                    total_damaged_size = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_damaged_wood_detail", x => x.damaged_wood_details_id);
                    table.ForeignKey(
                        name: "FK_damaged_wood_detail_wood_details_wood_details_id",
                        column: x => x.wood_details_id,
                        principalTable: "wood_details",
                        principalColumn: "wood_details_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wood_bill_detail",
                columns: table => new
                {
                    wood_bill_detail_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wood_bill_id = table.Column<long>(nullable: false),
                    wood_details_id = table.Column<long>(nullable: false),
                    rate = table.Column<decimal>(nullable: false),
                    amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wood_bill_detail", x => x.wood_bill_detail_id);
                    table.ForeignKey(
                        name: "FK_wood_bill_detail_wood_bill_wood_bill_id",
                        column: x => x.wood_bill_id,
                        principalTable: "wood_bill",
                        principalColumn: "wood_bill_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wood_bill_detail_wood_details_wood_details_id",
                        column: x => x.wood_details_id,
                        principalTable: "wood_details",
                        principalColumn: "wood_details_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "member_punishment",
                columns: table => new
                {
                    MemberPunishmentId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MembershipId = table.Column<long>(nullable: false),
                    IllegalActivity = table.Column<string>(nullable: true),
                    IssueDate = table.Column<DateTime>(nullable: false),
                    NepIssueDate = table.Column<string>(nullable: true),
                    PunishmentValidity = table.Column<DateTime>(nullable: false),
                    NepPunishmentValidity = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member_punishment", x => x.MemberPunishmentId);
                    table.ForeignKey(
                        name: "FK_member_punishment_membership_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "membership",
                        principalColumn: "MembershipId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    MemberId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MembershipId = table.Column<long>(nullable: false),
                    MemberCitizenship = table.Column<string>(maxLength: 50, nullable: true),
                    FullName = table.Column<string>(maxLength: 100, nullable: false),
                    Address = table.Column<string>(maxLength: 150, nullable: true),
                    ContactNo = table.Column<string>(maxLength: 20, nullable: true),
                    Age = table.Column<int>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    IsGharmuli = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ImageName = table.Column<string>(nullable: true),
                    FamilyRole = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_members_membership_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "membership",
                        principalColumn: "MembershipId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "membership_validity",
                columns: table => new
                {
                    MembershipValidityId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MembershipId = table.Column<long>(nullable: false),
                    IssueDate = table.Column<string>(nullable: true),
                    NepValidityDate = table.Column<string>(nullable: true),
                    ValidityDate = table.Column<DateTime>(nullable: false),
                    RenewedDate = table.Column<string>(nullable: true),
                    IsExpired = table.Column<bool>(nullable: false),
                    IsCurrent = table.Column<bool>(nullable: false),
                    IsCancelled = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membership_validity", x => x.MembershipValidityId);
                    table.ForeignKey(
                        name: "FK_membership_validity_membership_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "membership",
                        principalColumn: "MembershipId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "counter_sales_details",
                columns: table => new
                {
                    sales_detail_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sales_id = table.Column<long>(nullable: false),
                    service_id = table.Column<long>(nullable: false),
                    rate = table.Column<decimal>(nullable: false),
                    tax_amount = table.Column<decimal>(nullable: false),
                    qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counter_sales_details", x => x.sales_detail_id);
                    table.ForeignKey(
                        name: "FK_counter_sales_details_counter_sales_sales_id",
                        column: x => x.sales_id,
                        principalTable: "counter_sales",
                        principalColumn: "sales_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_counter_sales_details_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wood_bill_members",
                columns: table => new
                {
                    wood_bill_member_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wood_bill_id = table.Column<long>(nullable: false),
                    member_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wood_bill_members", x => x.wood_bill_member_id);
                    table.ForeignKey(
                        name: "FK_wood_bill_members_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wood_bill_members_wood_bill_wood_bill_id",
                        column: x => x.wood_bill_id,
                        principalTable: "wood_bill",
                        principalColumn: "wood_bill_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "wood_bill_members_transaction",
                columns: table => new
                {
                    wood_bill_member_transaction_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wood_bill_id = table.Column<long>(nullable: false),
                    member_id = table.Column<long>(nullable: false),
                    wood_details_id = table.Column<long>(nullable: false),
                    rate = table.Column<decimal>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    tax_amount = table.Column<decimal>(nullable: false),
                    is_cancelled = table.Column<bool>(nullable: false),
                    quantity = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wood_bill_members_transaction", x => x.wood_bill_member_transaction_id);
                    table.ForeignKey(
                        name: "FK_wood_bill_members_transaction_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wood_bill_members_transaction_wood_bill_wood_bill_id",
                        column: x => x.wood_bill_id,
                        principalTable: "wood_bill",
                        principalColumn: "wood_bill_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wood_bill_members_transaction_wood_details_wood_details_id",
                        column: x => x.wood_details_id,
                        principalTable: "wood_details",
                        principalColumn: "wood_details_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chiran_sales_detail_chiran_sales_id",
                table: "chiran_sales_detail",
                column: "chiran_sales_id");

            migrationBuilder.CreateIndex(
                name: "IX_chiran_sales_detail_wood_type_id",
                table: "chiran_sales_detail",
                column: "wood_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_counter_sales_user_id",
                table: "counter_sales",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_counter_sales_details_sales_id",
                table: "counter_sales_details",
                column: "sales_id");

            migrationBuilder.CreateIndex(
                name: "IX_counter_sales_details_service_id",
                table: "counter_sales_details",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_damaged_wood_detail_wood_details_id",
                table: "damaged_wood_detail",
                column: "wood_details_id");

            migrationBuilder.CreateIndex(
                name: "IX_dynamic_menus_module_id",
                table: "dynamic_menus",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_dynamic_menus_parent_menu_id",
                table: "dynamic_menus",
                column: "parent_menu_id");

            migrationBuilder.CreateIndex(
                name: "IX_firewood_sales_detail_firewood_sales_id",
                table: "firewood_sales_detail",
                column: "firewood_sales_id");

            migrationBuilder.CreateIndex(
                name: "IX_firewood_sales_detail_stock_item_id",
                table: "firewood_sales_detail",
                column: "stock_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_furniture_furniture_category_id",
                table: "furniture",
                column: "furniture_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_furniture_sales_detail_furniture_id",
                table: "furniture_sales_detail",
                column: "furniture_id");

            migrationBuilder.CreateIndex(
                name: "IX_furniture_sales_detail_furniture_sales_id",
                table: "furniture_sales_detail",
                column: "furniture_sales_id");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_ledger_group_id",
                table: "ledger",
                column: "ledger_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_balance_ledger_group_id",
                table: "ledger_balance",
                column: "ledger_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_balance_ledger_id",
                table: "ledger_balance",
                column: "ledger_id");

            migrationBuilder.CreateIndex(
                name: "IX_login_sessions_authentication_id",
                table: "login_sessions",
                column: "authentication_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_punishment_MembershipId",
                table: "member_punishment",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_members_MembershipId",
                table: "members",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_membership_LedgerId",
                table: "membership",
                column: "LedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_membership_validity_MembershipId",
                table: "membership_validity",
                column: "MembershipId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_stock_item_id",
                table: "purchase",
                column: "stock_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_maps_module_id",
                table: "role_permission_maps",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_maps_role_id",
                table: "role_permission_maps",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_services_category_id",
                table: "services",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_item_stock_item_availability_id",
                table: "stock_item",
                column: "stock_item_availability_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_item_stock_unit_id",
                table: "stock_item",
                column: "stock_unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_item_wood_type_id",
                table: "stock_item",
                column: "wood_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_detail_ledger_id",
                table: "transaction_detail",
                column: "ledger_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_detail_transaction_id",
                table: "transaction_detail",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_bill_detail_wood_bill_id",
                table: "wood_bill_detail",
                column: "wood_bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_bill_detail_wood_details_id",
                table: "wood_bill_detail",
                column: "wood_details_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_bill_members_member_id",
                table: "wood_bill_members",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_bill_members_wood_bill_id",
                table: "wood_bill_members",
                column: "wood_bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_bill_members_transaction_member_id",
                table: "wood_bill_members_transaction",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_bill_members_transaction_wood_bill_id",
                table: "wood_bill_members_transaction",
                column: "wood_bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_bill_members_transaction_wood_details_id",
                table: "wood_bill_members_transaction",
                column: "wood_details_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_details_piling_id",
                table: "wood_details",
                column: "piling_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_details_stock_category_purpose_id",
                table: "wood_details",
                column: "stock_category_purpose_id");

            migrationBuilder.CreateIndex(
                name: "IX_wood_details_wood_type_id",
                table: "wood_details",
                column: "wood_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_settings");

            migrationBuilder.DropTable(
                name: "billing_settings");

            migrationBuilder.DropTable(
                name: "chiran_sales_detail");

            migrationBuilder.DropTable(
                name: "counter_sales_details");

            migrationBuilder.DropTable(
                name: "damaged_wood_detail");

            migrationBuilder.DropTable(
                name: "day_close");

            migrationBuilder.DropTable(
                name: "dynamic_menus");

            migrationBuilder.DropTable(
                name: "firewood_sales_detail");

            migrationBuilder.DropTable(
                name: "fiscal_year");

            migrationBuilder.DropTable(
                name: "furniture_sales_detail");

            migrationBuilder.DropTable(
                name: "ledger_balance");

            migrationBuilder.DropTable(
                name: "ledger_setup");

            migrationBuilder.DropTable(
                name: "login_sessions");

            migrationBuilder.DropTable(
                name: "member_punishment");

            migrationBuilder.DropTable(
                name: "membership_validity");

            migrationBuilder.DropTable(
                name: "organization_setup");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "purchase");

            migrationBuilder.DropTable(
                name: "receipt");

            migrationBuilder.DropTable(
                name: "role_permission_maps");

            migrationBuilder.DropTable(
                name: "stock_movement");

            migrationBuilder.DropTable(
                name: "tole");

            migrationBuilder.DropTable(
                name: "transaction_detail");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "wood_bill_detail");

            migrationBuilder.DropTable(
                name: "wood_bill_members");

            migrationBuilder.DropTable(
                name: "wood_bill_members_transaction");

            migrationBuilder.DropTable(
                name: "chiran_sales");

            migrationBuilder.DropTable(
                name: "counter_sales");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "firewood_sales");

            migrationBuilder.DropTable(
                name: "furniture");

            migrationBuilder.DropTable(
                name: "furniture_sales");

            migrationBuilder.DropTable(
                name: "authentications");

            migrationBuilder.DropTable(
                name: "stock_item");

            migrationBuilder.DropTable(
                name: "modules");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "wood_bill");

            migrationBuilder.DropTable(
                name: "wood_details");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "service_categories");

            migrationBuilder.DropTable(
                name: "furniture_category");

            migrationBuilder.DropTable(
                name: "stock_item_availability");

            migrationBuilder.DropTable(
                name: "stock_unit");

            migrationBuilder.DropTable(
                name: "membership");

            migrationBuilder.DropTable(
                name: "piling");

            migrationBuilder.DropTable(
                name: "purpose_category");

            migrationBuilder.DropTable(
                name: "wood_type");

            migrationBuilder.DropTable(
                name: "ledger");

            migrationBuilder.DropTable(
                name: "ledger_group");
        }
    }
}
