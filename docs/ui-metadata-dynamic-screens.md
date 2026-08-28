# UI guide: Metadata-driven screens, Screen Architect & per-user layout

Share this with frontend developers integrating **ProcessFlow-style** dynamic UI against branch `feature/processflow-v4-alignment`.

Auth / base URL / CORS: see [ui-api-integration.md](./ui-api-integration.md) and [FRONTEND_AUTH_INTEGRATION.md](./FRONTEND_AUTH_INTEGRATION.md).

All paths below need:

```http
Authorization: Bearer <access_token>
Content-Type: application/json
```

---

## Mental model (how UI should think)

```
App boot
  → GET modules          → build navbar tabs
Open a screen
  → GET screen schema    → render form/grid dynamically (do NOT hardcode fields)
User edits / hides / reorders fields
  → PUT preferences      → per-user only
User saves data
  → call apiBasePath     → CRM lead APIs OR dynamic records
```

| Concept | Backend | UI |
|---------|---------|-----|
| Module | Navbar tab | One nav item |
| Screen | Page under module | Route + form |
| Section | Group of fields | Fieldset / accordion |
| Field | Input definition | Control by `controlType` / `dataType` |
| Custom field | Extra field on Hybrid screen | Same renderer; `isCustom: true` |
| Dynamic module | Fully custom module | Same navbar; data via `/dynamic_modules/.../records` |

---

## 1. App boot — navbar from backend (one API only)

**Use this for navbar — includes predefined CRM + any Screen Architect modules:**

```http
GET /api/v1/metadata/modules?activeOnly=true
```

Do **not** build the main navbar from `GET /api/v1/dynamic_modules` — that returns **only** custom (dynamic) modules. The metadata modules call already merges both.

| `source` | Meaning | Example |
|----------|---------|---------|
| `"metadata"` | Predefined / Hybrid (seeded) | CRM → screen `crm-leads` |
| `"dynamic"` | Created in Screen Architect | Any custom module |

Today’s seeded predefined module (after API restart on any environment, including Railway):

| Module code | Screen code | Route | Data API |
|-------------|-------------|-------|----------|
| `crm` | `crm-leads` | `/crm/leads` | `/api/v1/crm/leads` |
| `crm` | `crm-opportunities` | `/crm/opportunities` | `/api/v1/crm/opportunities` |

Seeder runs on **startup for every tenant** in the DB (not only local AuthSeed). Redeploy/restart Railway API once after this change so empty Railway metadata is filled.

So the UI developer does **not** hardcode CRM in the nav. CRM appears in the same list as new modules.

Response `data[]` shape:

```json
{
  "id": "...",
  "code": "crm",
  "name": "CRM",
  "source": "metadata",
  "isActive": true,
  "screens": [
    {
      "id": "...",
      "code": "crm-leads",
      "name": "CRM Leads",
      "route": "/crm/leads",
      "entityName": "crm-leads",
      "apiBasePath": "/api/v1/crm/leads"
    }
  ]
}
```

`source` is `"metadata"` (core/Hybrid) or `"dynamic"` (Screen Architect modules).

### UI logic

```ts
// On login / app shell mount
const { data: modules } = await api.get("/api/v1/metadata/modules?activeOnly=true");

// Navbar = modules
modules.forEach(m => addNavTab({ key: m.code, label: m.name, children: m.screens }));

// Route map
// metadata screen  →  m.screens[].route  (e.g. /crm/leads)
// dynamic screen   →  /dynamic/{moduleCode}/{entityName}  (or use screens[].route from API)
```

Do **not** hardcode CRM-only nav if you want Screen Architect modules to appear automatically.

Optional session restore:

```http
GET /api/v1/auth/me
```

---

## 2. Open screen — load schema, then values

### Schema (sections + fields + descriptions)

```http
GET /api/v1/metadata/screens/crm-leads
```

CRM Leads is seeded with UI sections:

| Section code | Title |
|--------------|--------|
| `primary_information` | Primary Information |
| `classification` | Classification |
| `additional_information` | Additional Information |
| `follow_ups` | Follow-ups |

Each section includes `description` and ordered `fields` (`fieldKey` is **camelCase**, matching CRM lead APIs: `companyName`, `phone`, …).

### Values (section-wise, with existing lead data)

```http
GET /api/v1/crm/leads/{id}/form
```

Returns:

```json
{
  "success": true,
  "data": {
    "id": "...",
    "screenCode": "crm-leads",
    "leadNumber": "LEAD-2026-000001",
    "sections": [
      {
        "code": "primary_information",
        "title": "Primary Information",
        "description": "Core company and contact details for the lead.",
        "fields": [
          {
            "fieldKey": "companyName",
            "label": "Company Name",
            "dataType": "text",
            "controlType": "input",
            "required": true,
            "readOnly": false,
            "displayOrder": 1,
            "value": "Nexus Innovations Pvt Ltd"
          }
        ]
      }
    ],
    "valuesBySection": {
      "primary_information": {
        "companyName": "Nexus Innovations Pvt Ltd",
        "company_name": "Nexus Innovations Pvt Ltd",
        "contactPerson": "Rajesh Kumar",
        "phone": "+91 98765 43210"
      },
      "classification": { "subsidiary": "..." },
      "additional_information": { "projectDescription": "...", "notes": "..." },
      "follow_ups": { "followUpDate": "...", "followUpStatus": "..." }
    }
  }
}
```

`valuesBySection` includes both camelCase and snake_case aliases so either UI style works. Prefer camelCase when calling create/update lead APIs.

### Hybrid / seeded screens (e.g. CRM Leads) — schema only

```http
GET /api/v1/metadata/screens/crm-leads
```

Returns **GenericPage** (already merges custom fields + **current user’s** hide/order prefs).

```json
{
  "screen": {
    "code": "crm-leads",
    "name": "CRM Leads",
    "route": "/crm/leads",
    "renderMode": "generic",
    "entityName": "crm-leads",
    "apiBasePath": "/api/v1/crm/leads"
  },
  "layout": { "mode": "form-with-grid", "columns": 12 },
  "sections": [
    {
      "code": "main",
      "title": "Lead Details",
      "type": "header",
      "fields": [
        {
          "fieldKey": "companyName",
          "label": "Company Name",
          "dataType": "string",
          "controlType": "input",
          "required": true,
          "readOnly": false,
          "visible": true,
          "width": 3,
          "displayOrder": 1,
          "isCustom": false
        }
      ]
    }
  ],
  "actions": [
    { "actionKey": "save", "label": "Save", "actionType": "api", "endpoint": "/api/v1/crm/leads" }
  ]
}
```

### Dynamic (Screen Architect) screens

```http
GET /api/v1/dynamic_modules/entities/{entityId}
```

Fields include `isVisible` / `displayOrder` after per-user prefs. `apiBasePath` points at records API.

### UI renderer (one component for all screens)

```ts
function DynamicForm({ page }: { page: GenericPage }) {
  const fields = page.sections
    .flatMap(s => s.fields)
    .filter(f => f.visible)                 // honor hide
    .sort((a, b) => a.displayOrder - b.displayOrder);

  return (
    <form>
      {page.sections.map(section => (
        <Section key={section.code} title={section.title}>
          {section.fields
            .filter(f => f.visible)
            .sort((a, b) => a.displayOrder - b.displayOrder)
            .map(f => (
              <FieldControl
                key={f.fieldKey}
                name={f.fieldKey}
                label={f.label}
                required={f.required}
                readOnly={f.readOnly}
                controlType={f.controlType}   // input | number | datepicker | checkbox | textarea | select
                dataType={f.dataType}
                width={f.width}
              />
            ))}
        </Section>
      ))}
    </form>
  );
}
```

**Rules**

- Never hardcode `companyName` / `email` for GenericPage mode — bind by `fieldKey`.
- `required === true` → always show; do not offer hide in UI (API rejects hide).
- Core CRM fields still save via CRM APIs; custom fields go in `customFields` bag (below).

---

## 3. Save data

### A) Hybrid CRM lead (core + custom fields)

Use existing lead APIs (`/api/v1/crm/leads`).

Create / update may include optional:

```json
{
  "companyName": "Acme",
  "phone": "+6591234567",
  "email": "jane@acme.com",
  "customFields": {
    "priority_score": 10,
    "region": "APAC"
  }
}
```

Get-by-id returns `customFields` when present. List may omit them (use get for detail form).

**UI split of form values**

```ts
const coreKeys = new Set(page.sections.flatMap(s => s.fields).filter(f => !f.isCustom).map(f => f.fieldKey));
const payload = {
  companyName: values.companyName,
  phone: values.phone,
  email: values.email,
  // ...other known core props your Lead DTO expects
  customFields: Object.fromEntries(
    Object.entries(values).filter(([k]) => !coreKeys.has(k) || pageField(k)?.isCustom)
  ),
};
```

Simpler approach: send all known Lead DTO properties as today; put only `isCustom` keys into `customFields`.

### B) Dynamic module records

```http
POST /api/v1/dynamic_modules/entities/{entityId}/records
PUT  /api/v1/dynamic_modules/records/{recordId}
GET  /api/v1/dynamic_modules/entities/{entityId}/records
```

Body:

```json
{
  "values": {
    "employee_name": "Ada",
    "salary": 120000
  }
}
```

Keys must match field definitions. Required/type validated by API.

---

## 4. Screen Architect (Settings → create module / screen / field)

Admin-only UX; call in order:

| Step | API |
|------|-----|
| Create module | `POST /api/v1/dynamic_modules` `{ "code", "name", "description?", "isActive" }` |
| Create screen (entity) | `POST /api/v1/dynamic_modules/{moduleId}/entities` `{ "entityName", "displayName" }` |
| Create field | `POST /api/v1/dynamic_modules/entities/{entityId}/fields` `{ "fieldKey", "label", "dataType", "displayOrder", "isRequired" }` |

Allowed `dataType`: `string`, `text`, `number`, `decimal`, `int`, `integer`, `boolean`, `bool`, `date`, `datetime`, `email`, `phone`, `lookup`.

After create → refresh `GET /api/v1/metadata/modules` so navbar updates.

### Add field on **existing** CRM screen

```http
POST /api/v1/metadata/entities/crm-leads/custom-fields
```

```json
{
  "fieldKey": "priority_score",
  "label": "Priority Score",
  "dataType": "number",
  "displayOrder": 100,
  "isRequired": false,
  "isActive": true
}
```

Then reload `GET /api/v1/metadata/screens/crm-leads` — new field appears (`isCustom: true`).

---

## 5. Per-user hide / unhide & drag-drop order

Prefs are **per logged-in user**, not tenant-wide.

### Save after user finishes layout edit

```http
PUT /api/v1/metadata/screens/{screenCode}/preferences
```

For dynamic screens use **entityName** as `screenCode` (same key used when merging prefs).

```json
{
  "fields": [
    { "fieldKey": "companyName", "isVisible": true, "displayOrder": 1 },
    { "fieldKey": "notes", "isVisible": false, "displayOrder": 20 },
    { "fieldKey": "email", "isVisible": true, "displayOrder": 2 }
  ]
}
```

- Send **full list** of fields user can control (replace strategy).
- Do not set `isVisible: false` on `required` fields → `400 VALIDATION_ERROR`.
- After save, either re-fetch screen schema or apply local state from the same payload.

### Suggested UI flow

1. Load schema (`GET screens/{code}`).
2. Show “Customize layout” → drag list + eye toggle (disable toggle when `required`).
3. On Save layout → `PUT .../preferences`.
4. On Cancel → discard local draft; keep last server schema.

```ts
async function saveLayout(screenCode: string, fields: FieldState[]) {
  await api.put(`/api/v1/metadata/screens/${screenCode}/preferences`, {
    fields: fields.map((f, i) => ({
      fieldKey: f.fieldKey,
      isVisible: f.required ? true : f.visible,
      displayOrder: i + 1,
    })),
  });
  // refresh
  return api.get(`/api/v1/metadata/screens/${screenCode}`);
}
```

---

## 6. Recommended app structure

```
AppShell
  useModules() → GET /metadata/modules
  <Navbar />

Routes
  /crm/leads          → HybridScreen("crm-leads")   // metadata GET + CRM APIs
  /dynamic/:mod/:ent  → DynamicScreen(entityId)     // entity GET + records APIs
  /settings/architect → ScreenArchitectPage         // dynamic_modules CRUD
  /settings/fields/:entity → CustomFieldsAdmin      // custom-fields CRUD
```

Shared pieces:

- `<GenericPageRenderer />` — one form engine
- `<FieldControl />` — maps `controlType` / `dataType`
- `<LayoutCustomizer />` — drag + hide → preferences PUT

---

## 7. Quick checklist for UI

- [ ] Login + Bearer on all calls  
- [ ] Boot navbar from `GET /api/v1/metadata/modules`  
- [ ] Render forms from schema, not hardcoded field lists  
- [ ] Filter `visible === false`; sort by `displayOrder`  
- [ ] Hybrid save: core Lead props + `customFields`  
- [ ] Dynamic save: `{ values: { ... } }` to records API  
- [ ] Layout customize → `PUT .../preferences` (per user)  
- [ ] Architect create module → entity → fields → refresh modules  

---

## 8. What not to do

- Don’t persist field order only in localStorage if you need sync across devices — use preferences API.  
- Don’t hide required fields in the client and expect save to work without them.  
- Don’t ALTER / assume new columns on `crm.leads` for custom attributes — use `customFields`.  
- Don’t treat Dynamic “entity” as Metadata “section” — sections exist only on metadata screens today.

---

## 10. Troubleshooting: modules empty in Swagger but rows exist in DB

Metadata is **tenant-scoped**. `GET /api/v1/metadata/modules` only returns rows where `tenant_id` matches the JWT `tenant_id` claim.

### Correct Swagger flow

1. `POST /api/v1/auth/login` with your Railway tenant (`tenantCode`, email, password).
2. Copy `data.accessToken`.
3. Click **Authorize** → paste token **without** the `Bearer ` prefix → Authorize.
4. Call `GET /api/v1/metadata/modules`.
5. Optional check: `GET /api/v1/auth/me` and confirm tenant matches DB:
   ```sql
   SELECT tenant_id, code, name FROM metadata.module_definitions WHERE NOT is_deleted;
   ```

### Why it looked empty before

If Swagger ran in Development **without** Authorize, the API used a fake tenant `11111111-...` that has **no** seeded metadata → `data: []` even though real tenant rows exist in the DB.

Fix (deployed with this change): no fake default tenant; without JWT you get **401** instead of a silent empty list.

On Railway Production, set `Swagger__Enabled=true` if you need Swagger outside Development.

| Area | Endpoints |
|------|-----------|
| Nav | `GET /api/v1/metadata/modules` |
| Schema | `GET /api/v1/metadata/screens/{code}` |
| Prefs | `PUT /api/v1/metadata/screens/{code}/preferences` |
| Custom fields | `GET/POST /api/v1/metadata/entities/{entityName}/custom-fields`, `PUT/DELETE /api/v1/metadata/custom-fields/{id}` |
| Dynamic architect | `/api/v1/dynamic_modules/...` |
| Dynamic data | `/api/v1/dynamic_modules/entities/{id}/records` |
| CRM leads | `/api/v1/crm/leads` |
| Me | `GET /api/v1/auth/me` |
