using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallBusiness.Migrations
{
    public partial class EditFav2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Profile_ProfileId",
                table: "Favorite");

            migrationBuilder.DropIndex(
                name: "IX_Favorite_ProfileId",
                table: "Favorite");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Favorite");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "Favorite",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_ProfileId",
                table: "Favorite",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Profile_ProfileId",
                table: "Favorite",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
