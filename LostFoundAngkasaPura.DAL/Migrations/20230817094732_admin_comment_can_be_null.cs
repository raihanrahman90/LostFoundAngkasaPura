using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class admin_comment_can_be_null : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_item_comment_admin_AdminId",
                table: "item_comment");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "item_comment",
                type: "varchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_item_comment_admin_AdminId",
                table: "item_comment",
                column: "AdminId",
                principalTable: "admin",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_item_comment_admin_AdminId",
                table: "item_comment");

            migrationBuilder.UpdateData(
                table: "item_comment",
                keyColumn: "AdminId",
                keyValue: null,
                column: "AdminId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "item_comment",
                type: "varchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(36)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_item_comment_admin_AdminId",
                table: "item_comment",
                column: "AdminId",
                principalTable: "admin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
