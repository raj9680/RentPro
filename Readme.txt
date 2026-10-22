# RentPro

A .NET 8 rental management application built on Clean Architecture, with an ASP.NET MVC frontend and a Web API backend.

This README explains the architecture and the reasoning behind it. The short version: six projects, dependencies that only ever point inward, and an MVC frontend that talks to its own API over HTTP instead of calling the backend directly.

---

## Architecture

```mermaid
graph TD
    UI["RentPro.UI<br/>ASP.NET MVC"]
    API["RentPro.Api<br/>Web API"]
    CON["RentPro.Contracts<br/>DTOs"]
    APP["RentPro.Application<br/>Use cases"]
    INF["RentPro.Infrastructure<br/>EF Core, SQL Server"]
    DOM["RentPro.Domain<br/>Entities"]

    UI -->|"HTTPS + JWT (not a reference)"| API
    UI --> CON
    API --> CON
    API --> APP
    API -.->|"DI registration only"| INF
    APP --> CON
    APP --> DOM
    INF --> APP
```

Arrows are compile-time project references unless labelled otherwise.

### Project references

| Project | Type | References |
|---|---|---|
| `RentPro.Domain` | Class library | — |
| `RentPro.Contracts` | Class library | — |
| `RentPro.Application` | Class library | `Domain`, `Contracts` |
| `RentPro.Infrastructure` | Class library | `Application` |
| `RentPro.Api` | ASP.NET Web API | `Application`, `Infrastructure`, `Contracts` |
| `RentPro.UI` | ASP.NET MVC | `Contracts` |

Two of these rows carry most of the design:

- **`RentPro.UI` references exactly one project.** It cannot reach Application, Infrastructure or Domain, so it cannot accidentally bypass the API or touch the database. The compiler enforces this, not a code review.
- **`RentPro.Infrastructure` references `Application`, not the other way around.** Application defines interfaces (`IPropertyRepository`, `IFileStorage`); Infrastructure implements them. This is the dependency inversion that keeps EF Core out of the business logic.

---

## What lives in each layer

### `RentPro.Domain`

Entities, value objects and the rules that are true regardless of how the application is delivered. No EF Core attributes, no DTOs, no NuGet packages beyond the BCL.

```
RentPro.Domain/
├─ Common/            BaseEntity.cs, BaseAuditableEntity.cs
├─ Entities/          Property.cs, Unit.cs, Tenant.cs, Lease.cs, Payment.cs
├─ ValueObjects/      Address.cs, Money.cs, DateRange.cs
├─ Enums/             LeaseStatus.cs, PaymentMethod.cs
└─ Exceptions/        LeaseOverlapException.cs
```

### `RentPro.Contracts`

Every DTO the application uses, in one place, referenced by both the UI and the API. These are the shapes that travel over HTTP.

```
RentPro.Contracts/
├─ Common/            PagedResponse.cs, ApiErrorResponse.cs, ApiRoutes.cs
├─ Properties/        CreatePropertyRequest.cs, PropertyResponse.cs
├─ Leases/            CreateLeaseRequest.cs, LeaseResponse.cs
├─ Tenants/
├─ Payments/
└─ Authentication/    LoginRequest.cs, LoginResponse.cs
```

Rules for this project:

- Plain POCOs and primitives only. No `IFormFile`, no `DbContext` types, no Domain entities.
- Domain enums are mirrored as strings, so renaming an enum member never changes the JSON contract.
- Zero project references and zero NuGet packages. If it needs a dependency, something in the wrong layer has leaked in.
- Validation attributes (`[Required]`, `[StringLength]`) belong here. MVC and the API then validate identically from one definition.

### `RentPro.Application`

Use cases, the interfaces Infrastructure implements, and the mapping between entities and DTOs. Knows nothing about HTTP, controllers or EF Core.

```
RentPro.Application/
├─ Common/
│  ├─ Interfaces/
│  │  ├─ Repositories/   IPropertyRepository.cs, ILeaseRepository.cs
│  │  └─ Services/       IFileStorage.cs, IEmailService.cs, ICurrentUser.cs
│  ├─ Mappings/          LeaseMappings.cs
│  └─ Models/            PaginatedList.cs, Result.cs
├─ Features/
│  ├─ Properties/        Commands/, Queries/
│  ├─ Leases/            Commands/, Queries/
│  └─ Payments/
└─ DependencyInjection.cs
```

### `RentPro.Infrastructure`

The only project in the solution that references EF Core. Everything that talks to the outside world — database, filesystem, email, token generation — lives here.

```
RentPro.Infrastructure/
├─ Persistence/
│  ├─ RentProDbContext.cs
│  ├─ Configurations/     IEntityTypeConfiguration per entity
│  ├─ Repositories/       PropertyRepository.cs, LeaseRepository.cs
│  ├─ Migrations/
│  └─ Seed/               *.json, SeedData.cs
├─ Identity/              JwtTokenGenerator.cs, PasswordHasher.cs
├─ Services/              EmailService.cs, FileStorageService.cs
└─ DependencyInjection.cs
```

### `RentPro.Api`

The backend. Owns authentication, error handling and the public surface of the system.

```
RentPro.Api/
├─ Controllers/        PropertiesController.cs, LeasesController.cs, AuthController.cs
├─ Middlewares/        ExceptionHandlingMiddleware.cs
├─ Mappings/           Contract ↔ Application DTO
├─ Extensions/         SwaggerExtensions.cs, JwtExtensions.cs
├─ appsettings.json    connection string, JWT settings
└─ Program.cs          AddApplication() + AddInfrastructure()
```

### `RentPro.UI`

An HTTP client with Razor views. It holds no business logic — anything that looks like a rule belongs in an Application handler behind the API.

```
RentPro.UI/
├─ Controllers/
├─ Models/             ViewModels — only where the shape differs from a contract
├─ Services/
│  ├─ ApiClients/      IPropertyApiClient.cs, PropertyApiClient.cs
│  └─ TokenHandler.cs  DelegatingHandler that attaches the JWT
├─ Views/
├─ wwwroot/
└─ Program.cs          cookie auth + AddHttpClient
```

---

## How a request flows

Taking **Create Lease**:

1. Razor form posts to `LeasesController` in `RentPro.UI`
2. `ModelState` validates the attributes on `CreateLeaseRequest`
3. `LeaseApiClient` serializes it and sends `POST /api/leases`; `TokenHandler` attaches the JWT
4. `LeasesController` in `RentPro.Api` deserializes the same type and validates it again
5. `ILeaseService` → `LeaseService` applies the business rules
6. `ILeaseRepository` → `LeaseRepository` → EF Core → SQL Server

Because the DTO is shared, step 4 deserializes exactly the type step 3 serialized. No mapping between them, and no drift when one side changes.

### Authentication

The UI posts credentials to `/api/auth/login`. The API verifies the password hash and issues a JWT. The UI stores that token in its own cookie and sends it as a bearer header on every subsequent call.

The API knows only bearer tokens. The UI knows only cookies. Neither needs to understand the other's session model.

---

## Why this architecture

**The frontend cannot cheat.** In a conventional MVC Clean Architecture setup, `UI` references `Application`, and nothing stops a controller from calling a service directly, injecting a `DbContext`, or slipping a LINQ query into a view. Removing that reference makes those shortcuts compile errors instead of review comments.

**A second client costs nothing.** A mobile app, a partner integration, a scheduled job or a Postman collection all consume the same API. In an MVC-only solution each of those means either a second web project or an API bolted on later, when the shapes have already hardened.

**Business logic is testable without a browser.** Integration tests hit HTTP endpoints and assert on JSON. No Razor rendering, no controller plumbing, no `TestServer` juggling two concerns at once.

**One definition of every DTO.** `RentPro.Contracts` is referenced by both ends, so the request the UI sends and the request the API expects are literally the same class. A missing property is a compile error on both sides, not a silent null at runtime.

**The database is reachable from exactly one project.** EF Core, migrations and connection strings live in `Infrastructure` and nowhere else. Swapping the provider, adding a read replica or introducing caching touches one project.

**Deployment and scaling decouple.** The API and the UI are separate processes. They can be deployed independently, scaled independently, and placed on different networks — the API behind a private boundary, the UI public.

---

## Trade-offs

This design is not free, and it is not the right answer for every project.

**Two processes to run and debug.** Local development means starting both. A failing request can be broken in either, and the stack trace stops at the network boundary.

**A serialization boundary.** A property that doesn't map is a null at runtime rather than a compile error. The shared Contracts project removes most of this risk, but not all of it — JSON casing, nullability and enum handling still need attention.

**More projects to navigate.** Six instead of four. For a small application maintained by one person, that overhead is real.

**Versioning is deferred, not solved.** Using one shared DTO set means `/v1` and `/v2` cannot have different shapes of the same concept. That is a deliberate trade for a single-frontend application; if a second consumer arrives, the contracts become the seam where an Application-side DTO set gets introduced.

**When a plain MVC Clean Architecture is the better choice:** one frontend, no second consumer planned, a single developer, and no requirement to deploy the backend separately. The four-project layout gives most of the layering benefit with none of the boundary cost, and the Contracts project is a clean place to split from later.

---

## Conventions

Rules worth enforcing with an architecture test (NetArchTest or ArchUnitNET) rather than trusting to review:

| Rule | Prevents |
|---|---|
| `RentPro.UI` has exactly one `ProjectReference`: `RentPro.Contracts` | the frontend reaching past the API |
| `Microsoft.EntityFrameworkCore` appears in no file outside `RentPro.Infrastructure` | persistence leaking into business logic or the host |
| `RentPro.Contracts` has zero project references and zero NuGet packages | Domain types or ASP.NET types entering the wire format |
| No `RentPro.Domain` type is reachable from `RentPro.UI` or `RentPro.Contracts` | entities being serialized straight to clients |

Two more that a test cannot catch:

- A new property on a Contracts class is a public API change. Decide it, rather than letting it happen because a handler returned it.
- File uploads do not cross the boundary as `IFormFile`. The ViewModel holds `IFormFile`, the contract holds filename and content type, the file travels as a `multipart/form-data` part, and Application receives a `Stream`.

---

## Stack

- .NET 8
- ASP.NET Core MVC and Web API
- Entity Framework Core 8, SQL Server
- JWT bearer authentication (API), cookie authentication (UI)
- Swagger / OpenAPI
