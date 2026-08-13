using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Crm
{
    /// <inheritdoc />
    public partial class AddLeadManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_leads_tenant_id_subsidiary_id_lead_number",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "currency_code",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "last_name",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "rating",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "source",
                schema: "crm",
                table: "leads");

            migrationBuilder.RenameColumn(
                name: "owner_user_id",
                schema: "crm",
                table: "leads",
                newName: "assigned_to_user_id");

            migrationBuilder.RenameColumn(
                name: "estimated_value",
                schema: "crm",
                table: "leads",
                newName: "annual_revenue");

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "crm",
                table: "leads",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "crm",
                table: "leads",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                schema: "crm",
                table: "leads",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "lead_number",
                schema: "crm",
                table: "leads",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "crm",
                table: "leads",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "company_name",
                schema: "crm",
                table: "leads",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address",
                schema: "crm",
                table: "leads",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "company_size",
                schema: "crm",
                table: "leads",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person",
                schema: "crm",
                table: "leads",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "industry",
                schema: "crm",
                table: "leads",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lead_source",
                schema: "crm",
                table: "leads",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "crm",
                table: "leads",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "project_description",
                schema: "crm",
                table: "leads",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "project_type",
                schema: "crm",
                table: "leads",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subsidiary",
                schema: "crm",
                table: "leads",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "website",
                schema: "crm",
                table: "leads",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "lead_followups",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lead_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("pk_lead_followups", x => x.id);
                    table.ForeignKey(
                        name: "fk_lead_followups_leads_lead_id",
                        column: x => x.lead_id,
                        principalSchema: "crm",
                        principalTable: "leads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lead_attachments",
                schema: "crm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    follow_up_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    file_path = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    content_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lead_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_lead_attachments_lead_followups_follow_up_id",
                        column: x => x.follow_up_id,
                        principalSchema: "crm",
                        principalTable: "lead_followups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_leads_tenant_id_assigned_to_user_id",
                schema: "crm",
                table: "leads",
                columns: new[] { "tenant_id", "assigned_to_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_leads_tenant_id_email",
                schema: "crm",
                table: "leads",
                columns: new[] { "tenant_id", "email" });

            migrationBuilder.CreateIndex(
                name: "ix_leads_tenant_id_lead_number",
                schema: "crm",
                table: "leads",
                columns: new[] { "tenant_id", "lead_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_leads_tenant_id_phone",
                schema: "crm",
                table: "leads",
                columns: new[] { "tenant_id", "phone" });

            migrationBuilder.CreateIndex(
                name: "ix_leads_tenant_id_status",
                schema: "crm",
                table: "leads",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_lead_attachments_follow_up_id",
                schema: "crm",
                table: "lead_attachments",
                column: "follow_up_id");

            migrationBuilder.CreateIndex(
                name: "ix_lead_followups_lead_id",
                schema: "crm",
                table: "lead_followups",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_lead_followups_tenant_id",
                schema: "crm",
                table: "lead_followups",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_lead_followups_tenant_id_is_deleted",
                schema: "crm",
                table: "lead_followups",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_lead_followups_tenant_id_lead_id_follow_up_date",
                schema: "crm",
                table: "lead_followups",
                columns: new[] { "tenant_id", "lead_id", "follow_up_date" });

            migrationBuilder.CreateIndex(
                name: "ix_lead_followups_tenant_id_status",
                schema: "crm",
                table: "lead_followups",
                columns: new[] { "tenant_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lead_attachments",
                schema: "crm");

            migrationBuilder.DropTable(
                name: "lead_followups",
                schema: "crm");

            migrationBuilder.DropIndex(
                name: "ix_leads_tenant_id_assigned_to_user_id",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_tenant_id_email",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_tenant_id_lead_number",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_tenant_id_phone",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "ix_leads_tenant_id_status",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "address",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "company_size",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "contact_person",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "industry",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "lead_source",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "project_description",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "project_type",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "subsidiary",
                schema: "crm",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "website",
                schema: "crm",
                table: "leads");

            migrationBuilder.RenameColumn(
                name: "assigned_to_user_id",
                schema: "crm",
                table: "leads",
                newName: "owner_user_id");

            migrationBuilder.RenameColumn(
                name: "annual_revenue",
                schema: "crm",
                table: "leads",
                newName: "estimated_value");

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "crm",
                table: "leads",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "lead_number",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "company_name",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "currency_code",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rating",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source",
                schema: "crm",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_leads_tenant_id_subsidiary_id_lead_number",
                schema: "crm",
                table: "leads",
                columns: new[] { "tenant_id", "subsidiary_id", "lead_number" },
                unique: true);
        }
    }
}
