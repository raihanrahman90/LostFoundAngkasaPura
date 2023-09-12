using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class add_closing_image : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClosingImage",
                table: "item_found",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingImage",
                table: "item_found");
        }
    }
}
