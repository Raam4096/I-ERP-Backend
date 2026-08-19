using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Crm
{
    /// <inheritdoc />
    public partial class AddOpportunityManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_opportunities_tenant_id_subsidiary_id_opportunity_number",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.RenameColumn(
                name: "amount",
                schema: "crm",
                table: "opportunities",
                newName: "opportunity_value");

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "crm",
                table: "opportunities",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "crm",
                table: "opportunities",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "stage",
                schema: "crm",
                table: "opportunities",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "opportunity_number",
                schema: "crm",
                table: "opportunities",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "crm",
                table: "opportunities",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "currency_code",
                schema: "crm",
                table: "opportunities",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.Sql(
                """
                UPDATE crm.opportunities SET opportunity_value = 0 WHERE opportunity_value IS NULL;
                """);

            migrationBuilder.AlterColumn<decimal>(
                name: "opportunity_value",
                schema: "crm",
                table: "opportunities",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)",
                oldPrecision: 19,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "closed_reason",
                schema: "crm",
                table: "opportunities",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "computations",
                schema: "crm",
                table: "opportunities",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "crm",
                table: "opportunities",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "probability",
                schema: "crm",
                table: "opportunities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "status_before_discard",
                schema: "crm",
                table: "opportunities",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "opportunity_followups",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    opportunity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    activity_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    follow_up_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    next_follow_up_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    remarks = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
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
                    table.PrimaryKey("pk_opportunity_followups", x => x.id);
                    table.ForeignKey(
                        name: "fk_opportunity_followups_opportunities_opportunity_id",
                        column: x => x.opportunity_id,
                        principalSchema: "crm",
                        principalTable: "opportunities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_opportunities_lead_id",
                schema: "crm",
                table: "opportunities",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_opportunities_tenant_id_lead_id",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "tenant_id", "lead_id" });

            migrationBuilder.CreateIndex(
                name: "ix_opportunities_tenant_id_opportunity_number",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "tenant_id", "opportunity_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_opportunities_tenant_id_status",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_opportunity_followups_opportunity_id",
                schema: "crm",
                table: "opportunity_followups",
                column: "opportunity_id");

            migrationBuilder.CreateIndex(
                name: "ix_opportunity_followups_tenant_id",
                schema: "crm",
                table: "opportunity_followups",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_opportunity_followups_tenant_id_is_deleted",
                schema: "crm",
                table: "opportunity_followups",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_opportunity_followups_tenant_id_opportunity_id_follow_up_da~",
                schema: "crm",
                table: "opportunity_followups",
                columns: new[] { "tenant_id", "opportunity_id", "follow_up_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "opportunity_followups",
                schema: "crm");

            migrationBuilder.DropIndex(
                name: "ix_opportunities_lead_id",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropIndex(
                name: "ix_opportunities_tenant_id_lead_id",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropIndex(
                name: "ix_opportunities_tenant_id_opportunity_number",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropIndex(
                name: "ix_opportunities_tenant_id_status",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropColumn(
                name: "closed_reason",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropColumn(
                name: "computations",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropColumn(
                name: "probability",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.DropColumn(
                name: "status_before_discard",
                schema: "crm",
                table: "opportunities");

            migrationBuilder.RenameColumn(
                name: "opportunity_value",
                schema: "crm",
                table: "opportunities",
                newName: "amount");

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                schema: "crm",
                table: "opportunities",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)",
                oldPrecision: 19,
                oldScale: 4);

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "crm",
                table: "opportunities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "crm",
                table: "opportunities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "stage",
                schema: "crm",
                table: "opportunities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "opportunity_number",
                schema: "crm",
                table: "opportunities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "crm",
                table: "opportunities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "currency_code",
                schema: "crm",
                table: "opportunities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_opportunities_tenant_id_subsidiary_id_opportunity_number",
                schema: "crm",
                table: "opportunities",
                columns: new[] { "tenant_id", "subsidiary_id", "opportunity_number" },
                unique: true);
        }
    }
}
