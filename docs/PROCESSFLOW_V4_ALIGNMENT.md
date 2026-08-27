# ProcessFlow v4 alignment (feature branch)

Branch: `feature/processflow-v4-alignment`  
Goal: align with client ProcessFlow v4 using versioned `/api/v1/*` routes.

## Shipped in this branch (additive)

| Change | Detail |
|--------|--------|
| `GET /api/v1/auth/me` | Returns current user + roles for UI session restore |
| `GET /api/v1/metadata/screens/{screenCode}` | GenericPage contract; merges core + custom fields |
| Metadata migrate + seed | Seeds CRM module + `crm-leads` / `crm-opportunities` for all tenants |
| System roles seed | All 10 ProcessFlow system roles for every tenant |
| Dev admin role | Seeded admin assigned **Super Admin** (in-tenant) |
| CRM routes | **`/api/v1/crm/*` only** (legacy `/api/crm/*` removed) |
| Dynamic Modules Screen Architect APIs | CRUD for modules → entities (screens) → fields; CRUD for `dynamic_records` (`payload_json`) |
| Metadata modules bootstrap | `GET /api/v1/metadata/modules` (metadata + dynamic nav tree) |
| Per-user field prefs | `PUT /api/v1/metadata/screens/{code}/preferences`; merged into screen/entity GET |
| Custom field CRUD | `.../entities/{entityName}/custom-fields` |
| CRM custom field values | Optional `customFields` on lead create/update/get via `ICustomFieldValueStore` |

## Compatibility

- UI should call **`/api/v1/crm/...`** only (not `/api/crm/...`).
- Refresh token remains **body JSON** (SPA-friendly); HttpOnly cookie is a later option.

## UI notes

1. Optional: call `GET /api/v1/auth/me` after login / on boot.
2. Optional: `GET /api/v1/metadata/screens/crm-leads` for GenericPage experiment.
3. No required UI break for current CRM screens.
4. **Screen Architect (custom modules):**
   - Navbar: `GET /api/v1/metadata/modules?activeOnly=true` (preferred) or `GET /api/v1/dynamic_modules?activeOnly=true`
   - Create module / entity / field via POST under `/api/v1/dynamic_modules/...`
   - Form schema: `GET /api/v1/dynamic_modules/entities/{entityId}`
   - Save data: `POST /api/v1/dynamic_modules/entities/{entityId}/records` with `{ "values": { "fieldKey": "..." } }`
   - UI “screen” maps to **entity**; “section” not in dynamic schema yet (group fields client-side if needed)
5. **Existing screens (Hybrid):**
   - Add field: `POST /api/v1/metadata/entities/crm-leads/custom-fields`
   - Lead payload may include `customFields: { "my_field": "value" }`
6. **Per-user hide / drag-drop:**
   - `PUT /api/v1/metadata/screens/{screenCode}/preferences` with `{ "fields": [ { "fieldKey", "isVisible", "displayOrder" } ] }`
   - Required fields cannot be hidden; prefs merge into `GET .../screens/{code}` and dynamic entity GET

## Still Todo (later PRs)

- Full tenant self-registration onboarding
- Permission enforcement matrix (who can edit Screen Architect)
- Dynamic module sections (optional)
- Workflow / Rules / Bridge / Print products
- Sales Quotation Hybrid path
- Operational AI
