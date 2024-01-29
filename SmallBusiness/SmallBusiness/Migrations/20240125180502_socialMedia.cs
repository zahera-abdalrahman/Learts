using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallBusiness.Migrations
{
    public partial class socialMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacebookLink",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstagramLink",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PinterestLink",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookLink",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "InstagramLink",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "PinterestLink",
                table: "Profile");
        }
    }
}
