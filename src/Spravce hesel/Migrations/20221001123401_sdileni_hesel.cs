using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravce_hesel.Migrations
{
    public partial class sdileni_hesel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_uzivatel",
                table: "uzivatel");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "uzivatel",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "heslo",
                table: "uzivatel",
                newName: "Heslo");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "uzivatel",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "sifra",
                table: "heslo",
                newName: "Sifra");

            migrationBuilder.RenameColumn(
                name: "hash",
                table: "heslo",
                newName: "Hash");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "heslo",
                newName: "Email");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "uzivatel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "uzivatel",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_uzivatel",
                table: "uzivatel",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "sdileni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_hesla = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sdilena_sifra = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sdileni", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sdileni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_uzivatel",
                table: "uzivatel");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "uzivatel");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "uzivatel",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Heslo",
                table: "uzivatel",
                newName: "heslo");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "uzivatel",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Sifra",
                table: "heslo",
                newName: "sifra");

            migrationBuilder.RenameColumn(
                name: "Hash",
                table: "heslo",
                newName: "hash");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "heslo",
                newName: "email");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "uzivatel",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_uzivatel",
                table: "uzivatel",
                column: "email");
        }
    }
}
