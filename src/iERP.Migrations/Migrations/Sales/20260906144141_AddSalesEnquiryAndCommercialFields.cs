using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Sales
{
    /// <inheritdoc />
    public partial class AddSalesEnquiryAndCommercialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sales_quotations_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropIndex(
                name: "ix_sales_quotation_lines_tenant_id_id",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropIndex(
                name: "ix_sales_invoices_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropIndex(
                name: "ix_sales_invoice_lines_tenant_id_id",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.RenameColumn(
                name: "document_no",
                schema: "sales",
                table: "sales_quotations",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "currency_code",
                schema: "sales",
                table: "sales_quotations",
                newName: "quotation_code");

            migrationBuilder.RenameColumn(
                name: "document_no",
                schema: "sales",
                table: "sales_invoices",
                newName: "invoice_code");

            migrationBuilder.RenameColumn(
                name: "currency_code",
                schema: "sales",
                table: "sales_invoices",
                newName: "customer");

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "sales_quotations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "sales_quotations",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "sales_quotations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customer",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "delivery",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "prices",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "projected_total",
                schema: "sales",
                table: "sales_quotations",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sales_rep",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subsidiary",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "valid_until",
                schema: "sales",
                table: "sales_quotations",
                type: "date",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "category",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country_of_origin",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "gross_amount",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "hs_code",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "price_level",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "rate",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "tax_code",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "uom",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "sales_invoices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "sales_invoices",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "sales_invoices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "department",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "due_date",
                schema: "sales",
                table: "sales_invoices",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "projected_total",
                schema: "sales",
                table: "sales_invoices",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sales_rep",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subsidiary",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "category",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country_of_origin",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "gross_amount",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "hs_code",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "price_level",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "rate",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "tax_code",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "uom",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sales_enquiries",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    enquiry_code = table.Column<string>(type: "text", nullable: false),
                    customer = table.Column<string>(type: "text", nullable: false),
                    contact_person = table.Column<string>(type: "text", nullable: true),
                    opportunity = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    alt_phone = table.Column<string>(type: "text", nullable: true),
                    probability = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    expected_close = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    win_loss_reason = table.Column<string>(type: "text", nullable: true),
                    projected_total = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: true),
                    forecast_type = table.Column<string>(type: "text", nullable: true),
                    weighted_total = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: true),
                    range = table.Column<string>(type: "text", nullable: true),
                    subsidiary = table.Column<string>(type: "text", nullable: true),
                    class_name = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<string>(type: "text", nullable: true),
                    department = table.Column<string>(type: "text", nullable: true),
                    sales_rep = table.Column<string>(type: "text", nullable: true),
                    last_sales_activity = table.Column<DateOnly>(type: "date", nullable: true),
                    currency = table.Column<string>(type: "text", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric(19,8)", precision: 19, scale: 8, nullable: false),
                    payment = table.Column<string>(type: "text", nullable: true),
                    prices = table.Column<string>(type: "text", nullable: true),
                    delivery = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_sales_enquiries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sales_enquiry_follow_ups",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sales_enquiry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    follow_up_date = table.Column<DateOnly>(type: "date", nullable: true),
                    next_follow_up_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    activity_type = table.Column<string>(type: "text", nullable: true),
                    remarks = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_sales_enquiry_follow_ups", x => x.id);
                    table.ForeignKey(
                        name: "fk_sales_enquiry_follow_ups_sales_enquiries_sales_enquiry_id",
                        column: x => x.sales_enquiry_id,
                        principalSchema: "sales",
                        principalTable: "sales_enquiries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sales_enquiry_lines",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sales_enquiry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_no = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "text", nullable: true),
                    item = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(19,6)", precision: 19, scale: 6, nullable: false),
                    uom = table.Column<string>(type: "text", nullable: true),
                    price_level = table.Column<string>(type: "text", nullable: true),
                    rate = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    discount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    tax_code = table.Column<string>(type: "text", nullable: true),
                    gross_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    class_name = table.Column<string>(type: "text", nullable: true),
                    country_of_origin = table.Column<string>(type: "text", nullable: true),
                    hs_code = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_sales_enquiry_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_sales_enquiry_lines_sales_enquiries_sales_enquiry_id",
                        column: x => x.sales_enquiry_id,
                        principalSchema: "sales",
                        principalTable: "sales_enquiries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotations_tenant_id_quotation_code",
                schema: "sales",
                table: "sales_quotations",
                columns: new[] { "tenant_id", "quotation_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotation_lines_sales_quotation_id",
                schema: "sales",
                table: "sales_quotation_lines",
                column: "sales_quotation_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotation_lines_tenant_id_sales_quotation_id_line_no",
                schema: "sales",
                table: "sales_quotation_lines",
                columns: new[] { "tenant_id", "sales_quotation_id", "line_no" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_invoices_tenant_id_invoice_code",
                schema: "sales",
                table: "sales_invoices",
                columns: new[] { "tenant_id", "invoice_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_invoice_lines_sales_invoice_id",
                schema: "sales",
                table: "sales_invoice_lines",
                column: "sales_invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_invoice_lines_tenant_id_sales_invoice_id_line_no",
                schema: "sales",
                table: "sales_invoice_lines",
                columns: new[] { "tenant_id", "sales_invoice_id", "line_no" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiries_tenant_id",
                schema: "sales",
                table: "sales_enquiries",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiries_tenant_id_enquiry_code",
                schema: "sales",
                table: "sales_enquiries",
                columns: new[] { "tenant_id", "enquiry_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiries_tenant_id_is_deleted",
                schema: "sales",
                table: "sales_enquiries",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_follow_ups_sales_enquiry_id",
                schema: "sales",
                table: "sales_enquiry_follow_ups",
                column: "sales_enquiry_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_follow_ups_tenant_id",
                schema: "sales",
                table: "sales_enquiry_follow_ups",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_follow_ups_tenant_id_is_deleted",
                schema: "sales",
                table: "sales_enquiry_follow_ups",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_follow_ups_tenant_id_sales_enquiry_id",
                schema: "sales",
                table: "sales_enquiry_follow_ups",
                columns: new[] { "tenant_id", "sales_enquiry_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_lines_sales_enquiry_id",
                schema: "sales",
                table: "sales_enquiry_lines",
                column: "sales_enquiry_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_lines_tenant_id",
                schema: "sales",
                table: "sales_enquiry_lines",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_lines_tenant_id_is_deleted",
                schema: "sales",
                table: "sales_enquiry_lines",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_enquiry_lines_tenant_id_sales_enquiry_id_line_no",
                schema: "sales",
                table: "sales_enquiry_lines",
                columns: new[] { "tenant_id", "sales_enquiry_id", "line_no" });

            migrationBuilder.AddForeignKey(
                name: "fk_sales_invoice_lines_sales_invoices_sales_invoice_id",
                schema: "sales",
                table: "sales_invoice_lines",
                column: "sales_invoice_id",
                principalSchema: "sales",
                principalTable: "sales_invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_sales_quotation_lines_sales_quotations_sales_quotation_id",
                schema: "sales",
                table: "sales_quotation_lines",
                column: "sales_quotation_id",
                principalSchema: "sales",
                principalTable: "sales_quotations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sales_invoice_lines_sales_invoices_sales_invoice_id",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_sales_quotation_lines_sales_quotations_sales_quotation_id",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropTable(
                name: "sales_enquiry_follow_ups",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "sales_enquiry_lines",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "sales_enquiries",
                schema: "sales");

            migrationBuilder.DropIndex(
                name: "ix_sales_quotations_tenant_id_quotation_code",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropIndex(
                name: "ix_sales_quotation_lines_sales_quotation_id",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropIndex(
                name: "ix_sales_quotation_lines_tenant_id_sales_quotation_id_line_no",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropIndex(
                name: "ix_sales_invoices_tenant_id_invoice_code",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropIndex(
                name: "ix_sales_invoice_lines_sales_invoice_id",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropIndex(
                name: "ix_sales_invoice_lines_tenant_id_sales_invoice_id_line_no",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "contact_person",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "currency",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "customer",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "delivery",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "department",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "location",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "payment",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "phone",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "prices",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "projected_total",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "sales_rep",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "subsidiary",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "valid_until",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "amount",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "category",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "country_of_origin",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "discount",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "gross_amount",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "hs_code",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "item",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "price_level",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "rate",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "tax_code",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "uom",
                schema: "sales",
                table: "sales_quotation_lines");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "contact_person",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "currency",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "department",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "due_date",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "location",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "payment",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "phone",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "projected_total",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "sales_rep",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "subsidiary",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "title",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "amount",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "category",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "country_of_origin",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "discount",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "gross_amount",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "hs_code",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "item",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "price_level",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "rate",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "tax_code",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.DropColumn(
                name: "uom",
                schema: "sales",
                table: "sales_invoice_lines");

            migrationBuilder.RenameColumn(
                name: "title",
                schema: "sales",
                table: "sales_quotations",
                newName: "document_no");

            migrationBuilder.RenameColumn(
                name: "quotation_code",
                schema: "sales",
                table: "sales_quotations",
                newName: "currency_code");

            migrationBuilder.RenameColumn(
                name: "invoice_code",
                schema: "sales",
                table: "sales_invoices",
                newName: "document_no");

            migrationBuilder.RenameColumn(
                name: "customer",
                schema: "sales",
                table: "sales_invoices",
                newName: "currency_code");

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "sales_quotations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "sales_quotations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "sales_quotations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "sales_quotation_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "sales_invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "sales_invoices",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "sales_invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "sales_invoice_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotations_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "sales_quotations",
                columns: new[] { "tenant_id", "subsidiary_id", "document_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_quotation_lines_tenant_id_id",
                schema: "sales",
                table: "sales_quotation_lines",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_invoices_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "sales_invoices",
                columns: new[] { "tenant_id", "subsidiary_id", "document_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_invoice_lines_tenant_id_id",
                schema: "sales",
                table: "sales_invoice_lines",
                columns: new[] { "tenant_id", "id" });
        }
    }
}
