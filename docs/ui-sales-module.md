# UI guide: Sales module

Share with frontend after backend deploy + DB migrate.

## Navbar

```http
GET /api/v1/metadata/modules
Authorization: Bearer <token>
```

Expect module `sales` / **Sales** with screens:

| Screen | `code` | `route` | `apiBasePath` |
|--------|--------|---------|---------------|
| Sales Enquiry | `sales-enquiries` | `/sales/enquiries` | `/api/v1/sales/enquiries` |
| Quotation | `sales-quotations` | `/sales/quotations` | `/api/v1/sales/quotations` |
| Orders | `sales-orders` | `/sales/orders` | `/api/v1/sales/orders` |
| Credit Notes | `sales-credit-notes` | `/sales/credit-notes` | `/api/v1/sales/credit-notes` |
| Debit Notes | `sales-debit-notes` | `/sales/debit-notes` | `/api/v1/sales/debit-notes` |
| Invoice | `sales-invoices` | `/sales/invoices` | `/api/v1/sales/invoices` |

Screen layouts are seeded and stay editable for field enhancements (do not hard-code form fields in UI — use metadata screens).

## Schema (GenericPage)

```http
GET /api/v1/metadata/screens/sales-enquiries
GET /api/v1/metadata/screens/sales-quotations
GET /api/v1/metadata/screens/sales-orders
GET /api/v1/metadata/screens/sales-credit-notes
GET /api/v1/metadata/screens/sales-debit-notes
GET /api/v1/metadata/screens/sales-invoices
```

Field keys are **camelCase**.

## Optional document links (open for workflow enhancements)

Documents are independent CRUD. Optional soft links (no hard FK / no forced conversion API yet):

| Document | Optional source fields |
|----------|------------------------|
| Quotation | `sourceEnquiryId` / `sourceEnquiryCode` |
| Order | `sourceEnquiryId`, `sourceQuotationId` (+ codes) |
| Invoice | `sourceOrderId`, `sourceQuotationId` (+ codes) |
| Credit / Debit note | `sourceInvoiceId` / `sourceInvoiceCode` |

If you pass a source **Id**, the API validates it exists and fills the code. Invalid ids return validation errors.

Suggested chain for UI (all optional):

`Enquiry → Quotation → Order → Invoice → Credit/Debit Note`

## Example payload (Enquiry)

```http
GET /api/v1/sales/enquiries/example
```

## CRUD APIs (all need JWT)

| Area | Base path |
|------|-----------|
| Enquiries | `/api/v1/sales/enquiries` |
| Quotations | `/api/v1/sales/quotations` |
| Orders | `/api/v1/sales/orders` |
| Invoices | `/api/v1/sales/invoices` |
| Credit notes | `/api/v1/sales/credit-notes` |
| Debit notes | `/api/v1/sales/debit-notes` |

Each supports `GET /`, `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}` (enquiries also have `/example`).

## Create body shape (shared)

Customer is required. Line items use the shared `items[]` grid (`category`, `item`, `quantity`, `rate`, `amount`, …).

Auto codes: `TEA-ENQ-…`, `TEA-QTN-…`, `TEA-SO-…`, `TEA-INV-…`, `TEA-CN-…`, `TEA-DN-…`.

## Deploy notes

1. Apply Sales migration `AddSalesOrderAndDebitCreditNoteCommercialFields`.
2. Restart API so metadata seeder registers the new Sales screens.
3. UI navbar = `GET /api/v1/metadata/modules` only.
