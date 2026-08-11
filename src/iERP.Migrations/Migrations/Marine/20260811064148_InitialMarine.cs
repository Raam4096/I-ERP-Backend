using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Marine
{
    /// <inheritdoc />
    public partial class InitialMarine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "marine");

            migrationBuilder.CreateTable(
                name: "port_locations",
                schema: "marine",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_port_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vessels",
                schema: "marine",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vessel_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    imo_number = table.Column<string>(type: "text", nullable: true),
                    vessel_type = table.Column<string>(type: "text", nullable: true),
                    flag_state = table.Column<string>(type: "text", nullable: true),
                    gross_tonnage = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: true),
                    year_built = table.Column<int>(type: "integer", nullable: true),
                    owner = table.Column<string>(type: "text", nullable: true),
                    classification_society = table.Column<string>(type: "text", nullable: true),
                    class_certificate_expiry = table.Column<DateOnly>(type: "date", nullable: true),
                    current_port_location_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("pk_vessels", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_port_locations_tenant_id",
                schema: "marine",
                table: "port_locations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_port_locations_tenant_id_code",
                schema: "marine",
                table: "port_locations",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_port_locations_tenant_id_is_deleted",
                schema: "marine",
                table: "port_locations",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_vessels_tenant_id",
                schema: "marine",
                table: "vessels",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_vessels_tenant_id_is_deleted",
                schema: "marine",
                table: "vessels",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_vessels_tenant_id_vessel_code",
                schema: "marine",
                table: "vessels",
                columns: new[] { "tenant_id", "vessel_code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "port_locations",
                schema: "marine");

            migrationBuilder.DropTable(
                name: "vessels",
                schema: "marine");
        }
    }
}
