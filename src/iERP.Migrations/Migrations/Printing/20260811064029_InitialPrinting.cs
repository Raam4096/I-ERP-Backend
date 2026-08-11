using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Printing
{
    /// <inheritdoc />
    public partial class InitialPrinting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "printing");

            migrationBuilder.CreateTable(
                name: "print_template_versions",
                schema: "printing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    print_template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_version_number = table.Column<int>(type: "integer", nullable: false),
                    template_content = table.Column<string>(type: "text", nullable: false),
                    output_type = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_print_template_versions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "print_templates",
                schema: "printing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_name = table.Column<string>(type: "text", nullable: false),
                    template_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_print_templates", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_print_template_versions_tenant_id",
                schema: "printing",
                table: "print_template_versions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_print_template_versions_tenant_id_is_deleted",
                schema: "printing",
                table: "print_template_versions",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_print_template_versions_tenant_id_print_template_id_templat~",
                schema: "printing",
                table: "print_template_versions",
                columns: new[] { "tenant_id", "print_template_id", "template_version_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_print_templates_tenant_id",
                schema: "printing",
                table: "print_templates",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_print_templates_tenant_id_entity_name_template_code",
                schema: "printing",
                table: "print_templates",
                columns: new[] { "tenant_id", "entity_name", "template_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_print_templates_tenant_id_is_deleted",
                schema: "printing",
                table: "print_templates",
                columns: new[] { "tenant_id", "is_deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "print_template_versions",
                schema: "printing");

            migrationBuilder.DropTable(
                name: "print_templates",
                schema: "printing");
        }
    }
}
