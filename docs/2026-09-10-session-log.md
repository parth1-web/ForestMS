# ForestMS — Work Log & Handover Notes (2026-09-10)

> Session record of the full codebase review and the **P0 security hardening** implemented on this date.
> Intended as the working reference for the next phases (P1 financial integrity, P2 modernization).

---

## 1. System Overview

| Item | Detail |
|---|---|
| Product | ForestMS — Jukekhadi/Jamunkhadi Community Forest User Group Management System (Nepal) |
| Stack | ASP.NET Core 3.1 MVC (Areas) · EF Core + Npgsql (PostgreSQL) · Autofac + MS DI · Rotativa (wkhtmltopdf) |
| Modules | Billing (wood/firewood/chiran/counter/furniture bills, members, punishments, day-close) · Inventory (stock, purchases, piling, wood details) · Accounting (ledgers, journals, payments, receipts, fiscal year, reports) · Administration (users/roles) · Setup |
| Special constraints | Nepali BS/AD date conversion throughout; en/ne localization; **external desktop counter-POS client** (ClickOnce WinForms, JWT-authenticated) calling `billing/counter-billing/*` and `api/service` |
| Build env | .NET SDK 10.0.401 installed; solution builds with **0 errors**, ~330 warnings (mostly NU19xxx NuGet vulnerability notices for Magick.NET 13.9.0) |

---

## 2. Code Review — Full Findings (Reference for P1/P2)

A complete review was performed across all modules. Below is the condensed defect register.
**Line numbers refer to the code as found before the P0 fixes.**

### 2.1 CRITICAL — Security (fixed in P0 unless noted)

| # | Finding | Location |
|---|---|---|
| S1 | Seeded admin password was literally `admin` (PBKDF2 hash committed) | `LE.Web/Seed Data/SeedData.txt:123` — **FIXED in P0** |
| S2 | 7 controllers had no `[Authorize]`: `WoodBillingController`, `ApiController` (billing), `MemberTransactionController`, `ChiranBillingController`, `CounterBillingReportController`, `StockItemController`, `StockUnitController` — anonymous users could create/cancel bills (money + ledger entries) and read financial data — **FIXED in P0** (global `AuthorizeFilter`) |
| S3 | Role/module permission model (`role_permission_maps`) enforced **only for navbar rendering** — no action-level permission checks anywhere | `NavbarViewComponent.cs` — open (P1: claims + policy/filter) |
| S4 | JWT key `"thisisasecreteforauth"` hardcoded; validation required iss/aud but token had neither — **FIXED in P0** | `Startup.cs`, `AccountController.GenerateToken` |
| S5 | Cookie `HttpOnly=false`, expiry contradictions (1 min / 30 days / unused 8h) — **FIXED in P0** | `Startup.cs` |
| S6 | Disabled users could still log in (`validateUser` ignored `is_enabled`) — **FIXED in P0** | `AuthenticationServiceImpl.validateUser` |
| S7 | CSRF disabled globally; `[IgnoreAntiforgeryToken]` on money-path POSTs; mutating GETs — **partially FIXED in P0** (money-path POSTs + login/logout; mutating GETs remain: wood bill cancel, day-close save, membership/member enable/disable/delete, purchase/stock-item delete, payment/receipt cancel) |
| S8 | PBKDF2 with only 1000 iterations, SHA-1 — **FIXED in P0** (310k, SHA-256, lazy upgrade) | `PasswordHashImpl.cs` |
| S9 | No login rate limiting / lockout — **FIXED in P0** (`LoginAttemptTracker`) |
| S10 | IDOR: `counter-billing/current-day?user_id=` let any user read others' sales; `user_id` on saves was client-supplied; `PurchaseController.add` `user_id` assignment was dead code (mapper overwrote it) — **FIXED in P0** | `CounterBillingController`, `PurchaseController.add` |
| S11 | No HTTPS enforcement (HSTS/redirect commented out); raw `ex.Message` returned to clients | `Startup.cs` — open (P1/P2) |
| S12 | DB credentials `postgres/postgres` committed in `appsettings.json` | file is git-ignored in repo, but values are known — rotate in production |

### 2.2 CRITICAL — Money & Business Logic (open — these are the P1 targets)

| # | Finding | Location |
|---|---|---|
| B1 | **Double-selling of wood logs**: `insert` sets `is_sold = true` without verifying it was `false`; no conditional update / concurrency check | `WoodBillServiceImpl.insert` (~line 243) |
| B2 | **Bill-number race**: app-generated PKs from a counter row read **outside** the TransactionScope in `BillingSettingsRepositoryImpl.get*Sequence()`; concurrent inserts collide (raw PG PK-violation shown to user) | `BillingSettingsRepositoryImpl.cs:52-71` |
| B3 | **Tax posted per stock-type group with full bill tax inside the loop** — mixed bills post tax N times and overstate cash | `WoodBillServiceImpl.insert` (~line 263) |
| B4 | **Wood bill cancel bypasses day-close** (counter sales have the check) and runs **without any transaction**; mixed bills reverse entirely to the first detail's stock-type ledger | `WoodBillServiceImpl.cancel` |
| B5 | Chiran day-close check **commented out** | `ChiranSalesServiceImpl` (~line 105) |
| B6 | **Punishment check inverted**: `Any(p => ... && !p.IsActive)` flags inactive punishments; expiry (`PunishmentValidity`) ignored | `MemberPunishmentController.GetMemberValidity` (~line 212) |
| B7 | **Deleting a purchase INCREASES stock**: service emits `decrease` but `Movement_ItemAvailabilityAdapterImpl` force-maps `delete → increase` | `Movement-ItemAvailabilityAdapterIMpl.cs:52-54` vs `PurchaseServiceImpl.cs:96-104` |
| B8 | No negative-stock guard; availability read-modify-write with no locking; first movement on a missing row stores positive qty for decreases | `StockItemAvailabilityServiceImpl.update/insert` |
| B9 | `PurchaseServiceImpl.delete` has no `TransactionScope` (check + soft-delete + movement not atomic) | `PurchaseServiceImpl.delete` |
| B10 | Wood duplicate-check typo `a.goliya_number.CompareTo(a.goliya_number)` (always true) + `Contains` (LIKE %..%) matching | `WoodDetailsServiceImpl.cs:102`, `WoodDetailsRepositoryImpl` |
| B11 | **Payment/Receipt cancel + receipt create discount aliasing**: already-added ledger line object mutated and re-added → Dr≠Cr for any voucher with discount | `PaymentServiceImpl.getTransactionDtoForReverseEntry`, `ReceiptServiceImpl` (same), `TransactionDtoAssemblerImpl` |
| B12 | `TransactionDto.calculateTransactionAmount` returns **first debit only** (early `return` inside loop) — journal header amount wrong for multi-line journals | `TransactionDto.cs:51-68` |
| B13 | **Year-close**: compares the SQL string literal instead of query result (P/L always posted as credit); trusts client-posted amounts; `Id + 1` assumption can leave **no running fiscal year** → NRE on all subsequent transactions | `FinancialYearServices.CloseYear/UpdateForThisYear` |
| B14 | Day-close screen: `getQueryable()` (never null) null-checked instead of `.Any()` → always shows "closed"; screen totals include cancelled bills while posting excludes them | `DayCloseController.add` (~line 69), sums at lines 45/66/67 |
| B15 | Membership save failure returns `success = true` ("Membership failed to save."); bill total trusted from client JS floats, never reconciled with detail sums; member-split rounding (`Round(qty/count, 4)`) never sums back to the bill | `MembershipController` (~line 184), `WoodBillingController.setWoodBillDto` |
| B16 | Fiscal-year validation one-sided: allows dates after FY end; NRE if no running FY | `TransactionServiceImpl.addTransaction` |
| B17 | Day book report: HTML filters by ToDate, PDF by FromDate | Accounting `ReportController` dayBook vs dayBookPrint |
| B18 | Purchase list applies `is_deleted` filter **after** `Skip/Take` (short pages, wrong counts) | `PurchaseController.Index` |

### 2.3 HIGH — Architecture / Data Layer (open — P2 targets)

- `SaveChanges()` inside every repository write → no unit-of-work; ambient `TransactionScope` + suppressed `AmbientTransactionWarning` (`Startup.cs`), `BaseRepositoryImpl`
- `TransactionManagerImpl` is dead, broken code (`[ThreadStatic]` counter, commit-before-save, negative-count possible) — delete
- Entire data layer synchronous (one `ToListAsync` in the solution); thread starvation under load
- Lazy-loading proxies enabled globally + N+1 patterns everywhere (member-transaction report loads whole table then lazy-queries per row; accounting reports call `GetRunningFinancialYear().Result` up to 2×/action)
- **Pagination broken**: `Pager.cs` renders empty `href=""`; `NextPageService` compares page to item count; `?page=0` → `Skip(-10)` → 500; `Skip/Take` without `OrderBy`
- `AutofacModule` double-registers `BaseController`; property injection only covers some controllers → latent NREs on `getLoggedInUserId()` in unlisted controllers
- `HomeController.Index` passes authentication id to a user lookup (works only because seed ids coincide)
- `LE.Integration` shadow services (unregistered, buggy: insert never saves the bill, cancel throws `NotImplementedException`) — delete or wire
- No logging at all (zero `ILogger`/Serilog); errors surface via TempData only
- `PasswordHash` money regex `^\d+\.\d{0,2}$` rejects integer amounts; EF Core doesn't enforce `RegularExpression` anyway

### 2.4 MEDIUM — Frontend / Repo Hygiene (open — P2 targets)

- `wwwroot` = 58 MB vendored assets; 107 JS files; **54 jQuery copies** across `components/`, `lib/`, `js/`, `pages/`; 3 Nepali datepickers; unused chart libs
- Binaries tracked in git: `wkhtmltopdf.exe` (16.1 MB), `wkhtmltoimage.exe` (21.7 MB), `counter_billing.zip` (9.3 MB) — and `obj/` artifacts baked into the initial commit history
- Hardcoded defaults: membership dates `"2079-02-17"`/`"2084-12-05"`; ledger ids 16/19 in payment ledger filtering (with an operator-precedence bug)
- Time sources mixed: `DateTime.Now` vs `getDateTimeByTimeZone()` (NPT) — bills near midnight land on wrong day for day-close
- README is the default GitLab template

---

## 3. P0 Security Hardening — Implemented (commit `9d423fe`)

All changes preserve existing behavior/routes for legitimate users. Build verified **0 errors** after every phase.

### Phase 1 — Authentication configuration
**Files:** `LE.Web/Startup.cs`, `LE.Web/appsettings.json`, `LE.Web/Controllers/AccountController.cs`

- Cookie is the single default scheme (was: JWT default assigned *after* cookie defaults — conflicting config). JWT remains available for API controllers that opt in via `[Authorize(AuthenticationSchemes = JwtBearer...)]`
- `Cookie.HttpOnly = true`; cookie named `LE.Auth`; expiry reads `Security:CookieExpirationHours` (default 8h); sign-in `ExpiresUtc` uses the same config (was 30-day hardcode)
- JWT: key/issuer/audience from configuration (`Jwt:Key/Issuer/Audience`); token generator now emits matching `iss`/`aud` claims so validation succeeds
- `appsettings.example.json` added (real `appsettings.json` is git-ignored in this repo — contains DB password)

### Phase 2 — Global authentication requirement
**Files:** `LE.Web/Startup.cs` (global `AuthorizeFilter`), `ErrorController.cs`, `AccountController.cs` (`[AllowAnonymous]`)

- Every MVC action requires an authenticated user by default — closes the 7 anonymous controllers (S2)
- `[AllowAnonymous]` only on `AccountController` (login) and `ErrorController` (error pages must render for unauthenticated 404s/redirects)

### Phase 3 — Login hardening
**Files:** `LE.Service/.../AuthenticationServiceImpl.cs`, `LE.Common/Library/PasswordHashImpl.cs`, `PasswordHash.cs`, new `LE.Web/Helpers/LoginAttemptTracker.cs`, `Startup.cs`, `AccountController.cs`

- `validateUser` rejects `is_enabled == false` accounts
- PBKDF2: 1000 → **310,000 iterations**, HMAC-**SHA-256**, 16-byte salt, 32-byte hash, versioned format `v2:salt:hash`
  - Legacy hashes (`1000:salt:hash`) still verify and are **transparently re-hashed to v2 on next successful login** — no password resets needed
  - Malformed hashes return `false` instead of throwing
- `LoginAttemptTracker` (singleton, in-memory): 5 failed attempts per username+IP → 15-min lockout; applied to both `login` and `jwtlogin`; success clears the counter

### Phase 4 — Seeded credential neutralization
**File:** `LE.Web/Seed Data/SeedData.txt`

- Removed the committed hash (cracked to `admin`); replaced with `{ADMIN_PASSWORD_HASH}` placeholder + PowerShell generator recipe matching the v2 format
- Verified the seed file is a manual reference (executed migrations contain no `InsertData`) — existing DBs unaffected by the file change, but **any DB seeded from the old file must rotate the admin password immediately**

### Phase 5 — CSRF + IDOR
**Files:** `WoodBillingController`, `ChiranBillingController`, `FirewoodBillingController`, `FurnitureBillingController`, `CounterBillingController`, `PurchaseController`, `WoodDetailsController`, `JournalController`, `AccountController`, `Views/Account/login.cshtml`, `Views/Shared/Components/HeaderView/Default.cshtml`

- `[ValidateAntiForgeryToken]` on money-path browser POSTs: wood/chiran/firewood/furniture bill `new`, journal `new`, wood details `new`/`edit`, **login** (forms/JS already emit the `XSRF-TOKEN` header — no client changes needed)
- `counter-billing/save` **keeps** `[IgnoreAntiforgeryToken]` (verified via binary inspection: it serves the ClickOnce desktop POS client which cannot send browser tokens); it is JWT+cookie dual-scheme authorized, and identity is enforced server-side
- IDOR/audit: `counter-billing/save` sets `dto.user_id = getLoggedInAuthenticationId()`; `current-day` rejects querying another user's sales; `PurchaseController.add` assigns `user_id` **after** `_mapper.Map` (was dead code overwritten by the mapper)
- Logout: header menu now POSTs a hidden antiforgery-protected form; legacy GET `/account/logout` still works (signs out + redirects, no session record)

### Deliberate trade-offs (documented in code)

1. Desktop POS endpoint exempt from antiforgery — mitigated by JWT auth + server-derived identity
2. GET logout fallback writes no logout-session record
3. `LoginAttemptTracker` is in-memory → single-server only, resets on restart (fine for current deployment; swap for a DB/cache-backed store if load-balanced)
4. Legacy password hashes upgrade lazily — DB shows mixed formats until each user logs in once

---

## 4. Verification Performed

- Full solution `dotnet build` after each phase and at the end: **0 errors** (pre-existing ~330 warnings, mostly Magick.NET NU19xxx advisories)
- Runtime smoke-test checklist for the next manual run in Visual Studio:
  - [ ] Login page loads logged-out; login works with existing credentials (legacy hash path)
  - [ ] Logged-out access to e.g. `/billing/wood-billing` redirects to `/account/login`
  - [ ] Bill creation (wood/chiran/journal) still works — XSRF-TOKEN header already sent by the views
  - [ ] Header-menu logout (POST) lands on login page
  - [ ] Counter POS client can still authenticate (JWT with new iss/aud) and save bills

---

## 5. Git / Repository

| Item | Value |
|---|---|
| Repo | `forest-ms/` (the inner directory — the outer `ForestMS/` folder is just a wrapper, has its own unrelated empty `.git`) |
| Commits | `3e6b7fb Initial Push` (original, includes `obj/` artifacts in history) → `9d423fe security: P0 hardening …` |
| Remotes | `origin` → `https://github.com/parth1-web/ForestMS.git` (pushed, `master` tracking) · `gitlab` → original GitLab remote (kept for reference) |
| Git config | user `Roshan` / `roshannpl1234r@gmail.com` |

Gotchas hit & resolved (may recur):

- **OneDrive locks**: 45 `.git/objects` files had ReadOnly+ReparsePoint attributes → `git push` failed with "unable to open loose object". Fix: clear ReadOnly recursively over `.git/objects`, retry. Same class of issue can hit `LE.Web/obj/.../Razor` during builds (MSB3231) — delete the locked `obj` dir and rebuild.
- `appsettings.json` is git-ignored by design (`/LE.Web/appsettings.json` in `.gitignore` line ~159) — fresh clones must copy from `appsettings.example.json`

---

## 6. Deployment Action Items

1. **Set a strong `Jwt:Key`** via environment variable / secret store on the server (override the `CHANGE_ME_IN_PRODUCTION_…` placeholder)
2. **Rotate the admin password** on any database seeded from the old `SeedData.txt` (it was `admin`)
3. Copy `appsettings.example.json` → `appsettings.json` on fresh clones; fill DB password + JWT key
4. Optionally tune `Security:CookieExpirationHours` (default 8)
5. Communicate to POS desktop users: no change needed, but their next app restart picks up the new JWT (iss/aud) validation

---

## 7. Next Steps Roadmap

### P1 — Financial integrity (recommended next)
1. **Double-selling guard**: conditional update (`WHERE is_sold = false`) or optimistic concurrency on `wood_details`; reject 0-row updates
2. **Bill numbers**: replace `billing_settings` counter reads with PostgreSQL sequences (`SELECT nextval(...)`) inside the transaction
3. **Tax allocation**: compute tax per stock-type group so Σgroup tax = bill tax once; make reverse entries group-aware
4. **Cancel flow**: add day-close check + `TransactionScope` to `WoodBillServiceImpl.cancel`; restore Chiran's check
5. **Punishment check**: flip to `p.IsActive && p.PunishmentValidity.Date >= today`; move into the service layer
6. **Stock fixes**: adapter must not override `operation` (delete → decrease); non-negative stock via conditional `UPDATE ... WHERE qty >= @qty` + `CHECK (qty >= 0)`; wrap `PurchaseServiceImpl.delete` in a transaction; fix goliya duplicate-check typo + exact-match lookup + unique index
7. **Accounting voucher aliasing bugs**: never mutate an already-added ledger line (payment/receipt cancel, receipt create — discount branch); fix `calculateTransactionAmount` to sum all debits
8. **Year-close**: execute the ledger-type query and use its result; null-check running FY; verify next-FY row exists; recompute close figures server-side
9. **Day-close**: `.Any()` instead of null-check on `getQueryable()`; add `is_cancelled == false` to screen totals
10. **Membership**: save-failure must return `success = false`; server-side reconcile `Σdetails == bill total (±0.02)`; penny-remainder member splits
11. **Mutating GETs → POST** (wood bill cancel, day-close save, membership/member enable-disable-delete, purchase/stock-item delete, payment/receipt cancel) + re-enable global `AutoValidateAntiforgeryToken` once all forms carry tokens
12. **Permission enforcement**: put module ids in claims at login; authorize filter/policy backed by `role_permission_maps` (currently menu-cosmetic)

### P2 — Modernization / hygiene
1. Upgrade .NET Core 3.1 (EOL) → LTS 8.x; clear NU19xxx NuGet vulnerabilities (Magick.NET 13.9.0)
2. Remove `SaveChanges()` from `BaseRepositoryImpl` (unit-of-work); delete dead `TransactionManagerImpl` + `LE.Integration` shadow services; drop `AllowCircularDependencies`
3. Async data layer; disable lazy loading → explicit `Include`/projections; fix pager (`href`s, `NextPageService`, clamp `page`/`number_of_rows`, `OrderBy` before `Skip/Take`)
4. Serilog logging + audit trail; stop returning `ex.Message` to clients
5. `wwwroot` cleanup (58 MB, 54 jQuery copies, unused libs) via libman/bundling; consider purging `obj/` + exes from git history (needs history rewrite — coordinate)
6. Standardize timestamps on Nepal tz; validate Nepali date inputs; single `IClock` abstraction
7. Replace README with real setup docs (reference this file)

---

*Authored 2026-09-10 after the P0 session. Update this log as P1/P2 work proceeds.*

---

# Addendum — P1 Financial Integrity (2026-09-13)

> All 12 P1 roadmap items implemented. Build verified **0 errors** after every phase.
> Changes were uncommitted in the working tree as of this addendum; commit when ready.

## Transaction infrastructure (underpins everything)

- **`BaseRepositoryImpl.beginTransaction()`** — new explicit `IDbContextTransaction` primitive on the shared scoped `AppDbContext`. Every prior `TransactionScope` in the codebase was a **no-op for EF Core 3.1** (ambient System.Transactions are not enlisted; the warning was suppressed in Startup). Nested calls join via a no-op placeholder. All money-path services rewritten to use it: wood/chiran/firewood/furniture/counter sales insert+cancel, day-close, payment/receipt make+cancel, purchase make+delete, membership save/update/delete, wood-details save/delete, journal, transaction service.
- `TransactionServiceImpl.addTransaction` now rejects dates outside the running FY (both sides, B16) and throws a clear error when no running FY exists.

## P1 items — status

1. **B1 double-selling** — `WoodDetailsRepository.markSoldIfNotSold(ids, salesId)` atomic conditional `UPDATE wood_details SET is_sold=true ... WHERE ... AND is_sold=false`; insert claims after the bill row exists (real `sales_id`), rejects the whole bill when the claimed count ≠ requested. (This session fixed the WIP's pre-insert ordering bug that wrote `sales_id=0` and its non-atomic check-then-write.)
2. **B2 bill numbers** — `BillingSettingsRepositoryImpl.nextSequence()`: single-statement `INSERT ... ON CONFLICT DO NOTHING` + `UPDATE ... RETURNING` (atomic, serialized per row) + unique index on `billing_settings.key` (migration `20260911000000_p1_billing_settings_unique_key`, de-dupes legacy racy rows first, keeping highest value).
3. **B3 tax allocation** — insert and reverse entries both allocate bill tax across stock-type groups proportionally (Σ group tax = bill tax); reverse entries are now group-aware per stock-type ledger.
4. **B4 cancel flow** — wood cancel: day-close check + real transaction (was: neither). Chiran day-close check restored (B5). Counter-bill cancel: idempotency guard added.
5. **B6 punishment** — `GetMemberValidity` + `IsMemberAlreadyPunished` now `IsActive && !IsCancelled && PunishmentValidity >= today` (was inverted).
6. **B7/B8/B9/B10 stock** — adapter no longer forces `delete → increase`; availability: decrease-on-missing-row rejected, negative-qty guard, `PurchaseServiceImpl.delete` transactional; goliya duplicate-check typo fixed (`a.CompareTo(b)`) + exact-match `getByGoliaNo` (no more LIKE %..%).
7. **B11 voucher aliasing** — payment/receipt cancel + receipt create discount branches use separate DTO instances (Dr=Cr restored); **B12** `TransactionDto.calculateTransactionAmount` sums all debits.
8. **B13 year-close** — ledger group type actually queried (was comparing the SQL string literal!); next-FY row existence verified (`Id+1` assumption removed); all Dapper writes now in one `NpgsqlTransaction`, EF posting covered by the shared-context transaction; running-FY null checks.
9. **B14 day-close** — `.Any()` instead of null-check on `IQueryable`; screen totals exclude cancelled bills (match posting).
10. **B15 membership** — failure now returns `success=false`; wood bill ledger posts server-computed detail sum with ±0.02 reconcile against client total; member qty/tax splits: rounded shares + last member absorbs remainder (sums back exactly).
11. **Mutating GETs → POST + `[ValidateAntiForgeryToken]`** — wood/chiran/firewood/furniture/counter bill cancel, day-close save, membership enable/disable/delete + members delete, purchase delete, stock-item delete/enable/disable, payment cancel, receipt cancel. New `wwwroot/js/post-link.js` (loaded in `_Layout`) converts `a.post-link` anchors into token-protected form posts (token from page-hidden input or `GET /Antiforgery/Token` — new `AntiforgeryController`). All corresponding views updated. **Note:** global `AutoValidateAntiforgeryToken` was NOT re-enabled (many remaining forms lack tokens; per-action attributes cover the money paths — revisit in P2).
12. **S3 permission enforcement** — new `LE.Web/Helpers/ModulePermissionFilter.cs` (registered as a global `TypeFilter`): resolves the request's `area/controller` prefix against `dynamic_menus.web_url` (navbar parity — users can't open what they can't see) and checks `role_permission_maps` via the user's roles; falls back to the area's `module_code` set for unmapped URLs (`module_code` is not unique — Billing + Utilities both use "billing"); JWT **counter-POS endpoints under `/billing/counter-billing/*` are exempt** (documented trade-off, same client as the P0 antiforgery exemption — staff logins hold no browser roles); root pages (`/home`) pass with global auth only. Denials → `ForbidResult` → cookie `AccessDeniedPath`/`/error/403` → new `Views/Error/Forbidden.cshtml`. `ModuleRepository.getByCode` added (set-based lookups use `getQueryable`).

## Other fixes included in the WIP (from the register)

- **B17** day book print/PDF honor ToDate (was FromDate — screen and print showed different days)
- **B18** purchase list: `is_deleted` filter applied before `Skip/Take` (no more short pages), `OrderByDescending` before pagination

## Deliberate trade-offs (P1)

1. POS endpoint module-check exemption (above) — mitigated by JWT auth + server-derived identity from P0
2. Antiforgery stays per-action (money paths covered) until a P2 sweep adds tokens to every form
3. `markSoldIfNotSold` interpolates ids into raw SQL — ids are `long`s from server-side DTOs (not user strings); safe
4. Year-close posts the EF leg on the shared context while Dapper legs run on their own connection/transaction — both commit only if both succeed, but it is not one distributed transaction (two-phase commit is out of scope on PostgreSQL/Npgsql here)
5. GET `/account/logout` legacy fallback retained (P0 decision, unchanged)

## Verification performed this session

- Full solution build: **0 errors** (348 pre-existing warnings)
- Audited every WIP diff against the defect register before accepting it
- Fixed two WIP defects: pre-insert double-sell guard (sales_id=0 + TOCTOU) and the module_code uniqueness landmine in the permission filter

## Runtime smoke-test checklist (manual, next run in Visual Studio)

- [ ] Login → navbar renders as before (permission filter does not affect rendering)
- [ ] Bill creation (wood/chiran/counter) — wood bill sets `sales_id` on wood_details rows (verify in DB)
- [ ] Cancel flows reject after day-close and are transactional
- [ ] `a.post-link` buttons work (cancel/delete/enable/disable) including confirm dialogs
- [ ] User without Billing module opening `/billing/wood-billing` directly → Forbidden page
- [ ] POS desktop client still authenticates and saves counter bills (counter-billing exemption)
- [ ] Apply migration `20260911000000_p1_billing_settings_unique_key` before first use (EF `Database.Migrate` or `dotnet ef database update`)

## Open items carried to P2

- S3 refinement: menu-level (not just module-level) permissions; antiforgery global enablement
- S11 HTTPS/HSTS enforcement; stop returning `ex.Message` to clients
- Everything already listed under P2 in the main log above

---

# Addendum 2 — P1 Runtime Smoke Test (2026-09-13)

> Executed against a live PostgreSQL 18 `Forest` DB on localhost with the app
> running under `dotnet exec` (Development env). Migration
> `20260911000000_p1_billing_settings_unique_key` applied cleanly via
> `dotnet ef database update` (index `IX_billing_settings_key` created).

## Defects found by the smoke test (fixed in this session)

1. **`NestedDbContextTransaction` TypeLoadException** — first bill insert threw
   "Method 'CommitAsync' does not have an implementation": `LE.Common` compiled
   against EF Core **2.1.0** while the app resolves EF Core 3.1.32 at runtime
   (3.1's `IDbContextTransaction` has `CommitAsync`/`RollbackAsync`). Fixed by
   aligning `LE.Common.csproj` to EF Core 3.1.32 and implementing the async
   members on the nested no-op placeholder.
2. **Misleading 405 on antiforgery failures** — `UseStatusCodePagesWithReExecute`
   replayed failed POSTs against the GET-only `/error/{code}` route, turning
   every antiforgery 400 into a confusing `405 Allow: GET`. Fixed:
   `ErrorController` now `AcceptVerbs(GET, POST, PUT, DELETE, PATCH)`. Real 405s
   (GET on POST-only actions) still surface correctly.
3. **`/debug/routes` temporary endpoint** (added for diagnosis) removed.

## Test matrix — results

| # | Test | Result |
|---|---|---|
| 1 | Anonymous → `/billing/wood-billing` | 302 → `/account/login?ReturnUrl=…` ✓ |
| 2 | Login page + form token render | 200 ✓ |
| 3 | Login POST (admin, correct+wrong token paths) | 302 → `/home` ✓ / 400 on bad token ✓ |
| 4 | Wood bill create (member sale, wood 3 @1000, qty 1.89) | bill 4 created; ledger Dr Cash/Cr Lakadi 1890 balanced; member split exact ✓ |
| 5 | Double-sell guard (resubmit same wood) | rejected: "Goliya 3 is already sold", no partial state ✓ |
| 6 | B15 reconcile (total 999 vs details 500) | rejected: "Bill total (999) does not match the sum of its details (500)." ✓ |
| 7 | Bill cancel via GET | 405 ✓ (mutating GET blocked) |
| 8 | Bill cancel POST w/o token | 400 ✓ |
| 9 | Bill cancel POST w/ form token (post-link.js path) | 302 → report; bill `is_cancelled=t`, wood `is_sold=f` restored, reverse ledger Dr Lakadi/Cr Cash balanced, member txn cancelled ✓ |
| 10 | Bill cancel POST w/ header token + JSON body (AJAX path) | 400 only for header+form-encoded curl artifact; header+JSON verified working in test 4 ✓ |
| 11 | `GET /Antiforgery/Token` | 200 + token + cookie pair ✓ |
| 12 | Permission filter: restricted user (role → Accounting module only) | billing/membership/inventory → 302 `/error/403` → "Access Denied" page ✓; accounting/ledgers → 200 ✓; `/home` → 200 ✓ |
| 13 | JWT login (`/account/jwtlogin`) | 200, token w/ iss=LE.Web, aud=LE.Clients ✓ |
| 14 | POS `current-day` w/ JWT (own id vs other user id) | 200 own data ✓ / other id: "not authorized to view sales of another user" (IDOR guard) ✓ |
| 15 | Unauthenticated POS save attempt | blocked (no DB write; error-page render NRE is a pre-existing cosmetic bug in `HeaderViewComponent` for anonymous users) |
| 16 | Bill insert after day-close | "Day is already closed. You cannot perform transactions in this date." ✓ |
| 17 | Bill cancel after day-close (B4) | blocked — bill not cancelled, no reverse entry ✓ (302 is the app's standard TempData-error redirect) |
| 18 | Migration applied | `__EFMigrationsHistory` + unique index ✓ |

Test data created for the run (restricted user/role, day-close row, test bills)
was removed afterwards; DB restored to pre-test state.

## Known issues surfaced (not fixed, logged for P2)

- `HeaderViewComponent.InvokeAsync` NREs when rendering for anonymous requests
  (error pages reached without a login). Cosmetic; does not bypass any control.
- `WoodBillingController.Index` action references a view (`Index.cshtml`) that
  does not exist — pre-existing; nothing links to it (menu uses `new/{type}`).
- Smoke-tooling note: PowerShell `Set-Content -Encoding UTF8` adds a BOM that
  breaks `[FromBody]` JSON model binding (members deserialized to null). The
  UI's own AJAX is unaffected.

---

# Addendum 3 — P2 Core Architecture (2026-09-14)

> First P2 tranche: dead-code removal, DI hygiene, logging, error sanitization,
> HTTPS enforcement. Build verified **0 errors** after every phase. The unit-of-work
> migration (SaveChanges removal) is deliberately deferred — see "Deferred" below.

## What was done

### Phase A — Dead code removal
- **`TransactionManagerImpl` + `TransactionManager` interface deleted** (`LE.Context/Helper/`). Zero references anywhere; class was broken by design ([ThreadStatic] counter, commit-before-save, negative counts possible).
- **`LE.Integration` project deleted entirely** (solution, `LE.Web.csproj` reference, Startup registrations, Dockerfile COPY). It contained unregistered shadow services (billing wood-bill/member service clones: insert never saved the bill, cancel threw `NotImplementedException`) and `AccountTransactionHelper` which was registered but had **no live consumers** (verified: nothing outside LE.Integration referenced it). ~200 lines of dead money-path code gone.

### Phase B — DI / Autofac hygiene
- **`AutofacModule` rewritten**: assembly-scans all non-abstract `*Controller` types with `PropertiesAutowired()` — replaces the hand-written list that missed several `BaseController`-derived controllers (latent NREs on `getLoggedInUserId()`), double-registered the abstract `BaseController` itself, and used `AllowCircularDependencies` (now dropped).
- **`HeaderViewComponent` anonymous-request NRE fixed** (smoke-test known issue): renders with `userDetail = null` when no authenticated user instead of throwing on error pages.
- Note: `LE.Web.Autofac` namespace collides with the `Autofac` package namespace — base class must be `global::Autofac.Module` (documented in the file).

### Phase C — Serilog logging
- Packages: `Serilog.AspNetCore 3.4.1` + `Serilog.Sinks.File 4.1.0` (3.1-compatible).
- `Program.cs`: bootstrap console logger (host start/crash/fatal), `UseSerilog` with config from `Serilog:*` appsettings keys (`MinimumLevel`, `File:path` default `logs/le-web-.log`, `File:retainedFileCountLimit` default 31). `Microsoft`/`System` at Warning.
- `Startup.Configure`: `UseSerilogRequestLogging()` (method, path, status, timing, user id) after forwarded-headers.

### Phase D — Exception-message sanitization (S11 second half)
- New **`LE.Web/Helpers/ExceptionMessageHelper.cs`**: `CustomException` (and subclasses) messages shown to users as-is — they are intentionally-written user-facing messages; **all other exceptions get a generic message** (stack contents, PG constraint names, file paths no longer leak). Wire shapes unchanged: alerts/TempData, `{error, responseText}` AJAX, `JsonWrapper` `{error}` JSON (POS client unaffected — same field structure), legacy `{success, message}` shape.
- **48 leak sites fixed across 38 controllers** (all `ex.Message`/`e.Message` in LE.Web controllers). Remaining `.Message` references in codebase: none outside the helper; vendored JS libs only (their own internals).
- Full details of unexpected exceptions are now in the Serilog log for support.

### Phase E — HTTPS enforcement (S11 first half) + misc
- `app.UseHsts()` in the **production** branch (was commented out); `app.UseHttpsRedirection()` restored — both **config-gated**: `Security:EnableHsts` (default true), `Security:EnableHttpsRedirect` (default true). Gates exist for proxies terminating TLS and plain-HTTP LAN deployments (POS client).
- `appsettings.example.json` updated with `Serilog` + new `Security` keys.
- **`HomeController.Index` user-lookup bug fixed**: was passing authentication id to `_userRepo.getById()` (only worked because seed ids coincide) — now uses `getLoggedInUserId()`.

## Deliberate trade-offs (P2 tranche 1)

1. HSTS/HTTPS-redirect default **on** in config but HSTS only applied in Production env — dev HTTP workflow unaffected; document the gates for the production reverse-proxy setup.
2. `ExceptionMessageHelper` shows generic text for unexpected exceptions — operational messages (day-closed, already-sold, reconciliation failures…) are all `CustomException` subclasses and pass through; verify no user-facing message was lost during smoke test.
3. Serilog file sink writes to `logs/` under the app working dir — Docker volume/hosting setup should mount/rotate that path (retention 31 days default).
4. `LE.Integration` deleted rather than wired — its features were broken clones; if integration billing is ever needed, reimplement against current service APIs.

## Process note — encoding incident (fixed)

The bulk sed-style replacement script used PowerShell 5.1 `Set-Content` without
`-Encoding`, which wrote **ANSI** and corrupted em-dashes (U+2014) in UTF-8-BOM
files (`ReportController.cs`, `MemberPunishmentController.cs` — my own P1
comments). Caught by a byte-level audit vs `git show HEAD`; both files restored
from HEAD with UTF-8 BOM and the replacements re-applied. **All 38 modified
files verified: non-ASCII char counts match HEAD exactly.** Lesson recorded for
future sessions: never use bare `Set-Content` on repo files in PS 5.1 — use
`[IO.File]::WriteAllText` with explicit `UTF8Encoding($true)`.

## Deferred from this tranche (P2 continues)

1. **Unit-of-work migration** (remove `SaveChanges()` from `BaseRepositoryImpl` insert/update/delete): touches **219 repo-write call sites** + ~100 legacy ambient `TransactionScope`s (currently no-ops that work only because each write self-commits). Converting all of them to explicit `beginTransaction`/commit — with a runtime smoke test like P1's — is a dedicated session. Money paths already use real transactions from P1, so the risk is in the long tail of setup/admin/catalog services.
2. .NET 3.1 → LTS 8.x upgrade (clears Magick.NET NU19xxx advisories; blocked until UoW lands to avoid triple merge conflicts)
3. Async data layer; lazy-loading disable; pager fixes (`href`s, `NextPageService`, clamp `page`, `OrderBy` before `Skip/Take`)
4. `wwwroot` cleanup (58 MB, 54 jQuery copies); git-history purge of binaries; real README
5. Menu-level permissions (beyond module-level); global `AutoValidateAntiforgeryToken`
6. Nepali timezone/`IClock` standardization

## Verification performed this session

- Full solution build after each phase and at end: **0 errors** (~348 pre-existing warnings, unchanged)
- `git status` audit: only intended files changed; encoding byte-audit of all 38 touched files vs HEAD
- No runtime smoke test this session (no DB changes, no route/shape changes intended) — recommend a quick manual pass: login, navbar, one bill create/cancel, POS JWT login, forbidden-page for restricted user, and check `logs/` is populated

---

# Addendum 4 — P2 Unit-of-Work Migration (2026-09-14/15, completed from WIP)

> The UoW migration deferred in Addendum 3 was found in progress in the working
> tree (96 modified files, uncommitted) and completed this session.
> Build verified **0 errors** after every fix.

## What the WIP did (audited and accepted)

- **`BaseRepositoryImpl`**: `update()`/`delete()` no longer call `SaveChanges()` —
  tracked changes flush via the new `saveChanges()` primitive (single-write
  operations) or before `tx.Commit()` (transactional operations). `insert()`
  still flushes because PKs are DB-generated (PostgreSQL `RETURNING`) and callers
  read `entity.<pk>` immediately after insert to wire child rows.
- **`saveChanges()` added to `BaseRepository` interface** and every repo interface
  signature (the bulk of the 96 files).
- **All services rewritten**: every remaining ambient `TransactionScope` (the
  ~100 legacy no-op scopes) removed and replaced with real
  `beginTransaction()` + `saveChanges()` + `Commit()` on the shared scoped
  `AppDbContext`; nested `beginTransaction` calls join the outer transaction
  via the no-op placeholder (introduced in the P1 EF-version fix).

## Defects found in the WIP by audit (fixed this session)

1. **`updateBalanceAmount` silent no-op** (`TransactionDetailServiceImpl`) — the
   admin "update ledger balances" endpoints (`/accounting/update-balance/{id}`,
   `update-balance-all`) call `updateLedgerBalances` → `this.update()`, which no
   longer self-saves, and nothing else in that chain flushed. Fixed: wrapped in
   `beginTransaction()` + `saveChanges()` + `Commit()`.
2. **Voucher-number race + no-flush** (`AccountSettingsRepositoryImpl.getTransactionSequence`) —
   accounting vouchers use the same racy counter-row read-modify-write that
   P1/B2 fixed for bill numbers; under UoW it was also never flushed (relying on
   the caller's `saveChanges`). Fixed: atomic `INSERT ... ON CONFLICT DO NOTHING`
   + `UPDATE ... RETURNING` (same pattern as `billing_settings`).
3. **Missing unique index on `account_settings.key`** — required for the
   `ON CONFLICT` insert to be race-free. New migration
   `20260914000000_p2_account_settings_unique_key` (de-dupes legacy racy rows
   keeping the highest value, then creates `IX_account_settings_key`), snapshot
   updated. **Run `dotnet ef database update` before first use.**
4. **`LE.Account.Context` shadow project deleted** — not in the solution, not
   referenced by any csproj, contained stale duplicate repo implementations
   (same profile as the deleted `LE.Integration`). Its
   `getTransactionSequence` variant still had the old racy non-flushing code.

## Audit methodology (reusable)

- Script scanned all `*Repo*.update()/delete()` call sites (95) and flagged those
  whose enclosing method had no `saveChanges()`/`Commit()` — 11 hits, all traced
  to private helpers invoked inside committing outer transactions (verified safe).
- Manual pass over non-`Repo`-named writes, `this.update/insert/delete` self-calls,
  and direct `appDbContext.Set<T>().Update/Remove` — none remaining.
- `StockItemAvailabilityServiceImpl.saveOrUpdate` deliberately flushes inside its
  loop: same stock item on multiple bill lines requires each iteration to see
  the previous write (stale-read guard documented in the file).

## Verification performed this session

- Full solution build after each fix and at end: **0 errors** (~332–350 warnings,
  all pre-existing Magick.NET NU19xxx advisories; count varies with incremental
  build passes)
- Encoding byte-audit on every file touched (BOM preserved; no ANSI corruption)

## Runtime smoke-test checklist (manual, next run)

- [ ] Apply migration `20260914000000_p2_account_settings_unique_key` before first use
- [ ] Journal/voucher creation (uses the new atomic `getTransactionSequence`) —
      verify `transaction_id` increments and concurrent vouchers don't collide
- [ ] Ledger update-balance endpoints actually persist (were the silent no-op)
- [ ] Re-run P1 smoke matrix (bill create/cancel, day-close, POS save) — UoW
      changed every write path's flush timing
- [ ] Setup screens (org-setup, ledger-setup, fiscal-year-setup) save correctly

## Deliberate trade-offs (P2 UoW)

1. `insert()` still self-flushes (DB-generated PKs are read immediately by
   callers); a future pass could return generated ids explicitly and defer
   these too — out of scope for 219 call sites.
2. `markSoldIfNotSold` flushes pending tracked changes before its raw UPDATE
   (unchanged from P1) — noted as intentional in the file.
3. Single-write `saveChanges()` calls are non-transactional by design (single
   row, no multi-step invariant) — matching pre-UoW behavior exactly.

## Remaining P2 backlog (unchanged from Addendum 3)

- .NET 3.1 → LTS 8.x upgrade (Magick.NET advisories) — unblocked now that UoW landed
- Async data layer; lazy-loading disable; pager fixes
- `wwwroot` cleanup; git-history purge of binaries; real README
- Menu-level permissions; global `AutoValidateAntiforgeryToken`
- Nepali timezone/`IClock` standardization

---
