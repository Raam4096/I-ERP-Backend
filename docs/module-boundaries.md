# Module Boundaries

Modules own their schema and tables. Cross-module relations use IDs/contracts, not direct table writes.

## Platform

| Module | Owns |
|--------|------|
| Tenancy | tenants, outbox_messages |
| Identity | users, roles, permissions, refresh tokens, field permissions |
| Organization | subsidiaries, branches, departments, cost centers, reporting dimensions |
| Settings | system_settings, document_sequences |
| Metadata | screen/field metadata, custom fields |
| Audit | activity_logs |
| Attachments | attachments metadata (blobs in Azure) |
| Notifications | notification_logs |
| DynamicModules | dynamic definitions/records (non-financial only) |

## Engines

| Module | Owns |
|--------|------|
| Workflow | definitions, steps, instances, history |
| Rules | rule_definitions |
| Bridge | bridge definitions/mappings/logs |
| Printing | print templates/versions |

## Business

| Module | Owns |
|--------|------|
| CRM | leads, opportunities, activities, **customers**, contacts, addresses |
| Catalog | items, categories, UoM, price lists |
| Sales | quotations, orders, invoices, credit notes, delivery orders |
| Procurement | vendors, PR/PO/GRN/supplier invoices |
| Inventory | warehouses, bins, inventory transactions, stock balances/reservations/transfers |
| Finance | COA, fiscal periods, journals, tax, FX, budgets |
| Banking | bank accounts, payment/receipt vouchers, reconciliations |
| Projects | projects, contracts, retention, subcontractors |
| HR | employees |
| Manufacturing | BOM, work centres, work orders |
| Assets | assets, types, maintenance schedules |
| Marine | vessels, ports |
| Reporting | report definitions + reporting connection factory |
| AI | tool registry metadata, permissions, AI logs |

## Explicit non-ownership examples

- Sales stores `CustomerId` but does **not** write `crm.customers`.
- Inventory owns stock quantities; Catalog `Item` does not hold on-hand qty.
- Finance owns journals; other modules must not post GL rows directly.
- DynamicRecord must never store GL/invoices/stock/payments source-of-truth data.
