# i-ERP Architecture

## Overview

i-ERP is a **modular monolith** using Clean Architecture boundaries, optimized for a medium-sized engineering team.

```
                    +----------------------+
                    |     React UI         |
                    +----------+-----------+
                               |
                               v
+--------------------------------------------------------------+
|                         iERP.Api                             |
|  middleware: correlation, tenant, auth, rate limit, errors   |
+---------------------------+----------------------------------+
                            |
        +-------------------+-------------------+
        v                   v                   v
+---------------+   +---------------+   +---------------+
| Platform      |   | Engines       |   | Business      |
| Tenancy       |   | Workflow      |   | CRM Catalog   |
| Identity      |   | Rules         |   | Sales Procure |
| Organization  |   | Bridge        |   | Inventory ... |
| Metadata ...  |   | Printing      |   | Finance AI    |
+-------+-------+   +-------+-------+   +-------+-------+
        |                   |                   |
        +-------------------+-------------------+
                            |
                            v
              +---------------------------+
              | PostgreSQL (schemas)      |
              | Redis (optional cache)    |
              | Outbox -> Service Bus*    |
              +---------------------------+

* Azure integrations are abstracted; local uses null/dev implementations.
```

## Dependency direction

```
API / Worker
    ↓
Module Application + Infrastructure
    ↓
Module Domain
    ↓
SharedKernel / Application.Abstractions
```

- Domain never references EF Core, ASP.NET, Redis, or Azure packages.
- Modules communicate via contracts/events, not by writing another module's tables.
- AI never accesses PostgreSQL directly; it must call ERP application services/tools.

## Hosts

| Host | Responsibility |
|------|----------------|
| `iERP.Api` | REST `/api/v1/*`, Swagger, health, auth middleware |
| `iERP.Worker` | Outbox polling / Hangfire jobs skeleton |

## Building blocks

| Project | Contents |
|---------|----------|
| `iERP.SharedKernel` | Entity bases, tenancy context, exceptions, API envelopes, permissions |
| `iERP.Application.Abstractions` | Cache, bus, files, AI, engines, options |
| `iERP.Infrastructure` | EF helpers, interceptors, Redis/Azure placeholders, JWT, OTel |

## AI path (mandatory)

```
User → AI Orchestrator → Semantic Kernel → Tool Registry → AI Governance → ERP Application/API → Database
```
