using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iERP.Migrations.Migrations.Sales
{
    /// <inheritdoc />
    public partial class AddSalesOrderAndDebitCreditNoteCommercialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sales_orders_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropIndex(
                name: "ix_sales_order_lines_tenant_id_id",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropIndex(
                name: "ix_credit_notes_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropIndex(
                name: "ix_credit_note_lines_tenant_id_id",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.RenameColumn(
                name: "document_no",
                schema: "sales",
                table: "sales_orders",
                newName: "order_code");

            migrationBuilder.RenameColumn(
                name: "currency_code",
                schema: "sales",
                table: "sales_orders",
                newName: "customer");

            migrationBuilder.RenameColumn(
                name: "document_no",
                schema: "sales",
                table: "credit_notes",
                newName: "note_code");

            migrationBuilder.RenameColumn(
                name: "currency_code",
                schema: "sales",
                table: "credit_notes",
                newName: "customer");

            migrationBuilder.AddColumn<string>(
                name: "source_enquiry_code",
                schema: "sales",
                table: "sales_quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_enquiry_id",
                schema: "sales",
                table: "sales_quotations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "sales_orders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "sales_orders",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "sales_orders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "delivery",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "delivery_date",
                schema: "sales",
                table: "sales_orders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "prices",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "projected_total",
                schema: "sales",
                table: "sales_orders",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sales_rep",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_enquiry_code",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_enquiry_id",
                schema: "sales",
                table: "sales_orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_quotation_code",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_quotation_id",
                schema: "sales",
                table: "sales_orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subsidiary",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                schema: "sales",
                table: "sales_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "sales_order_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "sales_order_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                schema: "sales",
                table: "sales_order_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "category",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country_of_origin",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                schema: "sales",
                table: "sales_order_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "gross_amount",
                schema: "sales",
                table: "sales_order_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "hs_code",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "price_level",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "rate",
                schema: "sales",
                table: "sales_order_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "tax_code",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "uom",
                schema: "sales",
                table: "sales_order_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_order_code",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_order_id",
                schema: "sales",
                table: "sales_invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_quotation_code",
                schema: "sales",
                table: "sales_invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_quotation_id",
                schema: "sales",
                table: "sales_invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "credit_notes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "credit_notes",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "credit_notes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "department",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "due_date",
                schema: "sales",
                table: "credit_notes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "projected_total",
                schema: "sales",
                table: "credit_notes",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sales_rep",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_invoice_code",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_invoice_id",
                schema: "sales",
                table: "credit_notes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "subsidiary",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                schema: "sales",
                table: "credit_notes",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "credit_note_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "credit_note_lines",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                schema: "sales",
                table: "credit_note_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "category",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "class_name",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "country_of_origin",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                schema: "sales",
                table: "credit_note_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "gross_amount",
                schema: "sales",
                table: "credit_note_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "hs_code",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "price_level",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "rate",
                schema: "sales",
                table: "credit_note_lines",
                type: "numeric(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "tax_code",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "uom",
                schema: "sales",
                table: "credit_note_lines",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "debit_notes",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    note_code = table.Column<string>(type: "text", nullable: false),
                    customer = table.Column<string>(type: "text", nullable: false),
                    contact_person = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    document_date = table.Column<DateOnly>(type: "date", nullable: true),
                    due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    subsidiary = table.Column<string>(type: "text", nullable: true),
                    class_name = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<string>(type: "text", nullable: true),
                    department = table.Column<string>(type: "text", nullable: true),
                    sales_rep = table.Column<string>(type: "text", nullable: true),
                    currency = table.Column<string>(type: "text", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric(19,8)", precision: 19, scale: 8, nullable: false),
                    projected_total = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: true),
                    payment = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    source_invoice_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_invoice_code = table.Column<string>(type: "text", nullable: true),
                    subsidiary_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
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
                    table.PrimaryKey("pk_debit_notes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "debit_note_lines",
                schema: "sales",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    debit_note_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    uom_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    discount_percent = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    tax_code_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tax_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
                    line_amount = table.Column<decimal>(type: "numeric(19,4)", precision: 19, scale: 4, nullable: false),
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
                    table.PrimaryKey("pk_debit_note_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_debit_note_lines_debit_notes_debit_note_id",
                        column: x => x.debit_note_id,
                        principalSchema: "sales",
                        principalTable: "debit_notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_tenant_id_order_code",
                schema: "sales",
                table: "sales_orders",
                columns: new[] { "tenant_id", "order_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_tenant_id_source_enquiry_id",
                schema: "sales",
                table: "sales_orders",
                columns: new[] { "tenant_id", "source_enquiry_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_tenant_id_source_quotation_id",
                schema: "sales",
                table: "sales_orders",
                columns: new[] { "tenant_id", "source_quotation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_lines_sales_order_id",
                schema: "sales",
                table: "sales_order_lines",
                column: "sales_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_lines_tenant_id_sales_order_id_line_no",
                schema: "sales",
                table: "sales_order_lines",
                columns: new[] { "tenant_id", "sales_order_id", "line_no" });

            migrationBuilder.CreateIndex(
                name: "ix_credit_notes_tenant_id_note_code",
                schema: "sales",
                table: "credit_notes",
                columns: new[] { "tenant_id", "note_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credit_notes_tenant_id_source_invoice_id",
                schema: "sales",
                table: "credit_notes",
                columns: new[] { "tenant_id", "source_invoice_id" });

            migrationBuilder.CreateIndex(
                name: "ix_credit_note_lines_credit_note_id",
                schema: "sales",
                table: "credit_note_lines",
                column: "credit_note_id");

            migrationBuilder.CreateIndex(
                name: "ix_credit_note_lines_tenant_id_credit_note_id_line_no",
                schema: "sales",
                table: "credit_note_lines",
                columns: new[] { "tenant_id", "credit_note_id", "line_no" });

            migrationBuilder.CreateIndex(
                name: "ix_debit_note_lines_debit_note_id",
                schema: "sales",
                table: "debit_note_lines",
                column: "debit_note_id");

            migrationBuilder.CreateIndex(
                name: "ix_debit_note_lines_tenant_id",
                schema: "sales",
                table: "debit_note_lines",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_debit_note_lines_tenant_id_debit_note_id_line_no",
                schema: "sales",
                table: "debit_note_lines",
                columns: new[] { "tenant_id", "debit_note_id", "line_no" });

            migrationBuilder.CreateIndex(
                name: "ix_debit_note_lines_tenant_id_is_deleted",
                schema: "sales",
                table: "debit_note_lines",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_debit_notes_tenant_id",
                schema: "sales",
                table: "debit_notes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_debit_notes_tenant_id_is_deleted",
                schema: "sales",
                table: "debit_notes",
                columns: new[] { "tenant_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_debit_notes_tenant_id_note_code",
                schema: "sales",
                table: "debit_notes",
                columns: new[] { "tenant_id", "note_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_debit_notes_tenant_id_source_invoice_id",
                schema: "sales",
                table: "debit_notes",
                columns: new[] { "tenant_id", "source_invoice_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_credit_note_lines_credit_notes_credit_note_id",
                schema: "sales",
                table: "credit_note_lines",
                column: "credit_note_id",
                principalSchema: "sales",
                principalTable: "credit_notes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_sales_order_lines_sales_orders_sales_order_id",
                schema: "sales",
                table: "sales_order_lines",
                column: "sales_order_id",
                principalSchema: "sales",
                principalTable: "sales_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_credit_note_lines_credit_notes_credit_note_id",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_sales_order_lines_sales_orders_sales_order_id",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropTable(
                name: "debit_note_lines",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "debit_notes",
                schema: "sales");

            migrationBuilder.DropIndex(
                name: "ix_sales_orders_tenant_id_order_code",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropIndex(
                name: "ix_sales_orders_tenant_id_source_enquiry_id",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropIndex(
                name: "ix_sales_orders_tenant_id_source_quotation_id",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropIndex(
                name: "ix_sales_order_lines_sales_order_id",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropIndex(
                name: "ix_sales_order_lines_tenant_id_sales_order_id_line_no",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropIndex(
                name: "ix_credit_notes_tenant_id_note_code",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropIndex(
                name: "ix_credit_notes_tenant_id_source_invoice_id",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropIndex(
                name: "ix_credit_note_lines_credit_note_id",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropIndex(
                name: "ix_credit_note_lines_tenant_id_credit_note_id_line_no",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "source_enquiry_code",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "source_enquiry_id",
                schema: "sales",
                table: "sales_quotations");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "contact_person",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "currency",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "delivery",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "delivery_date",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "department",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "location",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "payment",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "phone",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "prices",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "projected_total",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "sales_rep",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "source_enquiry_code",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "source_enquiry_id",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "source_quotation_code",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "source_quotation_id",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "subsidiary",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "title",
                schema: "sales",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "amount",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "category",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "country_of_origin",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "discount",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "gross_amount",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "hs_code",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "item",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "price_level",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "rate",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "tax_code",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "uom",
                schema: "sales",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "source_order_code",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "source_order_id",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "source_quotation_code",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "source_quotation_id",
                schema: "sales",
                table: "sales_invoices");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "contact_person",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "currency",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "department",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "due_date",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "location",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "payment",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "phone",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "projected_total",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "reason",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "sales_rep",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "source_invoice_code",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "source_invoice_id",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "subsidiary",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "title",
                schema: "sales",
                table: "credit_notes");

            migrationBuilder.DropColumn(
                name: "amount",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "category",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "class_name",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "country_of_origin",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "discount",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "gross_amount",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "hs_code",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "item",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "price_level",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "rate",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "tax_code",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.DropColumn(
                name: "uom",
                schema: "sales",
                table: "credit_note_lines");

            migrationBuilder.RenameColumn(
                name: "order_code",
                schema: "sales",
                table: "sales_orders",
                newName: "document_no");

            migrationBuilder.RenameColumn(
                name: "customer",
                schema: "sales",
                table: "sales_orders",
                newName: "currency_code");

            migrationBuilder.RenameColumn(
                name: "note_code",
                schema: "sales",
                table: "credit_notes",
                newName: "document_no");

            migrationBuilder.RenameColumn(
                name: "customer",
                schema: "sales",
                table: "credit_notes",
                newName: "currency_code");

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "sales_orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "sales_orders",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "sales_orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "sales_order_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "sales_order_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "subsidiary_id",
                schema: "sales",
                table: "credit_notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "document_date",
                schema: "sales",
                table: "credit_notes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                schema: "sales",
                table: "credit_notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "uom_id",
                schema: "sales",
                table: "credit_note_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "item_id",
                schema: "sales",
                table: "credit_note_lines",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_orders_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "sales_orders",
                columns: new[] { "tenant_id", "subsidiary_id", "document_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sales_order_lines_tenant_id_id",
                schema: "sales",
                table: "sales_order_lines",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_credit_notes_tenant_id_subsidiary_id_document_no",
                schema: "sales",
                table: "credit_notes",
                columns: new[] { "tenant_id", "subsidiary_id", "document_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credit_note_lines_tenant_id_id",
                schema: "sales",
                table: "credit_note_lines",
                columns: new[] { "tenant_id", "id" });
        }
    }
}
