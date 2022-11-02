using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravce_hesel.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hesla",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UzivatelskeID = table.Column<int>(type: "int", nullable: false),
                    Hash = table.Column<int>(type: "int", nullable: false),
                    Sifra = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    desifrovano = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sluzba = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hesla", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Sdilena_hesla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PuvodniHesloID = table.Column<int>(type: "int", nullable: false),
                    ZakladatelID = table.Column<int>(type: "int", nullable: false),
                    ZakladatelJmeno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UzivatelskeID = table.Column<int>(type: "int", nullable: false),
                    Potvrzeno = table.Column<bool>(type: "bit", nullable: false),
                    Sifra = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    desifrovano = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sluzba = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sdilena_hesla", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uzivatele",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Jmeno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Heslo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IV = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzivatele", x => x.Email);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hesla");

            migrationBuilder.DropTable(
                name: "Sdilena_hesla");

            migrationBuilder.DropTable(
                name: "Uzivatele");
        }
    }
}
