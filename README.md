# Currency Exchange DKK API

This project is a .NET 8 Web API that provides currency conversion functionality with JWT-based authentication and role-based access control (RBAC).
The base currency is DKK (Danish Krone), and exchange rates are fetched and stored from external sources.

🚀 Features

Currency conversion to DKK.

Fetch exchange rates and store them in the database.

JWT Authentication with Admin and User roles.

Role-based API access restrictions.

Repository + Unit of Work design pattern.

Logging with log4net.

Unit tests with xUnit & FluentAssertions.

Swagger UI for API documentation.

📂 Repository

GitHub Repo: currency-exchange-dkk

⚙️ Prerequisites

.NET 8 SDK

SQL Server LocalDB
 (default setup)

Visual Studio / VS Code

🔑 Database Setup

The project uses Entity Framework Core with migrations.

Connection string (in appsettings.json):

"ConnectionStrings": {
  "Default": "Server=(localdb)\\mssqllocaldb;Database=CurrencyDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}


Run the following commands to create/update the database:

dotnet ef database update --project CurrencyConversion.DataAccessLayer --startup-project CurrencyConversion.API

🔐 Authentication & Roles

This project implements JWT authentication.

User Roles:

Admin

Can access all APIs available to a normal user.

Additionally can access:

GET /api/rates → Get all exchange rates.

GET /api/convert/history → View conversion history.

User

Can register, log in, and use currency conversion endpoints.

👤 Register & Login
Register

POST /api/auth/register

Example request body for Admin:

{
  "username": "admin1",
  "password": "AdminPass123",
  "role": "Admin"
}


Example request body for User:

{
  "username": "user1",
  "password": "UserPass123",
  "role": "User"
}

Login

POST /api/auth/login

Example request body:

{
  "username": "admin1",
  "password": "AdminPass123"
}


This returns a JWT token which must be included in Swagger or Postman:

Authorization: Bearer <token>

🧪 Testing with Swagger

Swagger URL: https://localhost:7000/swagger

Register a user (Admin or User).

Log in to get a JWT token.

Click Authorize button in Swagger and paste:

Bearer <your-token>


Test APIs based on your role:

User → Can perform conversions.

Admin → Can perform conversions + access GetAllRates & ConversionHistory.

🧩 Tech Stack

ASP.NET Core 8

Entity Framework Core

SQL Server LocalDB

JWT Authentication

log4net (logging)

xUnit + FluentAssertions (testing)

🧪 Running Tests

Run all tests:

dotnet test
