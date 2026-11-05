InvoiceManager
Pardon me for my poor English!!!!:(
Full Stack Web Application for management and intelligent extraction for invoices by means AI

PROJECT STATE

Currently in development 

The backend initial structure and persistence with SQL Server are already.
The frontend, Angular, the endpoints and OpenAI integration will be integrarted.

Technologies

Frontend ........................ Angular 22 + TypeScript
Backend  ........................ C# ASP.NET Core Web API .NET 10 
ORM ............................. Entity Framework Core 10
Database ........................ Microsoft SQL Server
LLM ............................. OpenAI API
Version Control ................. Git + GitHub

Architecture

The backend use a layers architecture

*Api: reception of HTTP requests and application configuration.
*Application: coordination of use cases and definition of interfaces
*Domain: Entities and main rules
*Infrastructure: persistence with Entity Framework Core and integration with external services.

Prerequisites

*.NET SDK 10
*SQL Server Express
*Visual Studio 2026 or a compatible version with .NET 10
*Entity Framework Core CLI compatible with .NET 10

Local Configuration 
1.Clone the repository
git clone https://github.com/santiagocg2908/InvoiceManager.git 
cd InvoiceManager

2.Configure SQL Server 
.\SQLEXPRESS

On src/InvoiceManager.Api/appsettings.json, verify the connection string:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=InvoiceManagerDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Modify according of the local configuration of SQL Server if it necessary

3.Restore Dependence 
From the root of the repository

4.Compile the solution
dotnet build

5.Apply migrations
dotnet ef database update --project ./src/InvoiceManager.Infrastructure/InvoiceManager.Infrastructure.csproj --startup-project ./src/InvoiceManager.Api/InvoiceManager.Api.csproj --context InvoiceManagerDbContext

6.Execute the backend
dotnet run --project ./src/InvoiceManager.Api/InvoiceManager.Api.csproj

Technical decisions
*Code-First: The data model is defined using C# entities and Entity Framework Core configurations. Migrations keep the SQL Server schema synchronized.

*Layered architecture: Separates responsibilities across business logic, use cases, the API, and infrastructure.

Database
The migrations from Entity Framework Core are found in 
src/InvoiceManager.Infrastructure/Persistence/Migrations/

the SQL script from schema initialization is found in 
database/InvoiceManagerDb.sql

License
Developed Project for technical evaluation purposes 