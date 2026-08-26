# i-ERP Alignment Checklist vs ProcessFlow v4

**Source of truth:** `i-ERP_ProcessFlow_v4.docx`  
**Purpose:** One-page status for client review — what already matches, what is partial, what remains.

**Legend:** Done = implemented in backend foundation · Partial = schema/structure exists, product incomplete · Todo = not built yet

---

## 1. Architecture foundation

| Item (from ProcessFlow v4) | Status | Notes |
|---|---|---|
| Shared PostgreSQL + `tenant_id` per row (MVP) | **Done** | Matches §3 / §11; enterprise schema-per-tenant path documented as future |
| Modular monolith .NET 8 + Clean-style modules | **Done** | API + Worker hosts |
| UUID PKs, soft delete, audit columns, snake_case | **Done** | Standard across tenant entities |
| JWT access + refresh + logout | **Done** | Refresh currently via request body (SPA); doc prefers HttpOnly cookie — align in addendum |
| API success/error envelope + standard error codes | **Partial** | Codes exist; some HTTP status mappings (e.g. duplicate 409 vs doc 400) to align |
| `/api/v1/` on all endpoints | **Partial** | Auth under `/api/v1`; CRM currently `/api/crm/...` — standardize next |
| CI/CD, Git Flow `develop`, full Dev/Staging/Prod | **Partial** | Docker/Railway path exists; full pipeline & env split still Todo |
| React + Vite UI / React Native | **Todo** (UI repos) | Backend-ready contracts |

---

## 2. Tenancy, roles & onboarding (§8–§9)

| Item | Status | Notes |
|---|---|---|
| Tenant = company; data isolation by `tenant_id` | **Done** | |
| In-tenant **Super Admin** + **Tenant Admin** + module roles | **Partial** | Role model + `is_system_role` exist; 10 default roles not fully seeded/enforced |
| Auto-seed on tenant registration (roles, settings, sequences, screens, welcome email) | **Todo** | Dev seed only today |
| Module access matrix + field-level permissions | **Partial** | Tables/constants exist; runtime enforcement incomplete |
| Platform Operator billing/license console | **Out of scope in v4** | Optional later addendum — not required by ProcessFlow v4 |

---

## 3. Screens, metadata & custom fields (§4–§5)

| Item | Status | Notes |
|---|---|---|
| Core / Hybrid / Dynamic / Special screen model | **Partial** | Design aligned; GenericPage contract not fully productized |
| `GET /api/v1/metadata/screens/{screenCode}` | **Todo** | Metadata tables exist; screen merge API Todo |
| Custom fields via `custom_field_definitions` + `entity_name` = screen code | **Partial** | Tables exist; settings APIs + auto-render merge Todo |
| Core tables never altered for custom columns | **Done** (design) | EAV values pattern in place |
| Field-level permission application in UI/API | **Todo** | |

---

## 4. Engines (§6)

| Item | Status | Notes |
|---|---|---|
| Workflow Engine (draft→posted lifecycle) | **Partial** | Module shell; product APIs Todo |
| Rule Engine | **Partial** | Module shell; product APIs Todo |
| Bridge Engine (Quote→Order→Invoice) | **Partial** | Module shell; product APIs Todo |
| Print Engine | **Partial** | Module shell; product APIs Todo |

---

## 5. Business modules & masters (§20 + sprints)

| Item | Status | Notes |
|---|---|---|
| CRM Leads / Opportunities / Follow-ups (Hybrid pilot) | **Done** (MVP APIs) | Ahead of doc Sprint order; usable as Hybrid proof |
| Sales Quotation CRUD + document sequences (Sprint 2a) | **Todo** | Next Hybrid priority per doc |
| Customers / Items masters | **Partial** | Domain stubs; full product APIs Todo |
| Finance / Inventory / Procurement full flows | **Todo** | Module boundaries exist |
| 41 master data groups (setup sequence) | **Todo** | Spec complete in doc; implementation phased |

---

## 6. AI (§7) — Phase gate applies

| Item | Status | Notes |
|---|---|---|
| AI never touches DB directly (API/tools only) | **Done** (rule) | Architecture rule accepted |
| Operational AI (Semantic Kernel, Tool Registry, governance, Control Room) | **Todo** | Phase 1 after Hybrid+engines stable |
| Implementation AI (metadata generation) | **Todo** | Phase 2 only after Phase 1 sign-off |

---

## 7. Security, mobile, reporting (§13–§15)

| Item | Status | Notes |
|---|---|---|
| Password policy / lockout / Key Vault | **Partial / Todo** | Harden to §13 |
| Mobile approval inbox (Phase 1 mobile) | **Todo** | |
| Reporting on read replica | **Partial** | `ReportingDatabase` config concept exists |

---

## Recommended next build order (aligned to v4)

1. Metadata screen API + custom-field merge (GenericPage contract)  
2. Tenant onboarding seed + 10 system roles + permission enforcement  
3. Sales Quotation Hybrid path (sequences, workflow submit)  
4. Bridge + Print basics  
5. Operational AI spike (only after above is stable)

---

## Client confirmation requested

1. Accept ProcessFlow v4 as SoT (yes/no).  
2. Confirm Super Admin = **in-tenant** role (not platform vendor console) for Phase 1.  
3. Agree CRM-first delivery as accepted Hybrid pilot vs strict Sprint 2a Quotation-first.  
4. Prefer refresh token as **HttpOnly cookie** (doc) or keep **SPA body token** (current) for web UI.
