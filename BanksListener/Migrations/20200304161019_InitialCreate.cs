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
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Bank = table.Column<string>(nullable: true),
                    LastCheck = table.Column<DateTime>(),
                    StartedFrom = table.Column<DateTime>(),
                    UsdA = table.Column<double>(),
                    UsdB = table.Column<double>(),
                    EurA = table.Column<double>(),
                    EurB = table.Column<double>(),
                    RubA = table.Column<double>(),
                    RubB = table.Column<double>(),
                    EurUsdA = table.Column<double>(),
                    EurUsdB = table.Column<double>(),
                    RubUsdA = table.Column<double>(),
                    RubUsdB = table.Column<double>(),
                    RubEurA = table.Column<double>(),
                    RubEurB = table.Column<double>()
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
