using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class add_approval_by : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "item_claim_approval",
                type: "varchar(36)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_item_claim_approval_AdminId",
                table: "item_claim_approval",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_item_claim_approval_admin_AdminId",
                table: "item_claim_approval",
                column: "AdminId",
                principalTable: "admin",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_item_claim_approval_admin_AdminId",
                table: "item_claim_approval");

            migrationBuilder.DropIndex(
                name: "IX_item_claim_approval_AdminId",
                table: "item_claim_approval");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "item_claim_approval");
        }
    }
}
