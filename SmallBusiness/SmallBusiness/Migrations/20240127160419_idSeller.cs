using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallBusiness.Migrations
{
    public partial class idSeller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Sellers_SellerID",
                table: "Subscription");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Subscription",
                newName: "SellerId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_SellerID",
                table: "Subscription",
                newName: "IX_Subscription_SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Sellers_SellerId",
                table: "Subscription",
                column: "SellerId",
                principalTable: "Sellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Sellers_SellerId",
                table: "Subscription");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "Subscription",
                newName: "SellerID");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_SellerId",
                table: "Subscription",
                newName: "IX_Subscription_SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Sellers_SellerID",
                table: "Subscription",
                column: "SellerID",
                principalTable: "Sellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
