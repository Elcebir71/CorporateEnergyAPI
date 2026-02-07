# CorporateEnergyAPI Copilot Instructions

## Build, Test, and Lint

- **Build**: `dotnet build`
- **Run**: `dotnet run`
- **Test**: `dotnet test` (Run specific test: `dotnet test --filter "FullyQualifiedName~Namespace.Class.Method"`)
- **Format**: `dotnet format`
- **API Testing**: Use `CorporateEnergyAPI.http` to test API endpoints.

## Architecture

This project is an ASP.NET Core Web API targeting .NET 10.0. It follows a layered architecture:

- **Controllers** (`/Controllers`): Handle HTTP requests and responses. Use `[ApiController]` and `[Route("[controller]")]`.
- **Services** (`/Services`): Contain business logic. Injected into Controllers.
- **Data** (`/Data`): Handle data access and database context.
- **Models** (`/Models`): Represents the domain entities.
- **DTOs** (`/DTOs`): Data Transfer Objects for API input/output. Isolate domain models from the API contract.

## Key Conventions

- **Dependency Injection**: Register services in `Program.cs` using `builder.Services`. Use constructor injection.
- **Async/Await**: Use asynchronous methods for all I/O operations (database, file, network).
- **Naming**: Use PascalCase for public members and methods. Use camelCase for local variables and parameters.
- **API Contracts**: Always use DTOs for controller actions. Never return Entity Framework models directly.
- **Configuration**: Use `appsettings.json` for configuration and `IOptions<T>` pattern for access.
