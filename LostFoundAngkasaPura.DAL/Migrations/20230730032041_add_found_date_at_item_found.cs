using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundAngkasaPura.DAL.Migrations
{
    public partial class add_found_date_at_item_found : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FoundDate",
                table: "item_found",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoundDate",
                table: "item_found");
        }
    }
}
