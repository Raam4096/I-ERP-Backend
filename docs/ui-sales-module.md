# UI guide: Sales module (Enquiry / Quotation / Invoice)

Share with frontend after backend deploy + DB migrate.

## Navbar

```http
GET /api/v1/metadata/modules
Authorization: Bearer <token>
```

Expect module:

| Field | Value |
|-------|--------|
| `code` | `sales` |
| `name` | `Sales` |
| `source` | `metadata` |
| screens | **Sales Enquiry**, **Quotation**, **Invoice** only |

Old module `sales-distribution` / Sales Orders is removed (soft-deleted on seed).

### Screen codes / routes / APIs

| Screen | `code` | `route` | `apiBasePath` |
|--------|--------|---------|---------------|
| Sales Enquiry | `sales-enquiries` | `/sales/enquiries` | `/api/v1/sales/enquiries` |
| Quotation | `sales-quotations` | `/sales/quotations` | `/api/v1/sales/quotations` |
| Invoice | `sales-invoices` | `/sales/invoices` | `/api/v1/sales/invoices` |

## Schema (GenericPage)

```http
GET /api/v1/metadata/screens/sales-enquiries
GET /api/v1/metadata/screens/sales-quotations
GET /api/v1/metadata/screens/sales-invoices
```

Field keys are **camelCase** (matches UI sample): `enquiryCode`, `contactPerson`, `projectedTotal`, `items`, …

### Enquiry sections

`header`, `commercial`, `terms`, `follow_up`, `items` (line grid — `controlType: grid`, `dataType: json`)

## Example payload (Enquiry)

```http
GET /api/v1/sales/enquiries/example
```

Returns the Globex sample document (same shape as create body).

## CRUD APIs (all need JWT)

### Enquiries

| Method | Path |
|--------|------|
| GET | `/api/v1/sales/enquiries` |
| GET | `/api/v1/sales/enquiries/{id}` |
| GET | `/api/v1/sales/enquiries/example` |
| POST | `/api/v1/sales/enquiries` |
| PUT | `/api/v1/sales/enquiries/{id}` |
| DELETE | `/api/v1/sales/enquiries/{id}` |

### Quotations

| Method | Path |
|--------|------|
| GET/POST | `/api/v1/sales/quotations` |
| GET/PUT/DELETE | `/api/v1/sales/quotations/{id}` |

### Invoices

| Method | Path |
|--------|------|
| GET/POST | `/api/v1/sales/invoices` |
| GET/PUT/DELETE | `/api/v1/sales/invoices/{id}` |

## Create Enquiry body (camelCase)

Use the UI sample shape. Required: `customer`, `title`.

`items[]` fields: `category`, `item`, `description`, `quantity`, `uom`, `priceLevel`, `rate`, `discount`, `amount`, `taxCode`, `grossAmount`, `className`, `countryOfOrigin`, `hsCode`.

If `enquiryCode` / `quotationCode` / `invoiceCode` omitted, backend auto-generates (`TEA-ENQ-…`, `TEA-QTN-…`, `TEA-INV-…`).

Dates: prefer `YYYY-MM-DD` (`expectedClose`, `followUpDate`, …).

## UI binding rules

1. Navbar from **metadata modules only** (includes Sales + CRM + dynamic).
2. Open screen → load schema from metadata → bind controls by `fieldKey`.
3. Line items: render `items` as a grid; POST/PUT send `items` array.
4. Do not hardcode Sales field lists — metadata is source of truth.
5. CRM Lead JSON (`primary_information` / snake_case) is **not** Sales Enquiry — keep CRM separate.

## After deploy

1. Run Sales migration (`SalesDbContext`).
2. Restart API so metadata seeder adds Sales screens.
3. Login → `GET /api/v1/metadata/modules` → confirm `sales` with 3 screens.
4. `GET /api/v1/sales/enquiries/example` → paint demo form.
