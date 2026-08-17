# UI API integration guide

Guide for frontend developers calling the i-ERP backend (CRM Lead APIs and health checks).

Related backend detail: [crm-lead-management.md](./crm-lead-management.md)

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
| `GET` | `/api/v1/leads/health` | CRM module ping |

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
  "X-Tenant-Id": "11111111-1111-1111-1111-111111111111",
  "X-User-Id": "22222222-2222-2222-2222-222222222222",
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

**JWT login is not implemented yet.** There is no `/login` or token issue API.

### Temporary auth (Development)

When the API runs with `ASPNETCORE_ENVIRONMENT=Development` (local, and Railway while JWT is pending):

| Header | Required? | Description |
|--------|-----------|-------------|
| `Content-Type` | Yes for JSON body | `application/json` |
| `X-Tenant-Id` | Recommended | Tenant GUID (default used if omitted) |
| `X-User-Id` | Recommended | User GUID (default used if omitted) |
| `Authorization` | Not needed in Development | — |

**Default GUIDs if headers are omitted:**

- Tenant: `11111111-1111-1111-1111-111111111111`
- User: `22222222-2222-2222-2222-222222222222`

### Production (JWT — future)

When the API is `Production` without Development auth:

```http
Authorization: Bearer <access_token>
Content-Type: application/json
```

JWT must include claims:

- `tenant_id` (GUID)
- `user_id` (GUID)

Until that is shipped, calling Railway in **Production** returns **401** on `/api/crm/...`.

### What UI should send today (Railway + Vercel)

1. Confirm Railway API has `ASPNETCORE_ENVIRONMENT=Development`.
2. Send `Content-Type`, `X-Tenant-Id`, `X-User-Id` on every CRM request.
3. Do **not** send a fake Bearer token unless it is a real JWT signed by the API.

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

All of these require auth (Development headers or future JWT).

| Method | Path | Purpose |
|--------|------|---------|
| `POST` | `/api/crm/leads` | Create lead |
| `GET` | `/api/crm/leads` | List (paged) |
| `GET` | `/api/crm/leads/{id}` | Get one |
| `PUT` | `/api/crm/leads/{id}` | Update |
| `DELETE` | `/api/crm/leads/{id}` | Soft delete |
| `POST` | `/api/crm/leads/{leadId}/followups` | Add follow-up |
| `GET` | `/api/crm/leads/{leadId}/timeline` | Follow-up history |
| `PUT` | `/api/crm/followups/{id}` | Update follow-up |

JSON property names are **camelCase**.

**Dates:** send ISO-8601 (`followUpDate`, `nextFollowUpDate`). Any offset is accepted (e.g. `+05:30`); the API stores **UTC** in PostgreSQL. Prefer `...Z` (UTC) when possible.

### List query parameters

`GET /api/crm/leads?page=1&pageSize=20&search=&status=&assignedToUserId=&sortBy=&sortDescending=true`

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

`POST /api/crm/leads`

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
2. `GET {BASE_URL}/api/crm/leads` with `X-Tenant-Id` + `X-User-Id` → 200 (empty or list)  
3. `POST {BASE_URL}/api/crm/leads` with sample JSON → 201  
4. From Vercel UI (`https://i-erp-dev-ui.vercel.app`) call the same — CORS should allow it  

---

## 8. Checklist for UI developers

- [ ] Set `VITE_API_BASE_URL` / `NEXT_PUBLIC_API_BASE_URL` to local or Railway  
- [ ] Send `Content-Type: application/json` on writes  
- [ ] Send `X-Tenant-Id` and `X-User-Id` until JWT login exists  
- [ ] Run UI from an allowed origin (`localhost:3000`, `localhost:5173`, or Vercel URL above)  
- [ ] Handle `400` / `401` / `404` / `409`  
- [ ] Do not rely on Swagger in Production  
- [ ] Attachments: metadata only for now (no binary upload API yet)  

---

## 9. Not available yet

- Login / refresh token APIs  
- Real JWT issuance for Production  
- Binary file upload to blob storage  
- Lead → Opportunity conversion  

When JWT ships, replace header-based auth with `Authorization: Bearer ...` and keep the same REST paths.
