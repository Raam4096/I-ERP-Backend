using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Bridge
{
    /// <inheritdoc />
    public partial class InitialBridge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bridge");

            migrationBuilder.CreateTable(
                name: "bridge_definitions",
                schema: "bridge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    source_entity_name = table.Column<string>(type: "text", nullable: false),
                    target_entity_name = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("pk_bridge_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bridge_logs",
                schema: "bridge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bridge_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_bridge_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bridge_mappings",
                schema: "bridge",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bridge_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_field = table.Column<string>(type: "text", nullable: false),
                    target_field = table.Column<string>(type: "text", nullable: false),
                    transform_expression = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_bridge_mappings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bridge_definitions_tenant_id",
                schema: "bridge",
                table: "bridge_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_definitions_tenant_id_code",
                schema: "bridge",
                table: "bridge_definitions",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bridge_definitions_tenant_id_is_deleted",
                schema: "bridge",
                table: "bridge_definitions",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_bridge_logs_tenant_id",
                schema: "bridge",
                table: "bridge_logs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_logs_tenant_id_bridge_definition_id_source_record_id",
                schema: "bridge",
                table: "bridge_logs",
                columns: new[] { "tenant_id", "bridge_definition_id", "source_record_id" });

            migrationBuilder.CreateIndex(
                name: "ix_bridge_logs_tenant_id_is_deleted",
                schema: "bridge",
                table: "bridge_logs",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_bridge_mappings_tenant_id",
                schema: "bridge",
                table: "bridge_mappings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_bridge_mappings_tenant_id_bridge_definition_id_source_field",
                schema: "bridge",
                table: "bridge_mappings",
                columns: new[] { "tenant_id", "bridge_definition_id", "source_field" });

            migrationBuilder.CreateIndex(
                name: "ix_bridge_mappings_tenant_id_is_deleted",
                schema: "bridge",
                table: "bridge_mappings",
                columns: new[] { "tenant_id", "is_deleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bridge_definitions",
                schema: "bridge");

            migrationBuilder.DropTable(
                name: "bridge_logs",
                schema: "bridge");

            migrationBuilder.DropTable(
                name: "bridge_mappings",
                schema: "bridge");
        }
    }
}
