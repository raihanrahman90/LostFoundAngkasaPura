using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFound.DAL.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "user");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "user",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "user",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "user",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "user",
                newName: "FirstName");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "user",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "user",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
