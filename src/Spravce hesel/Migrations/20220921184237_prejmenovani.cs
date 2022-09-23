using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravce_hesel.Migrations
{
    public partial class prejmenovani : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_uivatele",
                table: "uivatele");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hesla",
                table: "hesla");

            migrationBuilder.RenameTable(
                name: "uivatele",
                newName: "uivatel");

            migrationBuilder.RenameTable(
                name: "hesla",
                newName: "heslo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_uivatel",
                table: "uivatel",
                column: "email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_heslo",
                table: "heslo",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_uivatel",
                table: "uivatel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_heslo",
                table: "heslo");

            migrationBuilder.RenameTable(
                name: "uivatel",
                newName: "uivatele");

            migrationBuilder.RenameTable(
                name: "heslo",
                newName: "hesla");

            migrationBuilder.AddPrimaryKey(
                name: "PK_uivatele",
                table: "uivatele",
                column: "email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_hesla",
                table: "hesla",
                column: "Id");
        }
    }
}
