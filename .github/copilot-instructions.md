# GitHub Copilot Instructions for Agile Hive Project

## Project Overview
- This is a monorepo containing backend and frontend.
- Backend includes multiple microservices implemented with ASP.NET Core and gRPC.
- IdentityService handles authentication and authorization using ASP.NET Identity and JWT.
- API Gateway exposes REST endpoints and consumes backend microservices via gRPC clients.
- Frontend is a React SPA consuming API Gateway endpoints.
- Database access uses Entity Framework Core with PostgreSQL.
- Microservices follow clean architecture: Domain, Application, Infrastructure, API projects.
- Use FluentValidation for request validation.
- Token-based authentication with refresh tokens.
- The project emphasizes asynchronous programming, dependency injection, and SOLID principles.

## Coding Style
- Use C# 12 features when applicable, but prioritize readability and maintainability.
- Favor async/await for I/O-bound operations.
- Follow .NET naming conventions.
- Use dependency injection via constructor injection.
- Use gRPC for inter-service communication; REST only for external-facing API Gateway.
- Use FluentValidation for input validation.
- Write concise and meaningful comments.
- Write unit tests and integration tests when possible (xUnit preferred).

## Common Tasks & Patterns
- How to define gRPC service methods and proto files.
- How to implement Identity with custom User and Role entities.
- How to generate and validate JWT and refresh tokens.
- How to configure EF Core DbContext with separate projects for domain entities and configurations.
- How to write service extensions to register services in DI container.
- How to map gRPC exceptions to REST HTTP responses.
- How to handle database migrations and seed initial data.
- How to consume gRPC clients from API Gateway project.
- How to organize microservices into separate projects and solutions inside the monorepo.

## What NOT to suggest
- Suggestions involving frameworks outside ASP.NET Core, React, EF Core, or gRPC.
- Outdated or deprecated ASP.NET Core versions.
- Complex solutions ignoring clean architecture principles.
- Synchronous blocking code for database or network calls.
- Overly verbose or unnecessary boilerplate code.
- Non-standard or insecure authentication implementations.

## Example prompt you might encounter:
- "How do I create a gRPC service that handles user registration with JWT token generation?"
- "Show me a FluentValidation rule for validating a password with at least one uppercase, lowercase, digit, and special character."
- "Help me implement refresh token rotation with EF Core and ASP.NET Identity."
- "How to configure dependency injection for microservices following clean architecture?"
- "How do I map a gRPC status code Unauthorized to an HTTP 401 response in API Gateway?"

---

Keep suggestions focused, idiomatic, secure, and aligned with modern ASP.NET Core microservices best practices.
