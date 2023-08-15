using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class add_realation_approval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemClaimId",
                table: "item_claim_approval",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_item_claim_approval_ItemClaimId",
                table: "item_claim_approval",
                column: "ItemClaimId");

            migrationBuilder.AddForeignKey(
                name: "FK_item_claim_approval_item_claim_ItemClaimId",
                table: "item_claim_approval",
                column: "ItemClaimId",
                principalTable: "item_claim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_item_claim_approval_item_claim_ItemClaimId",
                table: "item_claim_approval");

            migrationBuilder.DropIndex(
                name: "IX_item_claim_approval_ItemClaimId",
                table: "item_claim_approval");

            migrationBuilder.DropColumn(
                name: "ItemClaimId",
                table: "item_claim_approval");
        }
    }
}
