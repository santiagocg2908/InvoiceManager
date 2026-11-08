- InvoiceManager
  Pardon me for my poor English!!!!:(
  Full Stack Web Application for management and intelligent extraction for invoices by means AI
  
  PROJECT STATE
  
 Currently in development.

  The backend architecture, SQL Server persistence, REST API endpoints, frontend application and AI-powered invoice extraction are implemented. The project is undergoing integration testing and preparation for delivery.
  
  Technologies
  
  Frontend ........................ Angular 22 + TypeScript
  Backend  ........................ C# ASP.NET Core Web API .NET 10 
  ORM ............................. Entity Framework Core 10
  Database ........................ Microsoft SQL Server
  LLM ............................. Gemini-2.5-Flash-Lite
  Version Control ................. Git + GitHub
  
  Architecture
  
  The backend use a DDD architecture
  
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
  
  3.Configure Gemini

  Configure your Gemini API key using the configuration mechanism expected by the backend.

  Important: Do not commit API keys or other sensitive credentials to the repository.

  4.Restore Dependence 
  From the root of the repository
  dotnet restore
  
  5.Compile the solution
  dotnet build
  
  6.Apply migrations
  dotnet ef database update --project ./src/InvoiceManager.Infrastructure/InvoiceManager.Infrastructure.csproj --startup-project ./src/InvoiceManager.Api/InvoiceManager.Api.csproj --context InvoiceManagerDbContext
  
  7.Execute the backend
  dotnet run --project ./src/InvoiceManager.Api/InvoiceManager.Api.csproj

  8.Run the Frontend
  Open another terminal and navigate to the frontend directory:
  cd frontend
  npm install

  Verify that the API base URL configured in the Angular application points to the running backend.

  Start the development server:
  ng serve

  Open the application at:
  http://localhost:4200
  
  Technical decisions
  *Code-First: The data model is defined using C# entities and Entity Framework Core configurations. Migrations keep the SQL Server schema synchronized.
  
  *DDD architecture: Separates responsibilities across business logic, use cases, the API, and infrastructure.
  
  Database
  The migrations from Entity Framework Core are found in 
  src/InvoiceManager.Infrastructure/Persistence/Migrations/
  
  the SQL script from schema initialization is found in 
  database/InvoiceManagerDb.sql

  AI-powered Extraction

  Gemini 3.5 Flash Lite is used to extract structured information from invoice documents.

  This approach automates the initial data-entry process and reduces the need for manual registration.
  
  Angular

  Angular provides a component-based frontend for document uploads, invoice listing, filtering, pagination and invoice detail management.

  Folder "InvoicesForTest"
  The repository also includes sample invoice data for demonstration and testing purposes.

  License
  Developed Project for technical evaluation purposes

  -----------------Español---------------------------
    # InvoiceManager


  Aplicación web Full Stack para la gestión de facturas y la extracción inteligente de información mediante Inteligencia Artificial.

  Estado del proyecto

  **Actualmente en desarrollo.**

  La arquitectura del backend, la persistencia en SQL Server, los endpoints de la API REST, la aplicación frontend y la extracción de facturas mediante Inteligencia Artificial ya están implementados. El proyecto se encuentra en la etapa de pruebas de integración y preparación para su entrega.

  Tecnologías

  Frontend ........................ Angular 22 + TypeScript
  Backend  ........................ C# ASP.NET Core Web API .NET 10 
  ORM ............................. Entity Framework Core 10
  Database ........................ Microsoft SQL Server
  LLM ............................. Gemini-2.5-Flash-Lite
  Version Control ................. Git + GitHub

  Arquitectura

  El backend utiliza una arquitectura basada en los principios de DDD (Domain-Driven Design).

  *Api: Recepción de solicitudes HTTP y configuración de la aplicación.
  *Application:Coordinación de los casos de uso y definición de interfaces.
  *Domain:Entidades y reglas de negocio principales.
  *Infrastructure: Persistencia de datos mediante Entity Framework Core e integración con servicios externos.

  Requisitos previos

  .NET SDK 10.
  SQL Server Express.
  Visual Studio 2026 o una versión compatible con .NET 10.
  Herramientas CLI de Entity Framework Core compatibles con .NET 10.

  Configuración local

  1.Clonar el repositorio

  bash
  git clone https://github.com/santiagocg2908/InvoiceManager.git
  cd InvoiceManager
  

  2.Configurar SQL Server

  Instancia de SQL Server:

  
  .\SQLEXPRESS
  

  En el archivo src/InvoiceManager.Api/appsettings.json, verifica la cadena de conexión:

  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=.\\SQLEXPRESS;Database=InvoiceManagerDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
  }
  

  Modifica la cadena de conexión de acuerdo con la configuración local de SQL Server, si es necesario.

  3.Configurar Gemini

  Configura tu clave de API de Gemini utilizando el mecanismo de configuración esperado por el backend.

  *Importante: No incluyas claves de API ni otras credenciales sensibles en el repositorio.

  4.Restaurar las dependencias

  Desde la raíz del repositorio, ejecuta:

  
  dotnet restore
  

  5.Compilar la solución

  
  dotnet build
  

  #6.Aplicar las migraciones

  Ejecuta el siguiente comando para aplicar las migraciones y actualizar la base de datos:

  
  dotnet ef database update --project ./src/InvoiceManager.Infrastructure/InvoiceManager.Infrastructure.csproj --startup-project ./src/InvoiceManager.Api/InvoiceManager.Api.csproj --context InvoiceManagerDbContext
  

  #7.Ejecutar el backend

  
  dotnet run --project ./src/InvoiceManager.Api/InvoiceManager.Api.csproj
  

  #8.Ejecutar el frontend

  Abre otra terminal y navega hasta el directorio del frontend:

  
  cd frontend
  npm install


  Verifica que la URL base de la API configurada en la aplicación Angular apunte al backend en ejecución.

  Inicia el servidor de desarrollo:

  
  ng serve
  

  Abre la aplicación en el navegador:

  
  http://localhost:4200
  

  #Decisiones técnicas

  *Code-First:El modelo de datos se define mediante entidades de C# y configuraciones de Entity Framework Core. Las migraciones permiten mantener sincronizado el esquema de SQL Server con el modelo de datos de la aplicación.

  *Arquitectura DDD:Separa las responsabilidades relacionadas con la lógica de negocio, los casos de uso, la API y la infraestructura, facilitando el mantenimiento y la organización del código.

  #Base de datos

  Las migraciones de Entity Framework Core se encuentran en:

  
  src/InvoiceManager.Infrastructure/Persistence/Migrations/
  

  El script SQL para la inicialización del esquema de la base de datos se encuentra en:

  
  database/InvoiceManagerDb.sql


  #Extracción mediante Inteligencia Artificial

  Se utiliza Gemini 3.5 Flash Lite para extraer información estructurada de los documentos de facturas.

  Este enfoque permite automatizar el proceso inicial de registro de datos y reducir la necesidad de introducir información manualmente.

  #Angular

  Angular proporciona una interfaz frontend basada en componentes que permite:

  Cargar documentos.
  Consultar el listado de facturas.
  Filtrar facturas.
  Navegar mediante paginación.
  Consultar y gestionar los detalles de las facturas.

  #Carpeta "InvoicesForTest"

  El repositorio también incluye documentos de facturas de ejemplo para realizar demostraciones y pruebas de funcionamiento.

  #Licencia

  Proyecto desarrollado con fines de evaluación técnica.