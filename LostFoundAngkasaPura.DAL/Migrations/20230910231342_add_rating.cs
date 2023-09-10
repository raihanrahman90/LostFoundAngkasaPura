using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class add_rating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "item_claim",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "item_claim");
        }
    }
}
