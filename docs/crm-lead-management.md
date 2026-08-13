# CRM Lead Management — Implemented Features

## Are the REST APIs ready for clients?

**Yes.** The backend exposes real HTTP REST endpoints that a React (or any) client can call.

Typical flow:

```
Client (React / Postman / Swagger)
    ↓  HTTP JSON request
ASP.NET Core Minimal API  (/api/crm/...)
    ↓  Authorize
MediatR Command / Query
    ↓
FluentValidation  (reject bad payloads → 400)
    ↓
Application Handler  (business rules, duplicates → 409)
    ↓
EF Core CrmDbContext
    ↓
PostgreSQL  (schema: crm)
    ↓
ApiResponse / PagedResponse JSON back to client
```

So: **client calls API → we validate → we persist to DB → we return a standard JSON response.**

---

## Base URLs

| Item | Value |
|------|--------|
| API host (local) | `http://localhost:5080` |
| Swagger UI | `http://localhost:5080/swagger` |
| Health | `http://localhost:5080/health/live` |
| Database | PostgreSQL `ierp_dev` |
| Schema | `crm` |

In **Development**, endpoints are authorized automatically (dev auth).  
Optional headers:

- `X-Tenant-Id`
- `X-User-Id`

In Production, send a JWT Bearer token with `tenant_id` and `user_id` claims.

---

## REST APIs implemented

### Leads

| Method | Endpoint | Purpose |
|--------|----------|---------|
| `POST` | `/api/crm/leads` | Create lead (+ optional first follow-up) |
| `GET` | `/api/crm/leads` | Paginated list + search/filter/sort |
| `GET` | `/api/crm/leads/{id}` | Get one lead (includes follow-ups & attachments) |
| `PUT` | `/api/crm/leads/{id}` | Update lead |
| `DELETE` | `/api/crm/leads/{id}` | Soft delete lead |

### Follow-ups

| Method | Endpoint | Purpose |
|--------|----------|---------|
| `POST` | `/api/crm/leads/{leadId}/followups` | Add follow-up (+ optional attachments metadata) |
| `PUT` | `/api/crm/followups/{id}` | Update follow-up |
| `GET` | `/api/crm/leads/{leadId}/timeline` | Follow-up history for a lead |

---

## Query parameters (list leads)

`GET /api/crm/leads`

| Param | Description |
|-------|-------------|
| `page` | Page number (default 1) |
| `pageSize` | Page size (default 20, max 100) |
| `search` | Search company, email, phone, lead number, contact |
| `status` | Filter by status |
| `assignedToUserId` | Filter by assignee |
| `sortBy` | `companyName`, `status`, `leadNumber`, `email`, or created date |
| `sortDescending` | `true` / `false` (default `true`) |

---

## Sample client request

`POST /api/crm/leads`  
`Content-Type: application/json`

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

### Success response (201)

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

### Validation failure (400)

```json
{
  "success": false,
  "error": "VALIDATION_ERROR",
  "message": "One or more validation errors occurred.",
  "errors": ["'Email' is not a valid email address."]
}
```

### Duplicate email/phone (409)

```json
{
  "success": false,
  "error": "DUPLICATE_RECORD",
  "message": "A lead with the same email or phone already exists."
}
```

---

## Business rules implemented

| Rule | Behavior |
|------|----------|
| Auto lead number | `LEAD-YYYY-000001` |
| Required fields | Company name, phone, email |
| Email format | Validated |
| Website format | Valid absolute `http`/`https` URL when provided |
| Duplicate prevention | Same email **or** phone → HTTP 409 |
| Soft delete only | `DELETE` sets `is_deleted`; no hard delete |
| Audit fields | `created_at`, `created_by`, `updated_at`, `updated_by` (UTC) |
| Optimistic concurrency | `version` column |
| Multi-tenant isolation | `tenant_id` + EF global query filter |

---

## Database objects created

Schema: **`crm`**

| Table | Purpose |
|-------|---------|
| `crm.leads` | Lead master |
| `crm.lead_followups` | Follow-up activities (FK → leads, Restrict) |
| `crm.lead_attachments` | Attachment metadata only (FK → followups, Restrict) |

Migrations run automatically on API startup for `CrmDbContext` (`Database.MigrateAsync()`).

---

## Architecture pieces used

| Layer | What was built |
|-------|----------------|
| **Api** | `LeadEndpoints.cs` — REST route mapping |
| **Application** | MediatR commands/queries/handlers, FluentValidation, AutoMapper DTOs |
| **Domain** | Rich `Lead` aggregate + `LeadFollowUp` + `LeadAttachment` |
| **Infrastructure** | EF configurations, `CrmDbContext`, indexes, FKs |

Controllers/endpoints contain **no business logic** — only dispatch to MediatR.

---

## How to test quickly

1. Ensure PostgreSQL is running and DB `ierp_dev` exists.
2. Run:
   ```bash
   dotnet run --project src/iERP.Api
   ```
3. Open Swagger: http://localhost:5080/swagger
4. Call `POST /api/crm/leads`, then `GET /api/crm/leads`

Or with curl:

```bash
curl -X POST http://localhost:5080/api/crm/leads ^
  -H "Content-Type: application/json" ^
  -d "{\"companyName\":\"Acme\",\"phone\":\"+6591111111\",\"email\":\"a@acme.com\",\"status\":\"New\"}"
```

---

## What is intentionally not done yet

- Real login JWT issuance (dev auth is used locally)
- Uploading binary files to Azure Blob (attachments store metadata/path only)
- Lead → Opportunity conversion workflow
- Workflow / Rules / Notifications integration

Those can be added later without changing the REST contract shape for clients.
