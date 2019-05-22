using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiMultas.Data.Migrations
{
    public partial class InitMultas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agentes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(maxLength: 40, nullable: false),
                    Fotografia = table.Column<string>(nullable: true),
                    Esquadra = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agentes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Condutores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(nullable: true),
                    BI = table.Column<string>(nullable: true),
                    Telemovel = table.Column<string>(nullable: true),
                    DataNascimento = table.Column<DateTime>(nullable: false),
                    NumCartaConducao = table.Column<string>(nullable: true),
                    LocalEmissao = table.Column<string>(nullable: true),
                    DataValidadeCarta = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condutores", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Viaturas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Matricula = table.Column<string>(nullable: true),
                    Marca = table.Column<string>(nullable: true),
                    Modelo = table.Column<string>(nullable: true),
                    Cor = table.Column<string>(nullable: true),
                    NomeDono = table.Column<string>(nullable: true),
                    MoradaDono = table.Column<string>(nullable: true),
                    CodPostalDono = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viaturas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Multas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Infracao = table.Column<string>(nullable: true),
                    LocalDaMulta = table.Column<string>(nullable: true),
                    ValorMulta = table.Column<decimal>(nullable: false),
                    DataDaMulta = table.Column<DateTime>(nullable: false),
                    AgenteFK = table.Column<int>(nullable: false),
                    CondutorFK = table.Column<int>(nullable: false),
                    ViaturaFK = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Multas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Multas_Agentes_AgenteFK",
                        column: x => x.AgenteFK,
                        principalTable: "Agentes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Multas_Condutores_CondutorFK",
                        column: x => x.CondutorFK,
                        principalTable: "Condutores",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Multas_Viaturas_ViaturaFK",
                        column: x => x.ViaturaFK,
                        principalTable: "Viaturas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Multas_AgenteFK",
                table: "Multas",
                column: "AgenteFK");

            migrationBuilder.CreateIndex(
                name: "IX_Multas_CondutorFK",
                table: "Multas",
                column: "CondutorFK");

            migrationBuilder.CreateIndex(
                name: "IX_Multas_ViaturaFK",
                table: "Multas",
                column: "ViaturaFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Multas");

            migrationBuilder.DropTable(
                name: "Agentes");

            migrationBuilder.DropTable(
                name: "Condutores");

            migrationBuilder.DropTable(
                name: "Viaturas");
        }
    }
}
