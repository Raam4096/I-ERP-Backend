using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Metadata
{
    /// <inheritdoc />
    public partial class InitialMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "metadata");

            migrationBuilder.CreateTable(
                name: "custom_field_definitions",
                schema: "metadata",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_name = table.Column<string>(type: "text", nullable: false),
                    field_key = table.Column<string>(type: "text", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false),
                    data_type = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_custom_field_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "custom_field_values",
                schema: "metadata",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_name = table.Column<string>(type: "text", nullable: false),
                    record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_key = table.Column<string>(type: "text", nullable: false),
                    value_text = table.Column<string>(type: "text", nullable: true),
                    value_number = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: true),
                    value_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    value_boolean = table.Column<bool>(type: "boolean", nullable: true),
                    value_json = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("pk_custom_field_values", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "module_definitions",
                schema: "metadata",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_module_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "screen_definitions",
                schema: "metadata",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    route = table.Column<string>(type: "text", nullable: false),
                    render_mode = table.Column<string>(type: "text", nullable: false),
                    entity_name = table.Column<string>(type: "text", nullable: false),
                    api_base_path = table.Column<string>(type: "text", nullable: false),
                    workflow_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    print_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    ai_enabled = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_screen_definitions", x => x.id);
                    table.ForeignKey(
                        name: "fk_screen_definitions_module_definitions_module_definition_id",
                        column: x => x.module_definition_id,
                        principalSchema: "metadata",
                        principalTable: "module_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "section_definitions",
                schema: "metadata",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    screen_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_section_definitions", x => x.id);
                    table.ForeignKey(
                        name: "fk_section_definitions_screen_definitions_screen_definition_id",
                        column: x => x.screen_definition_id,
                        principalSchema: "metadata",
                        principalTable: "screen_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "field_definitions",
                schema: "metadata",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    section_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_key = table.Column<string>(type: "text", nullable: false),
                    label = table.Column<string>(type: "text", nullable: false),
                    data_type = table.Column<string>(type: "text", nullable: false),
                    control_type = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    is_read_only = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("pk_field_definitions", x => x.id);
                    table.ForeignKey(
                        name: "fk_field_definitions_section_definitions_section_definition_id",
                        column: x => x.section_definition_id,
                        principalSchema: "metadata",
                        principalTable: "section_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_custom_field_definitions_tenant_id",
                schema: "metadata",
                table: "custom_field_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_custom_field_definitions_tenant_id_entity_name_field_key",
                schema: "metadata",
                table: "custom_field_definitions",
                columns: new[] { "tenant_id", "entity_name", "field_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_custom_field_definitions_tenant_id_is_deleted",
                schema: "metadata",
                table: "custom_field_definitions",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_custom_field_values_tenant_id",
                schema: "metadata",
                table: "custom_field_values",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_custom_field_values_tenant_id_entity_name_record_id_field_k~",
                schema: "metadata",
                table: "custom_field_values",
                columns: new[] { "tenant_id", "entity_name", "record_id", "field_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_custom_field_values_tenant_id_is_deleted",
                schema: "metadata",
                table: "custom_field_values",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_field_definitions_section_definition_id",
                schema: "metadata",
                table: "field_definitions",
                column: "section_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_field_definitions_tenant_id",
                schema: "metadata",
                table: "field_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_field_definitions_tenant_id_is_deleted",
                schema: "metadata",
                table: "field_definitions",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_module_definitions_tenant_id",
                schema: "metadata",
                table: "module_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_module_definitions_tenant_id_code",
                schema: "metadata",
                table: "module_definitions",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_module_definitions_tenant_id_is_deleted",
                schema: "metadata",
                table: "module_definitions",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_screen_definitions_module_definition_id",
                schema: "metadata",
                table: "screen_definitions",
                column: "module_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_screen_definitions_tenant_id",
                schema: "metadata",
                table: "screen_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_screen_definitions_tenant_id_code",
                schema: "metadata",
                table: "screen_definitions",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_screen_definitions_tenant_id_is_deleted",
                schema: "metadata",
                table: "screen_definitions",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_section_definitions_screen_definition_id",
                schema: "metadata",
                table: "section_definitions",
                column: "screen_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_section_definitions_tenant_id",
                schema: "metadata",
                table: "section_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_section_definitions_tenant_id_is_deleted",
                schema: "metadata",
                table: "section_definitions",
                columns: new[] { "tenant_id", "is_deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "custom_field_definitions",
                schema: "metadata");

            migrationBuilder.DropTable(
                name: "custom_field_values",
                schema: "metadata");

            migrationBuilder.DropTable(
                name: "field_definitions",
                schema: "metadata");

            migrationBuilder.DropTable(
                name: "section_definitions",
                schema: "metadata");

            migrationBuilder.DropTable(
                name: "screen_definitions",
                schema: "metadata");

            migrationBuilder.DropTable(
                name: "module_definitions",
                schema: "metadata");
        }
    }
}
