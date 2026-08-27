using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Metadata
{
    /// <inheritdoc />
    public partial class AddUserFieldPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_field_preferences",
                schema: "metadata",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    screen_code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    field_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_user_field_preferences", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_field_preferences_tenant_id",
                schema: "metadata",
                table: "user_field_preferences",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_field_preferences_tenant_id_is_deleted",
                schema: "metadata",
                table: "user_field_preferences",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_user_field_preferences_tenant_id_user_id_screen_code",
                schema: "metadata",
                table: "user_field_preferences",
                columns: new[] { "tenant_id", "user_id", "screen_code" });

            migrationBuilder.CreateIndex(
                name: "ix_user_field_preferences_tenant_id_user_id_screen_code_field_~",
                schema: "metadata",
                table: "user_field_preferences",
                columns: new[] { "tenant_id", "user_id", "screen_code", "field_key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_field_preferences",
                schema: "metadata");
        }
    }
}
