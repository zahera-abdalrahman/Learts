using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallBusiness.Migrations
{
    public partial class editReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Profile_ProfileId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_ProfileId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "TestimonialStatus",
                table: "Testimonial");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "ReviewStatus",
                table: "Review",
                newName: "ProductId");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Testimonial",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Review",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Review_ProductId",
                table: "Review",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_ProductId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Testimonial");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Review",
                newName: "ReviewStatus");

            migrationBuilder.AddColumn<int>(
                name: "TestimonialStatus",
                table: "Testimonial",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Review_ProfileId",
                table: "Review",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Profile_ProfileId",
                table: "Review",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
