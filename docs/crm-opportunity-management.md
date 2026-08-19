# CRM Opportunity Management

Lead → Opportunity conversion, opportunity CRUD, discard/restore/soft-delete, follow-ups, and lead-centric history.

## Business flow

```text
Lead (qualified)
  → POST /api/crm/leads/{leadId}/convert-to-opportunity
  → Opportunity created (OPP-YYYY-000001)
  → Lead status = Converted, ConvertedOpportunityId set
  → Opportunity follow-ups continue commercial work
  → GET /api/crm/leads/{leadId}/history  (lead + opportunity follow-ups)
```

## APIs (all require JWT / auth)

| Method | Path | Notes |
|--------|------|--------|
| `POST` | `/api/crm/leads/{leadId}/convert-to-opportunity` | Create opportunity from lead |
| `GET` | `/api/crm/opportunities` | Paged list |
| `GET` | `/api/crm/opportunities/{id}` | Get one |
| `PUT` | `/api/crm/opportunities/{id}` | Update |
| `POST` | `/api/crm/opportunities/{id}/discard` | Reversible park |
| `POST` | `/api/crm/opportunities/{id}/restore` | Undo discard |
| `DELETE` | `/api/crm/opportunities/{id}` | Soft delete (no restore API) |
| `POST` | `/api/crm/opportunities/{id}/followups` | Add follow-up |
| `GET` | `/api/crm/opportunities/{id}/timeline` | Opp follow-ups only |
| `PUT` | `/api/crm/opportunity-followups/{id}` | Update follow-up |
| `GET` | `/api/crm/leads/{leadId}/history` | Unified history for UI |

## Convert request example

```json
{
  "opportunityValue": 250000,
  "probability": 75,
  "status": "New",
  "computations": "Estimated project value based on scope...",
  "notes": "Commercial discussion in progress.",
  "closedReason": null,
  "currencyCode": "USD",
  "expectedCloseDate": "2026-12-31",
  "followUp": {
    "activityType": "Call",
    "followUpDate": "2026-08-20T10:00:00Z",
    "nextFollowUpDate": "2026-08-27T10:00:00Z",
    "remarks": "Kickoff commercial call",
    "status": "Open"
  }
}
```

Opportunity number is **server-generated** (`OPP-2026-000001`). Do not send client opportunity ids.

## Discard vs Delete

| Action | Effect | Restore? |
|--------|--------|----------|
| Discard | `status = Discarded`, keeps row visible if filtered | Yes via `/restore` |
| Delete | Soft delete (`is_deleted`) | No product API (row remains in DB) |

## Statuses

`New`, `InProgress`, `Won`, `Lost`, `Discarded`  
`closedReason` required when `Won` or `Lost`.

## Lead history item

```json
{
  "id": "...",
  "source": "Lead" | "Opportunity",
  "parentId": "...",
  "parentNumber": "LEAD-2026-000001",
  "activityType": "Call",
  "followUpDate": "...",
  "remarks": "...",
  "status": "Open"
}
```

## Migration

`20260819184118_AddOpportunityManagement` — alters `crm.opportunities`, adds `crm.opportunity_followups`.
