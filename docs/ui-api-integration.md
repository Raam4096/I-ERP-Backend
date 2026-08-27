# UI API integration guide

Guide for frontend developers calling the i-ERP backend (CRM Lead APIs and health checks).

Related backend detail: [crm-lead-management.md](./crm-lead-management.md)

**Metadata-driven UI / Screen Architect / per-user field layout:**  
→ **[ui-metadata-dynamic-screens.md](./ui-metadata-dynamic-screens.md)** (share this with UI for ProcessFlow GenericPage work)

---

## 1. Base URLs

| Environment | Base URL | Notes |
|-------------|----------|--------|
| Local API | `http://localhost:5080` | Run with `dotnet run --project src/iERP.Api` |
| Railway (dev deploy) | `https://<your-railway-public-domain>` | Copy from Railway → API service → Networking |
| Swagger | `{BASE_URL}/swagger` | Only when API runs as **Development** |

**Health (no auth required):**

| Method | Path | Purpose |
|--------|------|---------|
| `GET` | `/health/live` | Liveness |
| `GET` | `/health` | Aggregate health |
| `GET` | `/health/ready` | Readiness (includes DB when configured) |
| `GET` | `/api/v1/crm/health` | CRM module ping |

Always start with `GET {BASE_URL}/health/live` to confirm connectivity.

---

## 2. Frontend configuration (your app)

### Environment variables

**Vite**

```env
VITE_API_BASE_URL=https://YOUR-RAILWAY-DOMAIN
# local:
# VITE_API_BASE_URL=http://localhost:5080
```

**Next.js**

```env
NEXT_PUBLIC_API_BASE_URL=https://YOUR-RAILWAY-DOMAIN
```

Use this value for all `fetch` / axios calls. Do **not** hardcode secrets in the UI.

### Example axios / fetch setup

```ts
const API_BASE = import.meta.env.VITE_API_BASE_URL; // or NEXT_PUBLIC_...

const defaultHeaders: HeadersInit = {
  "Content-Type": "application/json",
  // Prefer: Authorization: Bearer <accessToken> from login
  // Dev-only alternative: X-Tenant-Id must be the real tenant GUID from platform.tenants
};

export async function apiFetch(path: string, init?: RequestInit) {
  const res = await fetch(`${API_BASE}${path}`, {
    ...init,
    headers: {
      ...defaultHeaders,
      ...(init?.headers ?? {}),
    },
  });
  return res;
}
```

Replace the default tenant/user GUIDs when real login exists.

---

## 3. Authentication & headers (current state)

**JWT login is implemented.** Prefer Bearer tokens for UI (local, Vercel, Railway).

Full contract: [FRONTEND_AUTH_INTEGRATION.md](./FRONTEND_AUTH_INTEGRATION.md)

### Recommended (all environments)

```http
Authorization: Bearer <access_token>
Content-Type: application/json
```

Login: `POST /api/v1/auth/login` with `tenantCode`, `email`, `password`.

JWT claims used by the API:

- `tenant_id` (GUID)
- `user_id` (GUID)

### Temporary Development header auth (local only)

When Development and **no** `Authorization` header is sent, you may send:

| Header | Required? | Description |
|--------|-----------|-------------|
| `X-Tenant-Id` | **Required** for header auth | Must be the **real** tenant GUID from `platform.tenants` (same as metadata `tenant_id`) |
| `X-User-Id` | Optional | User GUID |

There is **no** fake default tenant anymore (that caused empty module lists). Prefer JWT Authorize in Swagger.

Do not use header auth for the Vercel UI once JWT is wired.

### Local seed user (Development + AuthSeed)

| Field | Value |
|-------|--------|
| Tenant | `demo` |
| Email | `admin@ierp.local` |
| Password | `ChangeMe!123` |

---

## 4. CORS (browser)

Backend allows these origins (no trailing slash):

- `http://localhost:3000`
- `http://localhost:5173`
- `https://i-erp-dev-ui.vercel.app`

If your UI runs on another URL, ask backend to add it under `Cors:Origins` and redeploy.

**You do not configure CORS in the frontend.** If the browser reports CORS errors:

1. Confirm your page origin matches the list exactly (scheme + host + port).
2. Confirm you are calling the Railway/local API base URL (not a wrong host).
3. Confirm the API redeploy includes the CORS change.

Optional local proxy (Vite) if you prefer same-origin calls during local UI work:

```ts
// vite.config.ts
server: {
  proxy: {
    "/api": "http://localhost:5080",
    "/health": "http://localhost:5080",
  },
}
```

Then set `VITE_API_BASE_URL=` empty or `""` and call `/api/...` on the Vite origin.

---

## 5. CRM Lead APIs

All of these require auth (JWT Bearer).

| Method | Path | Purpose |
|--------|------|---------|
| `POST` | `/api/v1/crm/leads` | Create lead |
| `GET` | `/api/v1/crm/leads` | List (paged) |
| `GET` | `/api/v1/crm/leads/{id}` | Get one |
| `PUT` | `/api/v1/crm/leads/{id}` | Update |
| `DELETE` | `/api/v1/crm/leads/{id}` | Soft delete |
| `POST` | `/api/v1/crm/leads/{leadId}/followups` | Add follow-up |
| `GET` | `/api/v1/crm/leads/{leadId}/timeline` | Follow-up history |
| `PUT` | `/api/v1/crm/followups/{id}` | Update follow-up |

JSON property names are **camelCase**.

**Dates:** send ISO-8601 (`followUpDate`, `nextFollowUpDate`). Any offset is accepted (e.g. `+05:30`); the API stores **UTC** in PostgreSQL. Prefer `...Z` (UTC) when possible.

### List query parameters

`GET /api/v1/crm/leads?page=1&pageSize=20&search=&status=&assignedToUserId=&sortBy=&sortDescending=true`

| Param | Default / notes |
|-------|-----------------|
| `page` | `1` |
| `pageSize` | `20` |
| `search` | company, email, phone, lead number, contact |
| `status` | filter |
| `assignedToUserId` | GUID |
| `sortBy` | e.g. `companyName`, `status`, `leadNumber`, `email` |
| `sortDescending` | default `true` |

### Create lead body

`POST /api/v1/crm/leads`

Required: `companyName`, `phone`, `email`.

```json
{
  "companyName": "Acme Pte Ltd",
  "contactPerson": "Jane Doe",
  "phone": "+6591234567",
  "email": "jane@acme.com",
  "industry": "Marine",
  "address": "Singapore",
  "annualRevenue": 1000000,
  "assignedTo": "22222222-2222-2222-2222-222222222222",
  "companySize": "50-100",
  "leadSource": "Website",
  "projectDescription": "ERP rollout",
  "projectType": "Implementation",
  "status": "New",
  "subsidiary": "Acme SG",
  "subsidiaryId": null,
  "website": "https://acme.com",
  "notes": "Hot lead",
  "followUp": {
    "activityType": "Call",
    "followUpDate": "2026-08-13T10:00:00Z",
    "nextFollowUpDate": "2026-08-20T10:00:00Z",
    "remarks": "Intro call",
    "status": "Open",
    "attachments": []
  }
}
```

### Success response shape

```json
{
  "success": true,
  "message": "Lead created successfully.",
  "data": {
    "id": "...",
    "leadNumber": "LEAD-2026-000001",
    "companyName": "Acme Pte Ltd",
    "email": "jane@acme.com",
    "phone": "+6591234567",
    "status": "New"
  }
}
```

### Paged list shape

```json
{
  "success": true,
  "data": [ /* LeadDto[] */ ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 42,
    "totalPages": 3
  },
  "message": null
}
```

`DELETE` returns **204 No Content** (empty body).

### Error handling (UI)

| HTTP | Typical meaning | UI action |
|------|-----------------|-----------|
| `400` | Validation | Show `errors` / `message` |
| `401` | Missing/invalid auth | Check headers / env Development |
| `404` | Not found | Show not found |
| `409` | Duplicate email or phone | Show conflict message |

Example validation error:

```json
{
  "success": false,
  "error": "VALIDATION_ERROR",
  "message": "One or more validation errors occurred.",
  "errors": ["'Email' is not a valid email address."]
}
```

---

## 6. Lead fields for forms / tables

Use these from `data` / list items:

`id`, `leadNumber`, `companyName`, `contactPerson`, `phone`, `email`, `industry`, `address`, `annualRevenue`, `assignedToUserId`, `companySize`, `leadSource`, `projectDescription`, `projectType`, `status`, `subsidiary`, `subsidiaryId`, `website`, `notes`, `createdAt`, `createdBy`, `updatedAt`, `updatedBy`, `version`, `followUps`

Lead numbers look like `LEAD-2026-000001`.

---

## 7. Quick smoke test (browser / Postman)

1. `GET {BASE_URL}/health/live` → 200  
2. `GET {BASE_URL}/api/v1/crm/leads` with Bearer token → 200 (empty or list)  
3. `POST {BASE_URL}/api/v1/crm/leads` with sample JSON → 201  
4. From Vercel UI (`https://i-erp-dev-ui.vercel.app`) call the same — CORS should allow it  

---

## 8. Checklist for UI developers

- [ ] Set `VITE_API_BASE_URL` / `NEXT_PUBLIC_API_BASE_URL` to local or Railway  
- [ ] Login via `POST /api/v1/auth/login`, store tokens  
- [ ] Send `Authorization: Bearer <accessToken>` on CRM calls  
- [ ] Handle `401` with refresh once, then login  
- [ ] Run UI from an allowed origin (`localhost:3000`, `localhost:5173`, or Vercel URL above)  
- [ ] Handle `400` / `401` / `404` / `409`  
- [ ] Do not rely on Swagger in Production  
- [ ] Attachments: metadata only for now (no binary upload API yet)  

---

## 9. Not available yet

- Full user administration APIs (invite/reset password UI flows)
- Real binary file upload to blob storage  
- Permission-policy authorization on every module endpoint  
- Dynamic module **sections** (group fields client-side for now)

Login / refresh / logout JWT APIs **are available** — see [FRONTEND_AUTH_INTEGRATION.md](./FRONTEND_AUTH_INTEGRATION.md).

Lead → Opportunity APIs **are available** under `/api/v1/crm/opportunities`.  
Dynamic modules + metadata prefs — see [ui-metadata-dynamic-screens.md](./ui-metadata-dynamic-screens.md).
