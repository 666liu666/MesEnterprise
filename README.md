# MesEnterprise Modern MES API

This repository provides a clean-room implementation of a modern, enterprise-grade Manufacturing Execution System (MES) backend built with .NET 9, Oracle Database, Redis, Quartz, SignalR and an opinionated modular architecture.

## Solution layout

```
MesEnterprise.sln
├── src/
│   ├── MesEnterprise.Api               # API host, controllers, middleware, DI bootstrapping
│   ├── MesEnterprise.Application       # CQRS layer, validators, MediatR handlers, DTO mapping
│   ├── MesEnterprise.Domain            # Domain entities for identity, manufacturing, quality, inventory & data center
│   ├── MesEnterprise.Infrastructure    # EF Core persistence, Oracle integrations, caching, jobs, security, plugins
│   ├── MesEnterprise.Plugins.Abstractions # Contracts for dynamically loaded plugins
│   └── MesEnterprise.Shared            # Cross-cutting constants, DTO primitives and DI helpers
└── Directory.Build.props               # Common build configuration (net9.0, nullable enabled)
```

## Key platform features

* **Authentication & Authorization** – JWT Bearer auth, role/permission claims, multi-tenant awareness and pluggable token issuance.
* **Auditing** – Structured request/exception logging written to the Oracle database with middleware hooks.
* **Caching** – Hybrid in-memory + Redis distributed cache with extensibility for module-specific caching.
* **Task scheduling** – Quartz.NET nightly job example for BOM/process synchronization.
* **Realtime** – SignalR hub ready for production notifications with Redis backplane support.
* **Security** – Anti-forgery protection, strict response headers, centralized exception handling, correlation IDs and validation pipeline.
* **Monitoring** – Prometheus metrics endpoint, health checks, OpenTelemetry tracing/metrics exporters.
* **Configuration management** – Hierarchical `appsettings` with reload support, plugin folder path control and scheduler tweaks.
* **Documentation** – Swagger/OpenAPI with API versioning, security schemes and auto-generated docs.
* **Database** – EF Core 9 (preview) with Oracle provider, schema configurations, seed data and per-tenant DbContext factory.
* **Multi-tenancy** – Tenant catalog context, dynamic connection strings, global query filters and runtime tenant switching API.
* **Plugin framework** – `AssemblyLoadContext`-based loader for hot-loading business modules without redeploying the core.

## Getting started

1. **Install prerequisites**: .NET 9 SDK preview, Oracle client libraries, Redis, and Oracle EF Core provider.
2. **Update secrets**: Replace placeholder credentials inside `appsettings.json` and configure the referenced Oracle `TNS` alias (`VN_YNMESD_DEV`).
3. **Apply database migrations**: Use `dotnet ef database update` targeting the `MesEnterprise.Infrastructure` project (requires Oracle connectivity).
4. **Run the API**: `dotnet run --project src/MesEnterprise.Api` – Swagger UI is hosted at `/swagger`. SignalR hub is exposed at `/hubs/mes`.
5. **Schedule jobs**: Quartz is configured via cron expression inside the `Scheduler:NightlySyncCron` configuration section.
6. **Observability**: Scrape metrics from `/metrics`, query health probes at `/health/live` & `/health/ready`, and export traces via OTLP.

## Development guidance

* Controllers are organized by manufacturing domains. The **Data Center** area contains dedicated sub-folders for process, model, vendor, routing, etc.
* New use cases should be implemented as MediatR commands/queries inside the `Application` project and exposed via controllers in the API project.
* To extend master data management or quality modules, define new domain entities and configuration classes inside the `Domain` and `Infrastructure` layers respectively.
* The `MesEnterprise.Plugins.Abstractions` project can be referenced by external assemblies to dynamically register additional business services.
* Use the `HybridCacheService` to coordinate caching across Redis and in-memory caches, and rely on the `ICacheService` abstraction for feature-specific caching strategies.

## Operational notes

* Health checks tag ready components for Kubernetes style probes, while Prometheus metrics include HTTP, runtime and EF Core collectors.
* The JWT signing key and Oracle credentials should be stored in a secure secret provider (Azure Key Vault, HashiCorp Vault, etc.).
* `SeedData.InitializeAsync` migrates the database and provisions a default administrator account (`admin / Mes@123456`). Change the password immediately in production environments.
* Tenant switching is exposed via `POST /api/v1/tenants/switch/{tenantId}` and updates the current tenant context per request scope.

> ⚠️ The repository targets .NET 9 preview packages. Restore & build operations require the matching .NET SDK preview release.
