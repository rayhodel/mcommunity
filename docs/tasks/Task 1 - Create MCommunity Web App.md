# Task 1 - Create MCommunity Web App

Create a .NET Core MVC web app for MCommunity lookup.

## Status: Completed
## Dependencies: None
## Steps to Complete:
[x] 1. Initialize .NET 10 MVC Project
   [x] 1.1. Run `dotnet new mvc -n MCommunityWeb -f net10.0`
   [x] 1.2. Add necessary NuGet packages (System.Net.Http.Json)
[x] 2. Implement Backend Service
   [x] 2.1. Create `MCommunityService` class
   [x] 2.2. Implement `GetTokenAsync`
   [x] 2.3. Implement `GetPersonAsync` and `GetGroupAsync`
[x] 3. Implement Controller
   [x] 3.1. Create `HomeController` actions
   [x] 3.2. Create `Search` endpoint to proxy requests
[x] 4. Implement Frontend
   [x] 4.1. Create `Index.cshtml` with responsive layout
   [x] 4.2. Add "Settings" modal for API credentials
   [x] 4.3. Implement JavaScript for LocalStorage and AJAX calls
[x] 5. Styling
   [x] 5.1. Add CSS for professional look

## Completion Notes:
Task 1 completed on December 10, 2025.
- Initialized .NET 10 MVC project.
- Implemented `MCommunityService` for API interaction.
- Created `HomeController` with `Search` action.
- Built responsive frontend with Bootstrap and LocalStorage for credentials.
- Added custom styling.
