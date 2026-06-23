using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleEstoque.Migrations
{
    public partial class AddPrecoUnitarioNaSaida : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitarioNaSaida",
                table: "Saidas",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE "Saidas" s
                SET "PrecoUnitarioNaSaida" = m."PrecoUnitario"
                FROM "Materiais" m
                WHERE s."MaterialId" = m."Id";
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoUnitarioNaSaida",
                table: "Saidas");
        }
    }
}
