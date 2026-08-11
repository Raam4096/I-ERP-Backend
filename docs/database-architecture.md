# Database Architecture

## Engine

- PostgreSQL 16
- EF Core 8 + Npgsql
- snake_case tables/columns
- UUID primary keys
- soft delete + audit columns on tenant-owned tables
- money: `NUMERIC(19,4)` (quantities `19,6`, FX `19,8`)

## Schemas

| Schema | Owner module | Major tables |
|--------|--------------|--------------|
| `platform` | Tenancy / platform | `tenants`, `outbox_messages` |
| `identity` | Identity | `users`, `roles`, `permissions`, `user_roles`, `role_permissions`, `refresh_tokens`, `field_permission_grants` |
| `organization` | Organization / Settings | `subsidiaries`, `branches`, `departments`, `cost_centers`, `reporting_dimensions`, `system_settings`, `document_sequences` |
| `metadata` | Metadata | `module_definitions`, `screen_definitions`, `section_definitions`, `field_definitions`, `custom_field_definitions`, `custom_field_values` |
| `workflow` | Workflow | `workflow_definitions`, `workflow_steps`, `workflow_instances`, `workflow_histories` |
| `rules` | Rules | `rule_definitions` |
| `bridge` | Bridge | `bridge_definitions`, `bridge_mappings`, `bridge_logs` |
| `printing` | Printing | `print_templates`, `print_template_versions` |
| `crm` | CRM | `leads`, `opportunities`, `activities`, `customers`, `contacts`, `addresses` |
| `catalog` | Catalog | `items`, `item_categories`, `units_of_measure`, `price_lists`, `price_list_items` |
| `sales` | Sales | `sales_quotations`, `sales_orders`, `sales_invoices`, `credit_notes`, `delivery_orders` (+ lines) |
| `procurement` | Procurement | `vendors`, `purchase_requests`, `purchase_orders`, `goods_received_notes`, `supplier_invoices` (+ lines) |
| `inventory` | Inventory | `warehouses`, `bin_locations`, `inventory_transactions`, `stock_balances`, `stock_reservations`, `stock_transfers` |
| `finance` | Finance | `chart_of_accounts`, `fiscal_years`, `journal_entries`, `tax_codes`, `currencies`, `exchange_rates`, `budgets`, ... |
| `banking` | Banking | `bank_accounts`, `payment_vouchers`, `receipt_vouchers`, `bank_reconciliations` |
| `projects` | Projects | `projects`, `contracts`, `retention_rules`, `subcontractors` |
| `hr` | HR | `employees` |
| `manufacturing` | Manufacturing | `bills_of_materials`, `work_centres`, `work_orders` |
| `assets` | Assets | `assets`, `asset_types`, `asset_maintenance_schedules` |
| `marine` | Marine | `vessels`, `port_locations` |
| `reporting` | Reporting | `report_definitions` |
| `notifications` | Notifications | `notification_logs` |
| `attachments` | Attachments | `attachments` |
| `audit` | Audit | `activity_logs` |
| `ai` | AI | `ai_tool_definitions`, `ai_tool_permissions`, `ai_logs` |
| `dynamic` | DynamicModules | `dynamic_module_definitions`, `dynamic_entity_definitions`, `dynamic_field_definitions`, `dynamic_records` |

Application tables do **not** use the PostgreSQL `public` schema.

## Connection strings

- `PrimaryDatabase` — OLTP / module DbContexts
- `ReportingDatabase` — read-model / reporting connection (may equal primary locally)

## Outbox

`platform.outbox_messages` stores integration events for later Worker publishing.
