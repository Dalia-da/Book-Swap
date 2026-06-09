# BookSwap

BookSwap is an ASP.NET Core MVC web application for exchanging and lending books between users.

## Main Features
- User registration and login with ASP.NET Identity
- Add, edit, browse, and search books
- Book categories and book details pages
- Lending workflow and lending history
- Comments and ratings
- User-to-user chat/messages
- Admin and seeded roles support

## Tech Stack
- ASP.NET Core MVC (.NET 6)
- Entity Framework Core
- SQL Server
- ASP.NET Identity

## Project Structure
- `Controllers/` application controllers
- `Models/` domain and view models
- `Views/` Razor UI pages
- `ContextDBConfig/` database context and seed data
- `wwwroot/` static files and uploaded images
- `Migrations/` Entity Framework migrations

## Setup
1. Open the solution file `BookSwap1.sln` in Visual Studio.
2. Update `appsettings.json` with your SQL Server connection string and email settings.
3. Restore NuGet packages.
4. Apply migrations / update the database.
5. Run the project.

## Notes
- This repository has been cleaned for GitHub upload.
- Sensitive credentials were removed from configuration files.
- Build folders such as `bin`, `obj`, and `.vs` were excluded.
