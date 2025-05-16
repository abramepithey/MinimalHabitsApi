# HabitTrackerAPI

A clean, scalable .NET 9 Web API for tracking user habits, featuring full CRUD functionality, user authentication, and proper architectural separation (Controllers, Services, Models).

## Features

- .NET 9 Web API using minimal API patterns
- Entity Framework Core with SQL Server
- Full CRUD for habits
- User authentication and management (register/login)
- Separation of concerns: clean folder structure for maintainability
- Thoughtful error handling and validation

## Tech Stack

- .NET 9
- Entity Framework Core
- SQL Server (local or Azure-hosted)
- ASP.NET Identity (or custom auth, if applicable)

## Getting Started

1. Clone the repo  
2. Update your `appsettings.json` with your connection string  
3. Run EF migrations: `dotnet ef database update`  
4. Run the API: `dotnet run`

## API Docs

Endpoints are organized around REST conventions. A Swagger UI is available at `/swagger`.

## Status

Actively maintained and paired with a Blazor WebAssembly frontend: [HabitTrackerBlazor](https://github.com/abramepithey/MinimalHabitsBlazor)
