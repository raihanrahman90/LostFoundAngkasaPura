using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class notification_add_foreign_key_to_item_claim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemClaimId",
                table: "user_notification",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ItemClaimId",
                table: "admin_notification",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_user_notification_ItemClaimId",
                table: "user_notification",
                column: "ItemClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_admin_notification_ItemClaimId",
                table: "admin_notification",
                column: "ItemClaimId");

            migrationBuilder.AddForeignKey(
                name: "FK_admin_notification_item_claim_ItemClaimId",
                table: "admin_notification",
                column: "ItemClaimId",
                principalTable: "item_claim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_notification_item_claim_ItemClaimId",
                table: "user_notification",
                column: "ItemClaimId",
                principalTable: "item_claim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admin_notification_item_claim_ItemClaimId",
                table: "admin_notification");

            migrationBuilder.DropForeignKey(
                name: "FK_user_notification_item_claim_ItemClaimId",
                table: "user_notification");

            migrationBuilder.DropIndex(
                name: "IX_user_notification_ItemClaimId",
                table: "user_notification");

            migrationBuilder.DropIndex(
                name: "IX_admin_notification_ItemClaimId",
                table: "admin_notification");

            migrationBuilder.DropColumn(
                name: "ItemClaimId",
                table: "user_notification");

            migrationBuilder.DropColumn(
                name: "ItemClaimId",
                table: "admin_notification");
        }
    }
}
