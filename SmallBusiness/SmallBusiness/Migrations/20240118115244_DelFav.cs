using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallBusiness.Migrations
{
    public partial class DelFav : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFav",
                table: "Product");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFav",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
