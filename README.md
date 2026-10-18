# WeddingApi

> A multi-role backend platform for discovering, booking, and paying for wedding services — connecting brides & grooms with venues, photographers, caterers, and every other vendor involved in planning a wedding.

**منصة لتخطيط وحجز خدمات الأفراح إلكترونيًا** — تجمع العرسان مع مزودي الخدمات (قاعات، تصوير، كاترينج، فساتين، وغيرها) في مكان واحد.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![JWT](https://img.shields.io/badge/Auth-JWT-000000?logo=jsonwebtokens)](https://jwt.io/)
[![License](https://img.shields.io/badge/License-MIT-green)](#license)

---

## Table of Contents

- [Overview](#overview)
- [User Roles](#user-roles)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration & Secrets](#configuration--secrets)
- [API Overview](#api-overview)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

WeddingApi is a RESTful backend for a wedding-planning marketplace. Brides and grooms browse and book services, service providers manage their offerings and bookings, and a staff hierarchy (Admin / Supervisor) oversees the whole operation — all through a single, role-aware API.

The project follows a clean, layered architecture to keep business rules independent of frameworks and infrastructure, making it straightforward to test, extend, and swap out individual pieces (e.g. database provider, media host) without rewriting the domain logic.

## User Roles

| Role | Responsibilities |
|---|---|
| **Admin** | Full control: manages supervisors, approves/rejects service providers, oversees all bookings & payments, monitors published content. |
| **Supervisor** | Day-to-day monitoring of bookings, quick responses to customer inquiries, escalates issues to Admin. |
| **Service Provider** | Publishes services/packages with pricing, manages bookings and availability, tracks payments received. |
| **Client (Bride / Groom)** | Browses and books services, tracks booking progress, pays via multiple methods; bride and groom accounts can be linked together. |

## Features

- 🔐 **JWT authentication** with ASP.NET Core Identity, role-based authorization (`Admin`, `Supervisor`, `Provider`, `Client`)
- 👰🤵 **Linked bride/groom accounts** sharing a wedding plan
- 🏷️ **Service catalog** — each provider can list multiple priced packages (`Service` entity), not just a single flat rate
- 📅 **Booking lifecycle** — `Pending → Confirmed → Completed / Cancelled / Refunded`, with calendar and search queries
- 💳 **Payments** — partial/installment payments per booking, with automatic booking confirmation once fully paid
- 🖼️ **Media management** — Cloudinary-backed image/video uploads for provider portfolios
- 📊 **Admin dashboard** — bookings, revenue, and client KPIs at a glance
- 🧭 **Provider approval workflow** — pending → approved/rejected, gated at login
- 📄 **Swagger / OpenAPI** documentation out of the box

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 9 / ASP.NET Core Web API |
| ORM | Entity Framework Core 9 (Code-First + Migrations) |
| Database | Microsoft SQL Server |
| Auth | ASP.NET Core Identity + JWT Bearer tokens |
| Media storage | Cloudinary |
| API docs | Swashbuckle (Swagger UI) |

## Architecture

The solution is split into three projects following a **Clean / N-Layer Architecture**:

```
WeddingApi.core            →  Domain layer: Entities, DTOs, repository & service interfaces
                               (no dependency on EF Core or ASP.NET — pure business types)

WeddingApi.infrastructure  →  Implementation layer: EF Core DbContext, repositories,
                               Unit of Work, external services (Auth, Cloudinary), migrations

WeddingApi.web             →  Presentation layer: Controllers, Program.cs / startup,
                               Swagger, configuration
```

Controllers depend only on `IUnitOfWorks` (never on `DbContext` directly), which exposes one repository per aggregate (`Bookings`, `Clients`, `Payments`, `Services`, `Media`, `ServiceProviders`) through lazy initialization — keeping data access swappable and unit-testable.

## Project Structure

```
WeddingApi/
├── WeddingApi.core/
│   ├── Entities/            # Booking, Client, Payment, Service, ServiceProvider, ...
│   ├── DTOs/                # Request/response contracts, grouped by feature
│   ├── Interfaces/          # IBookingRepository, IUnitOfWorks, IAuthService, ...
│   └── Common/               # PagedResult<T>, QueryParams
│
├── WeddingApi.infrastructure/
│   ├── Data/                 # WeddingDbContext
│   ├── Repositories/         # EF Core implementations
│   ├── UnitOfWorks/          # UnitOfWork (aggregates all repositories)
│   ├── Services/             # AuthService (JWT issuing, registration, approvals)
│   ├── Seeder/                # DbInitializer (roles + default admin)
│   └── Migrations/
│
└── WeddingApi.web/
    ├── Controllers/           # Auth, Bookings, Clients, Payments, Services,
    │                          # ServiceProvider, Media, Dashboard
    └── Program.cs              # DI, JWT, CORS, Swagger, Identity setup
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote instance)
- A [Cloudinary](https://cloudinary.com/) account (free tier is enough) for media uploads

### Setup

```bash
# 1. Clone the repository
git clone https://github.com/Rasem7/WeddingApi.git
cd WeddingApi/WeddingApi.web

# 2. Configure local secrets (see next section — do NOT edit appsettings.json directly)
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<a-random-string-at-least-32-characters>"
dotnet user-secrets set "ConnectionStrings:Default" "<your-sql-server-connection-string>"
dotnet user-secrets set "Cloudinary:CloudName" "<your-cloud-name>"
dotnet user-secrets set "Cloudinary:ApiKey" "<your-api-key>"
dotnet user-secrets set "Cloudinary:ApiSecret" "<your-api-secret>"
dotnet user-secrets set "AdminSettings:Password" "<your-admin-password>"

# 3. Apply database migrations
dotnet ef database update --project ../WeddingApi.infrastructure --startup-project .

# 4. Run
dotnet run
```

The API will be available at the URL shown in the console, with Swagger UI at `/swagger`. On first run, the app seeds the `Admin`, `Supervisor`, `Client`, and `Provider` roles and creates a default admin account.

## Configuration & Secrets

`appsettings.json` intentionally ships with **empty placeholder values** — no real connection strings, keys, or passwords are committed to this repository. All real values must be supplied locally via [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) (development) or environment variables (production/hosting).

See [`appsettings.Example.json`](WeddingApi.web/appsettings.Example.json) for the full list of required keys.

> ⚠️ Never commit real credentials. If a secret is ever pushed by mistake, rotate it immediately — removing it from a later commit does not remove it from Git history.

## API Overview

| Controller | Base route | Purpose |
|---|---|---|
| `AuthController` | `/api/Auth` | Login, registration (client/provider), profile, supervisor management, provider approval |
| `BookingsController` | `/api/Bookings` | Create/search bookings, calendar view, status updates |
| `ClientsController` | `/api/Clients` | Admin/Supervisor client management |
| `ServiceProviderController` | `/api/ServiceProvider` | Provider directory, CRUD |
| `ServicesController` | `/api/Services` | Priced service packages per provider |
| `PaymentsController` | `/api/Payments` | Record payments, booking payment totals, financial reports |
| `MediaController` | `/api/Media` | Upload/delete provider portfolio images & videos (Cloudinary) |
| `DashboardController` | `/api/Dashboard` | Admin KPIs: bookings, revenue, clients |

Full interactive documentation is available via Swagger UI once the project is running (`/swagger`).

## Roadmap

- [x] Role-based JWT authentication (Admin, Supervisor, Provider, Client)
- [x] Booking + payment flow with auto-confirmation
- [x] Priced service packages per provider
- [ ] Provider availability/scheduling calendar
- [ ] Geo-based service search & recommendations
- [ ] Shared bride/groom to-do list
- [ ] Real-time notifications
- [ ] Payment gateway integration (InstaPay, Vodafone Cash)
- [ ] Review & rating system tied to completed bookings

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes with clear, descriptive messages
4. Open a pull request describing what changed and why

Please avoid committing secrets, generated `bin`/`obj` folders, or publish output — see `.gitignore`.

## License

This project is licensed under the MIT License.
