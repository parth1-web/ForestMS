# ForestMS — Jukekhadi/Jamunkhadi Community Forest User Group Management System

A comprehensive management system for community forest user groups in Nepal, handling billing, inventory, accounting, and administration.

## Overview

| Item | Detail |
|------|--------|
| **Product** | ForestMS — Community Forest User Group Management System (Nepal) |
| **Stack** | ASP.NET Core 3.1 MVC (Areas) · EF Core + Npgsql (PostgreSQL) · Autofac · Rotativa (wkhtmltopdf) |
| **Modules** | Billing (wood/firewood/chiran/counter/furniture), Inventory, Accounting, Administration, Setup |
| **Special Features** | Nepali BS/AD date conversion · en/ne localization · External ClickOnce WinForms POS client (JWT-authenticated) |

## Architecture

```
LE.Web                 → Main MVC application (Areas: Billing, Accounting, Inventory, Administration, Setup)
LE.Service             → Business logic services
LE.Entities            → Domain entities
LE.Common              → Shared utilities, helpers, base classes
LE.Context             → EF Core DbContext, repositories
LE.Infrastructure      → Infrastructure services
LE.Account.*           → Account module (Common, Entities, Factories, Infrastructure, Service, Providers)
LE.Billing.*           → Billing module (Common, Context, Entities, Factories, Infrastructure, Service)
LE.Inventory.*         → Inventory module (Common, Context, Entities, Infrastructure, Service)
```

## Key Features

### Billing Module
- Wood billing (member/counter sales, goliya tracking, double-sell prevention)
- Firewood billing
- Chiran billing
- Furniture billing
- Counter billing (POS integration via JWT)
- Membership management
- Member punishment tracking
- Day-close operations

### Inventory Module
- Stock management (items, units, types)
- Purchase management
- Wood details (goliya tracking, transfer to chiran)
- Stock piling

### Accounting Module
- Ledger setup & groups
- Journal vouchers
- Payment/Receipt vouchers
- Fiscal year management
- Financial reports (Day Book, Trial Balance, P&L, Balance Sheet, Cash/Bank Book, Ledger Statement)

### Administration
- User management
- Role-based access control
- Module/permission mapping

## Security Status

| Phase | Status | Key Fixes |
|-------|--------|-----------|
| **P0** | ✅ Complete | Admin password rotation, 7 anonymous controllers secured, JWT key externalized, PBKDF2 1000→310k iterations (SHA-256), login rate limiting, CSRF on money paths, IDOR fixes, disabled-user login blocked |
| **P1** | ✅ Complete | Double-sell guard (atomic), bill numbers via PG sequences, tax allocation per stock-type, cancel+day-close transactional, punishment logic fixed, stock deletion bug fixed, voucher aliasing bugs fixed, year-close corrected, membership reconciliation, mutating GETs→POST+antiforgery, module-level permission enforcement |
| **P2** | 🚧 In Progress | Dead code removal, Autofac hygiene, Serilog logging, exception sanitization, HTTPS/HSTS enforcement, Unit-of-Work migration (219 repo-write sites) |

## Getting Started

### Prerequisites
- .NET SDK 3.1+ (upgrade to 8.x planned)
- PostgreSQL 12+
- wkhtmltopdf (for PDF reports via Rotativa)

### Configuration
1. Copy `LE.Web/appsettings.example.json` → `LE.Web/appsettings.json`
2. Update connection string (`DefaultConnection`)
3. Set strong `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`
4. Set `Security:CookieExpirationHours` (default 8)
5. Set `Serilog:MinimumLevel`, `Serilog:File:path` (default `logs/le-web-.log`)

### Database Setup
```bash
cd LE.Web
dotnet ef database update
```
**Required migrations** (run in order):
- `20260911000000_p1_billing_settings_unique_key` — billing settings unique index
- `20260914000000_p2_account_settings_unique_key` — account settings unique index

### Running
```bash
cd LE.Web
dotnet run
```
Access at `https://localhost:5001` (HTTPS enforced by default in Production).

### POS Desktop Client
The ClickOnce WinForms counter-POS client authenticates via JWT to `/account/jwtlogin` and calls:
- `GET /billing/counter-billing/current-day`
- `POST /billing/counter-billing/save` (antiforgery exempt — JWT + server-derived identity)

## Development

### Build
```bash
dotnet build LE.Forest.sln
```
Expected: **0 errors**, ~330-350 warnings (pre-existing Magick.NET NU19xxx advisories).

### Project Structure (Key Files)
```
LE.Web/
├── Controllers/           → Area controllers (Billing, Accounting, Inventory, Admin, Setup)
├── Views/                 → Razor views per area
├── wwwroot/               → Static assets (58 MB, cleanup in progress)
├── Helpers/               → LoginAttemptTracker, ModulePermissionFilter, ExceptionMessageHelper, etc.
├── Seed Data/             → SeedData.txt (admin password placeholder)
├── appsettings.example.json
├── Program.cs             → Serilog bootstrap
└── Startup.cs             → DI, auth, middleware pipeline
```

### Key Conventions
- **Transactions**: Use `BaseRepository.beginTransaction()` → `saveChanges()` → `Commit()` (no `TransactionScope`)
- **Permissions**: `ModulePermissionFilter` (global TypeFilter) checks `role_permission_maps` via user roles
- **Antiforgery**: Per-action `[ValidateAntiForgeryToken]` on money paths; `post-link.js` converts `a.post-link` to token-protected POST
- **Dates**: Nepali BS/AD conversion throughout; timezone = NPT (UTC+5:45)
- **Errors**: `CustomException` messages shown to users; all others sanitized via `ExceptionMessageHelper`

## Deployment Checklist

1. **Set strong `Jwt:Key`** via environment variable / secret store
2. **Rotate admin password** on any DB seeded from old `SeedData.txt` (was `admin`)
3. Copy `appsettings.example.json` → `appsettings.json`; fill DB password + JWT key
4. Apply EF migrations (`dotnet ef database update`)
5. Configure reverse proxy (nginx/IIS) for TLS termination — HSTS/HTTPS redirect gated by `Security:EnableHsts` / `Security:EnableHttpsRedirect`
6. Mount `logs/` volume for Serilog file sink (retention 31 days default)
7. Communicate to POS users: no change needed; next app restart picks up new JWT iss/aud

## Roadmap (P2 Remaining)

- [ ] .NET 3.1 → LTS 8.x upgrade (clears Magick.NET NU19xxx advisories)
- [ ] Async data layer; disable lazy loading; fix pager (`href`s, `NextPageService`, `OrderBy` before `Skip/Take`)
- [ ] wwwroot cleanup (58 MB, 54 jQuery copies, unused libs) via libman/bundling
- [ ] Git history purge of binaries (`wkhtmltopdf.exe`, `counter_billing.zip`, `obj/` artifacts)
- [ ] Menu-level permissions (beyond module-level); global `AutoValidateAntiforgeryToken`
- [ ] Nepali timezone/`IClock` standardization (single time abstraction)
- [ ] Real documentation beyond this README

## License

Proprietary — Leading Edge Software, Nepal.

## Contact

For support or inquiries, contact the development team.