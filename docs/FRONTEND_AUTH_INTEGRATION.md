# Frontend auth integration

Backend contract for the React UI. This is documentation only — no frontend code lives in this repository.

Base URL examples:

- Local: `http://localhost:5080`
- Railway: `https://YOUR-RAILWAY-DOMAIN`

---

## Authentication flow

```text
React UI
  → POST /api/v1/auth/login  (tenantCode + email + password)
  → store accessToken + refreshToken securely
  → call APIs with Authorization: Bearer <accessToken>
  → on 401: POST /api/v1/auth/refresh with refreshToken (once), retry
  → logout: POST /api/v1/auth/logout with refreshToken, clear storage
```

Do **not** put tokens in localStorage if you can use memory + httpOnly cookies later. For the current API (Bearer body refresh, no cookies), memory or sessionStorage is typical for SPA demos.

---

## Login endpoint

`POST /api/v1/auth/login`  
`Content-Type: application/json`  
**Anonymous** (no Bearer required)

### Request

```json
{
  "tenantCode": "demo",
  "email": "admin@ierp.local",
  "password": "ChangeMe!123"
}
```

| Field | Required | Notes |
|-------|----------|--------|
| `tenantCode` | yes | Multi-tenant login key (e.g. `demo`) |
| `email` | yes | User email within that tenant |
| `password` | yes | Plain password over HTTPS only |

### Success `200`

```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "base64-opaque-token...",
    "accessTokenExpiresAt": "2026-08-18T16:00:00+00:00",
    "refreshTokenExpiresAt": "2026-09-01T15:45:00+00:00",
    "tokenType": "Bearer",
    "user": {
      "id": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
      "tenantId": "11111111-1111-1111-1111-111111111111",
      "email": "admin@ierp.local",
      "userName": "admin",
      "displayName": "Demo Admin",
      "roles": ["Tenant Admin"]
    }
  }
}
```

### Status codes

| Code | Meaning |
|------|---------|
| `200` | Login OK |
| `400` | Validation error |
| `401` | Invalid credentials (generic message — no account enumeration) |

### Local Development seed user

When `ASPNETCORE_ENVIRONMENT=Development` and `AuthSeed:Enabled=true`:

| Field | Value |
|-------|--------|
| Tenant | `demo` |
| Email | `admin@ierp.local` |
| Password | `ChangeMe!123` |

Change password / disable seed before any shared environment.

---

## JWT usage

Every protected API call:

```http
Authorization: Bearer <accessToken>
Content-Type: application/json
```

Do **not** send `X-Tenant-Id` / `X-User-Id` when using JWT. Tenant and user come from token claims (`tenant_id`, `user_id`).

### Token lifetime

Configured defaults:

| Token | Lifetime |
|-------|----------|
| Access token | **15 minutes** (`Jwt:AccessTokenMinutes`) |
| Refresh token | **14 days** (`Jwt:RefreshTokenDays`) |

---

## Refresh endpoint

`POST /api/v1/auth/refresh`  
**Anonymous**

### Request

```json
{
  "refreshToken": "<refresh-token-from-login>"
}
```

### Success `200`

Same shape as login (`AuthTokenResponse`). Old refresh token is **revoked** (rotation). Store the new refresh token.

### Failure `401`

Invalid, expired, or revoked refresh token → clear session and send user to login.

---

## Logout endpoint

`POST /api/v1/auth/logout`  
**Anonymous** (sends refresh token in body)

```json
{
  "refreshToken": "<current-refresh-token>"
}
```

- Success: `204 No Content`
- Access JWT cannot be revoked server-side; it expires naturally (≤15 min)
- Always clear tokens from the client after logout

---

## 401 handling

Recommended client behavior:

1. If request was already a refresh → logout / redirect to login  
2. Else if refresh token exists → call `/api/v1/auth/refresh` once  
3. Retry original request with new access token  
4. If refresh fails → clear tokens → login page  

Do not infinite-loop refresh.

---

## 403 handling

`403 Forbidden` means the user is authenticated but not allowed. Show “access denied”; do not treat as “re-login” unless product rules say so.

Role/permission enforcement beyond “authenticated” is still limited on many endpoints; backend remains authoritative.

---

## Roles / claims

Access token includes:

| Claim | Purpose |
|-------|---------|
| `sub` / `user_id` | User id |
| `tenant_id` | Tenant id (required for multi-tenant APIs) |
| `email` | Email |
| `role` / `ClaimTypes.Role` | Role names (e.g. `Tenant Admin`) |

UI may show role labels; **never** rely on client-side claim checks as security.

---

## CORS

Allowed origins (backend):

- `http://localhost:3000`
- `http://localhost:5173`
- `https://i-erp-dev-ui.vercel.app`

- Credentials / cookies: **not required** for auth (Bearer in header)  
- Do **not** set `withCredentials: true` unless you later switch to cookie-based refresh  
- Ask backend to whitelist any new UI origin

---

## Protected CRM example

After login:

```http
GET /api/crm/leads?page=1&pageSize=20
Authorization: Bearer <accessToken>
```

---

## Example (fetch)

```ts
const API = import.meta.env.VITE_API_BASE_URL;

export async function login(tenantCode: string, email: string, password: string) {
  const res = await fetch(`${API}/api/v1/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ tenantCode, email, password }),
  });
  if (!res.ok) throw new Error("Login failed");
  const json = await res.json();
  return json.data; // { accessToken, refreshToken, user, ... }
}

export async function apiFetch(path: string, accessToken: string, init: RequestInit = {}) {
  const res = await fetch(`${API}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${accessToken}`,
      ...(init.headers ?? {}),
    },
  });
  if (res.status === 401) {
    // trigger refresh + retry in your auth layer
  }
  return res;
}
```

---

## Related docs

- Backend implementation notes: [JWT_AUTH_IMPLEMENTATION.md](./JWT_AUTH_IMPLEMENTATION.md)
- CRM APIs: [ui-api-integration.md](./ui-api-integration.md)
