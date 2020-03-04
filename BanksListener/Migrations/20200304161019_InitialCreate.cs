using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BanksListener.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KomBankRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bank = table.Column<string>(nullable: true),
                    LastCheck = table.Column<DateTime>(nullable: false),
                    StartedFrom = table.Column<DateTime>(nullable: false),
                    UsdA = table.Column<double>(nullable: false),
                    UsdB = table.Column<double>(nullable: false),
                    EurA = table.Column<double>(nullable: false),
                    EurB = table.Column<double>(nullable: false),
                    RubA = table.Column<double>(nullable: false),
                    RubB = table.Column<double>(nullable: false),
                    EurUsdA = table.Column<double>(nullable: false),
                    EurUsdB = table.Column<double>(nullable: false),
                    RubUsdA = table.Column<double>(nullable: false),
                    RubUsdB = table.Column<double>(nullable: false),
                    RubEurA = table.Column<double>(nullable: false),
                    RubEurB = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KomBankRates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KomBankRates");
        }
    }
}
