using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BanksListener.Migrations
{
    public partial class BelStockArchiveAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BelStockArchive",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(),
                    Currency = table.Column<int>(),
                    First = table.Column<double>(),
                    Min = table.Column<double>(),
                    Max = table.Column<double>(),
                    Last = table.Column<double>(),
                    TurnoverInByn = table.Column<double>(),
                    TurnoverInCurrency = table.Column<double>(),
                    Count = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BelStockArchive", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BelStockArchive");
        }
    }
}
