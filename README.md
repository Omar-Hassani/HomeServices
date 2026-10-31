# HomeServices

An Arabic-first (RTL) marketplace that connects customers in Syria with verified home-service
providers: electricians, plumbers, painters, AC and refrigerator technicians, furniture movers,
cooks, home cleaners and general maintenance.

Customers search by service, area, price and rating, send a request with an address and a
preferred time slot, follow it step by step, and review the provider once the job is done.
Providers manage a professional profile, prices and work areas, and handle incoming requests.
Admins verify providers and moderate the platform.

---

## Contents

- [Features](#features)
- [Tech stack](#tech-stack)
- [Architecture](#architecture)
- [Getting started (local development)](#getting-started-local-development)
- [Trying the full flow](#trying-the-full-flow)
- [Configuration](#configuration)
- [Database and migrations](#database-and-migrations)
- [Deploying to production](#deploying-to-production)
- [Security notes](#security-notes)
- [Conventions](#conventions)
- [Known limitations and next steps](#known-limitations-and-next-steps)

---

## Features

**Visitors**
- Home page with search, service categories and top-rated providers.
- Provider search with filters (category, service, governorate, area, price range, minimum rating,
  availability), six sort orders and pagination. Colloquial search terms such as "سباك" or
  "كهربجي" match through per-category search keywords.
- Category pages and public provider profiles (services and prices, work areas, portfolio, reviews).

**Customers**
- Registration and login, profile and photo.
- Booking form: service, area (only areas the provider covers), address, date (today to 60 days
  ahead, in Syria time) and time slot.
- "My orders" with tabs, a status stepper and a full history timeline; cancel before work starts.
- One review per completed order (1–5 stars and an optional comment).

**Providers**
- Dashboard with verification status, a profile-completion checklist and an availability switch.
- Professional profile, services and prices, work areas, portfolio images (up to 12).
- Incoming orders: accept (optionally adjusting the price), reject with a reason, start, complete
  with a final price, or cancel with a reason.
- Reviews page.

**Admins** (`/Admin`)
- Overview KPIs and an order-status breakdown.
- Provider verification: approve (complete profiles only), reject or suspend with a required note,
  reinstate.
- User management: search, filter, deactivate/reactivate (ends active sessions within a minute).
- Read-only order browser, review moderation (hide/unhide, ratings recalculated automatically).
- Catalog management: categories, services, search keywords, governorate areas.

**Cross-cutting**
- In-app notifications for every order event, new reviews and verification decisions, with a header
  bell, unread badge (polled every minute) and a notifications page.
- Contact details and the full address are revealed to the other party only after acceptance.

---

## Tech stack

| Area | Technology |
|---|---|
| Runtime | .NET 9 (SDK 9.0.x) |
| Web | ASP.NET Core MVC, Razor views, Areas (`Admin`, `Provider`, `Customer`) |
| Data | Entity Framework Core 9, SQL Server (LocalDB for development) |
| Identity | ASP.NET Core Identity with roles (`Admin`, `Provider`, `Customer`) |
| UI | Bootstrap 5 RTL, custom design system (CSS tokens), Font Awesome 6 Free, IBM Plex Sans Arabic and Readex Pro fonts — all self-hosted |
| Tooling | `dotnet-ef` and LibMan as local tools (`.config/dotnet-tools.json`) |

No CDN is used at runtime: fonts, icons and libraries are served from `wwwroot`.

---

## Architecture

A layered solution with four projects. Dependencies point inwards only.

```
HomeServices.sln
└── src/
    ├── HomeServices.Domain          Entities, enums, order workflow rules (no dependencies)
    ├── HomeServices.Application     Service interfaces, DTOs, Result types, constants
    ├── HomeServices.Infrastructure  EF Core DbContext, configurations, migrations, seed data,
    │                                service implementations, file storage, Identity setup
    └── HomeServices.Web             Controllers, areas, views, view models, CSS/JS, Program.cs
```

Key design points:

- **Order lifecycle** is defined once in `Domain/Workflow/ServiceRequestWorkflow.cs`: which status
  can move to which, and whether the customer or the provider may perform each action.
  `ServiceRequest.Status` is an optimistic-concurrency token, so two simultaneous actions on the same
  order cannot both apply.
- **Ratings** are recalculated from visible reviews inside the same transaction as the change, never
  incremented, so hiding a review always yields a correct average.
- **Notifications** go through `INotificationService`, which stores them and then forwards to any
  registered `INotificationChannel` (email/SMS/push can be added without touching callers).
  A failed notification never fails the business operation.
- **File storage** is behind `IFileStorageService` (local `wwwroot/uploads` today). Image type is
  detected from file signatures (JPEG/PNG/WebP, max 5 MB) and GPS data is stripped from JPEGs.
- **Reference data** (14 governorates, 87 areas, 8 categories, 31 services) is seeded through
  migrations; admins extend it from the admin panel.

---

## Getting started (local development)

### Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server **LocalDB** (installed with Visual Studio, or via the SQL Server Express installer).
  Any SQL Server instance works if you change the connection string.

### 1. Restore tools and packages

```bash
git clone <repository-url> HomeServices
cd HomeServices
dotnet tool restore        # installs dotnet-ef and libman locally
dotnet restore
```

Front-end libraries (Bootstrap, jQuery, Font Awesome, fonts) are committed under `wwwroot`.
To re-download them, run `dotnet libman restore` inside `src/HomeServices.Web`.

### 2. Configure the first admin account

The admin account is created on startup from configuration. Credentials are **never** stored in the
repository — use User Secrets in development:

```bash
cd src/HomeServices.Web
dotnet user-secrets set "SeedAdmin:Email" "admin@homeservices.local"
dotnet user-secrets set "SeedAdmin:Password" "<a strong password with at least one digit>"
dotnet user-secrets set "SeedAdmin:FullName" "مدير المنصة"
```

If these values are missing, the app still starts and logs a warning; you can add them later and
restart.

### 3. Run

```bash
cd src/HomeServices.Web
dotnet run
```

Open <http://localhost:5187> (or <https://localhost:7189> with `dotnet run --launch-profile https`).

On the first run in the `Development` environment the app:

1. creates the `HomeServicesDb` database on LocalDB and applies all migrations
   (`Database:ApplyMigrationsOnStartup` is `true` in `appsettings.Development.json`);
2. seeds the roles and the reference data;
3. creates the admin account from User Secrets.

A health endpoint is available at <http://localhost:5187/health> (`Healthy` when the database is
reachable).

---

## Trying the full flow

1. **Register a provider** — open *انضم كمقدم خدمة* (`/Account/Register?type=provider`). You land
   on the provider dashboard (`/Provider`).
2. **Complete the profile** — headline, bio (30+ characters), city, at least one service with a
   price and at least one work area. The checklist on the dashboard shows what is missing.
3. **Approve it as admin** — log in with the admin account, open `/Admin/Providers?status=Pending`
   and click *توثيق*. The provider now appears in search and receives a notification.
4. **Book as a customer** — register a customer account (`/Account/Register`), find the provider
   from the home page or `/Providers`, and click *اطلب الخدمة*.
5. **Handle the order as the provider** — `/Provider/Orders`: accept, start, complete.
6. **Review as the customer** — open the completed order from *طلباتي* and leave a rating.
7. **Moderate as admin** — `/Admin/Reviews` lets you hide a review; the provider's average updates.

---

## Configuration

Settings come from `appsettings.json`, `appsettings.{Environment}.json`, User Secrets (development
only) and environment variables. In environment variables, `:` becomes `__`
(e.g. `ConnectionStrings__DefaultConnection`).

| Key | Default | Purpose |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | LocalDB `HomeServicesDb` | SQL Server connection string. **Override in production.** |
| `Database:ApplyMigrationsOnStartup` | `true` in Development, `false` otherwise | Apply pending migrations at startup. Keep `false` in production and run the migration script instead. |
| `SeedAdmin:Email` / `SeedAdmin:Password` / `SeedAdmin:FullName` | — | First admin account, created once if the email does not exist. |
| `DataProtection:KeysPath` | empty | Directory where data-protection keys are persisted. **Required in production** (see below). |
| `ReverseProxy:Enabled` | `false` | Trust `X-Forwarded-For`/`X-Forwarded-Proto` when running behind Nginx/IIS/a load balancer. |
| `ReverseProxy:KnownProxies` | `[]` | Extra proxy IP addresses to trust (loopback is trusted by default). |
| `AllowedHosts` | `*` | Set to your domain(s) in production, e.g. `homeservices.sy;www.homeservices.sy`. |
| `Logging:Console:FormatterName` | `json` in Production | Structured JSON logs for log collectors. |

---

## Database and migrations

Migrations live in `src/HomeServices.Infrastructure/Data/Migrations`. Run these commands from the
repository root.

```bash
# Add a migration after changing entities or configurations
dotnet ef migrations add <Name> \
  --project src/HomeServices.Infrastructure \
  --startup-project src/HomeServices.Web \
  --output-dir Data/Migrations

# Apply migrations to the configured database
dotnet ef database update \
  --project src/HomeServices.Infrastructure \
  --startup-project src/HomeServices.Web

# Generate an idempotent SQL script for production
dotnet ef migrations script --idempotent -o migrate.sql \
  --project src/HomeServices.Infrastructure \
  --startup-project src/HomeServices.Web
```

> When running `sqlcmd` manually against this database, pass `-I` (QUOTED_IDENTIFIER ON); Identity
> tables use filtered indexes that require it.

---

## Deploying to production

### 1. Publish

```bash
dotnet publish src/HomeServices.Web -c Release -o ./publish
```

User uploads (`wwwroot/uploads`) are excluded from the build and the publish output on purpose —
they are runtime data that lives on the server.

### 2. Prepare the database

Create the database, then run the idempotent script generated above (`migrate.sql`) with your usual
tooling. Reference data (governorates, areas, categories, services) is included in the migrations.

### 3. Set environment variables

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Server=...;Database=HomeServicesDb;User Id=...;Password=...;Encrypt=True"
SeedAdmin__Email=admin@your-domain
SeedAdmin__Password=<strong password>
SeedAdmin__FullName=مدير المنصة
DataProtection__KeysPath=/var/homeservices/keys
AllowedHosts=your-domain;www.your-domain
ReverseProxy__Enabled=true          # only if behind a reverse proxy
```

Remove the `SeedAdmin__Password` variable after the first successful start if you prefer; the account
is only created once.

### 4. Persist what must survive restarts and deployments

| Path | Why |
|---|---|
| `DataProtection:KeysPath` directory | Encrypts login cookies and antiforgery tokens. Without persisted keys every restart logs all users out and invalidates open forms. Must be writable by the app and shared between instances if you run more than one. |
| `wwwroot/uploads` | Profile photos and portfolio images. Keep it outside the deployment folder (or on a mounted volume) and include it in backups. |

### 5. Run behind a reverse proxy (example: Nginx on Linux)

Run the app with Kestrel (e.g. as a `systemd` service with `ASPNETCORE_URLS=http://127.0.0.1:5000`)
and terminate HTTPS in Nginx:

```nginx
server {
    listen 443 ssl http2;
    server_name your-domain;

    ssl_certificate     /etc/letsencrypt/live/your-domain/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/your-domain/privkey.pem;

    client_max_body_size 8m;   # image uploads are limited to 5 MB by the app

    location / {
        proxy_pass         http://127.0.0.1:5000;
        proxy_set_header   Host $host;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```

Set `ReverseProxy__Enabled=true` so the app sees the real client IP (used by rate limiting) and the
original HTTPS scheme. On Windows/IIS, install the ASP.NET Core Hosting Bundle and point the site at
the publish folder; the same settings apply.

### 6. Monitor

- `GET /health` returns `Healthy` (200) when the app can reach the database — use it for uptime
  checks and load-balancer probes.
- Logs are written to the console as JSON in Production.

---

## Security notes

- **Authorization**: role-protected areas; every order, review, notification and profile operation
  is scoped to the current user on the server, not just hidden in the UI.
- **CSRF**: antiforgery validation on all unsafe requests (global filter).
- **Open redirects**: only local return URLs are followed.
- **Brute force and abuse**: rate limiting on login/registration (10/min per IP), uploads
  (20 per 10 min per user) and order/review submissions (15 per 10 min per user); Identity lockout
  after 5 failed logins.
- **Account suspension**: deactivating a user rotates their security stamp, which is re-validated
  every minute, so open sessions end promptly.
- **Headers**: Content-Security-Policy (self-only, with a single hashed inline script),
  `X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff`, `Referrer-Policy`,
  `Permissions-Policy`, HSTS in production.
- **Uploads**: type detected from file signatures, size-limited, served with `nosniff`; GPS/XMP
  location data stripped from JPEGs; deletion restricted to the uploads folder.
- **Input safety**: admin-editable values that end up in HTML attributes (category icon class and
  color) are validated against strict patterns.
- **Secrets**: no passwords or connection secrets in the repository; development uses Windows
  authentication for LocalDB and User Secrets for the admin password.

> If you change the inline script in `Views/Shared/_Layout.cshtml`, update its hash in
> `src/HomeServices.Web/Security/SecurityHeadersMiddleware.cs`, otherwise the browser will block it.

---

## Conventions

- **Language and layout**: Arabic UI, `dir="rtl"`, Bootstrap RTL build.
- **Culture**: `ar-SY` with the Gregorian calendar and Latin digits; numeric inputs use
  `inputmode="numeric"` text fields so browsers do not render Arabic-Indic digits.
- **Time**: dates are stored in UTC and shown in Syria time (`Application/Common/SyriaTime.cs`);
  booking date windows are computed in Syria time.
- **Currency**: Syrian pound (ل.س), formatted as `75,000 ل.س`.
- **Privacy**: public reviews show the customer's first name and last initial only.

---

## Known limitations and next steps

- **No automated test project yet.** All phases were verified end-to-end against a running instance;
  adding unit tests (workflow rules, rating calculation, phone normalization, JPEG scrubbing) and
  integration tests (`WebApplicationFactory`) is the recommended next step.
- **Notifications are in-app only.** Email/SMS/push can be added as `INotificationChannel`
  implementations.
- **Location scrubbing covers JPEG only.** PNG and WebP uploads keep any embedded metadata.
- **Catalog counts are cached for up to 10 minutes** when a provider changes their own services;
  admin changes invalidate the cache immediately.
- **Local file storage** suits a single server; for multiple instances, implement
  `IFileStorageService` against object storage and share the data-protection keys.
