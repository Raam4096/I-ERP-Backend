using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Metadata
{
    /// <inheritdoc />
    public partial class AddSectionDescriptionAndSyncLeadLayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "metadata",
                table: "section_definitions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "metadata",
                table: "section_definitions");
        }
    }
}
