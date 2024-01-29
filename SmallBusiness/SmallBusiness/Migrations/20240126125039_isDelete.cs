using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallBusiness.Migrations
{
    public partial class isDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Category",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Category");
        }
    }
}
