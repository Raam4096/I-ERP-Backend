# ProcessFlow v4 alignment (feature branch)

Branch: `feature/processflow-v4-alignment`  
Goal: align with client ProcessFlow v4 **without breaking** deployed CRM on `/api/crm/*`.

## Shipped in this branch (additive)

| Change | Detail |
|--------|--------|
| `GET /api/v1/auth/me` | Returns current user + roles for UI session restore |
| `GET /api/v1/metadata/screens/{screenCode}` | GenericPage contract; merges core + custom fields |
| Metadata migrate + seed | Seeds CRM module + `crm-leads` screen on AuthSeed tenant |
| System roles seed | All 10 ProcessFlow system roles for tenant |
| Dev admin role | Seeded admin assigned **Super Admin** (in-tenant) |
| CRM dual routes | `/api/crm/*` **kept**; aliases added under `/api/v1/crm/*` |
| Dynamic Modules Screen Architect APIs | CRUD for modules → entities (screens) → fields; CRUD for `dynamic_records` (`payload_json`) |

## Compatibility

- Existing UI on `/api/crm/leads` and `/api/crm/opportunities` continues to work.
- New UI / ProcessFlow clients can adopt `/api/v1/...` gradually.
- Refresh token remains **body JSON** (SPA-friendly); HttpOnly cookie is a later option.

## UI notes

1. Optional: call `GET /api/v1/auth/me` after login / on boot.
2. Optional: `GET /api/v1/metadata/screens/crm-leads` for GenericPage experiment.
3. No required UI break for current CRM screens.
4. **Screen Architect (custom modules):**
   - Navbar: `GET /api/v1/dynamic_modules?activeOnly=true`
   - Create module / entity / field via POST under `/api/v1/dynamic_modules/...`
   - Form schema: `GET /api/v1/dynamic_modules/entities/{entityId}`
   - Save data: `POST /api/v1/dynamic_modules/entities/{entityId}/records` with `{ "values": { "fieldKey": "..." } }`
   - UI “screen” maps to **entity**; “section” not in dynamic schema yet (group fields client-side if needed)

## Still Todo (later PRs)

- Full tenant self-registration onboarding
- Permission enforcement matrix
- Custom field settings CRUD APIs (Hybrid overlays on fixed CRM entities)
- Dynamic module sections (optional)
- Workflow / Rules / Bridge / Print products
- Sales Quotation Hybrid path
- Operational AI
