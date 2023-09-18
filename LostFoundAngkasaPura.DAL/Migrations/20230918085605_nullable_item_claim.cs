using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class nullable_item_claim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_closing_documentation_item_claim_ItemClaimId",
                table: "closing_documentation");

            migrationBuilder.AlterColumn<string>(
                name: "ItemClaimId",
                table: "closing_documentation",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_closing_documentation_item_claim_ItemClaimId",
                table: "closing_documentation",
                column: "ItemClaimId",
                principalTable: "item_claim",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_closing_documentation_item_claim_ItemClaimId",
                table: "closing_documentation");

            migrationBuilder.UpdateData(
                table: "closing_documentation",
                keyColumn: "ItemClaimId",
                keyValue: null,
                column: "ItemClaimId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ItemClaimId",
                table: "closing_documentation",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_closing_documentation_item_claim_ItemClaimId",
                table: "closing_documentation",
                column: "ItemClaimId",
                principalTable: "item_claim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
