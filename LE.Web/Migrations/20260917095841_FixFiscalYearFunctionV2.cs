using Microsoft.EntityFrameworkCore.Migrations;

namespace LE.Web.Migrations
{
    public partial class FixFiscalYearFunctionV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION fn_get_fiscal_year_id(p_date timestamp without time zone)
RETURNS integer AS $$
DECLARE
    v_fiscal_year_id integer;
BEGIN
    SELECT id INTO v_fiscal_year_id
    FROM financial_year
    WHERE DATE(p_date) BETWEEN DATE(startdate) AND DATE(enddate)
    ORDER BY id DESC
    LIMIT 1;
    
    RETURN v_fiscal_year_id;
END;
$$ LANGUAGE plpgsql;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION fn_get_fiscal_year_id(p_date timestamp without time zone)
RETURNS integer AS $$
DECLARE
    v_fiscal_year_id integer;
BEGIN
    SELECT id INTO v_fiscal_year_id
    FROM financial_year
    WHERE DATE(p_date) BETWEEN DATE(start_date) AND DATE(end_date)
    ORDER BY id DESC
    LIMIT 1;
    
    RETURN v_fiscal_year_id;
END;
$$ LANGUAGE plpgsql;
");
        }
    }
}