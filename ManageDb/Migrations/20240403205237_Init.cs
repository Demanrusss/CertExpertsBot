using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageDb.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechRegs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShortName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechRegs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TNVEDCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TNVEDCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TNVEDCodeTechReg",
                columns: table => new
                {
                    TNVEDCodesId = table.Column<int>(type: "INTEGER", nullable: false),
                    TechRegsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TNVEDCodeTechReg", x => new { x.TNVEDCodesId, x.TechRegsId });
                    table.ForeignKey(
                        name: "FK_TNVEDCodeTechReg_TNVEDCodes_TNVEDCodesId",
                        column: x => x.TNVEDCodesId,
                        principalTable: "TNVEDCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TNVEDCodeTechReg_TechRegs_TechRegsId",
                        column: x => x.TechRegsId,
                        principalTable: "TechRegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TNVEDCodeTechReg_TechRegsId",
                table: "TNVEDCodeTechReg",
                column: "TechRegsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TNVEDCodeTechReg");

            migrationBuilder.DropTable(
                name: "TNVEDCodes");

            migrationBuilder.DropTable(
                name: "TechRegs");
        }
    }
}
