using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageDb.Migrations
{
    /// <inheritdoc />
    public partial class AddedDescriptionToTechReg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "TechRegs",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TechRegs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TechRegs");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TechRegs",
                newName: "ShortName");
        }
    }
}
