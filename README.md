# TODOtNET - REST API
## Table of Content
1. [Introduction](#introduction)
2. [Used SDK version](#used-sdk-version)
3. [Used tools](#used-tools)
4. [Used packages](#used-packages)
5. [How to run the API](#how-to-run-the-api)\
	5.1. [Docker](#docker)\
	5.2. [SDK](#sdk)
6. [Test and Documentation (Swagger)](#test-and-documentation-swagger)
7. [Limitations](#limitations)\
    7.1. [Exception/error handling](#exception-error-handling)
8. [Some further development possibilities](#some-further-development-possibilities)
9. [Resources](#resources)
10. [Appendix](#appendix)

## 1. Introduction <a name="introduction"></a>
This is a basic ASP.NET Core RESTful web API with CRUD operations and user login/account management. You can register a new user account with your email address, name and password. After successful registration, you can log in and add, edit, remove and track TODO items. You can also edit your account data and delete your account. This app was created for learning purpose, but is might be useful as a starting-point for other projects.

The API uses:
- JWT for authentication,
- Entity Core as ORM
- Identity Core for identity management
- SQLite for DB management
- Swashbuckle as Swagger

Tooling:
- Postman for testing
- Git Extensions as git gui
- VSC as text editor
- Docker for containerization
- Windows 10 as OS

If any question, please do not hesitate to contact me.

## 2. Used SDK version <a name="used-sdk-version"></a>
.NET Core SDK v3.1.201
## 3. Used tools <a name="used-tools"></a>
- dotnet-ef
- dotnet-aspnet-codegenerator
## 4. Used packages <a name="used-packages"></a>
- Microsoft.AspNetCore.Authentication.JwtBearer v3.1.3
- Microsoft.AspNetCore.Identity v2.2.0
- Microsoft.AspNetCore.Identity.EntityFrameworkCore v3.1.3
- Microsoft.AspNetCore.Mvc.NewtonsoftJson v3.1.3
- Microsoft.EntityFrameworkCore.Design v3.1.2
- Microsoft.EntityFrameworkCore.SQLite v3.1.2
- Microsoft.EntityFrameworkCore.SqlServer v3.1.2
- Microsoft.IdentityModel.Tokens v6.5.0
- Microsoft.VisualStudio.Web.CodeGeneration.Design v3.1.1
- Swashbuckle.AspNetCore v5.3.3
- Swashbuckle.AspNetCore.Newtonsoft v5.3.3
- AutoMapper.Extensions.Microsoft.DependencyInjection v7.0.0
## 5. How to run the API <a name="how-to-run-the-api"></a>
### 5.1. Docker <a name="docker"></a>
### 5.2. SDK <a name="sdk"></a>
- download and install .NET Core SDK version v3.1.201 or greater (latest 3.1)
- clone or download the content of the repository
- open a terminal and navigate to the containing folder
- write "dotnet restore" and press Enter
- write "dotnet run" and press Enter
- if no error message in the terminal, open your browser (recommended: latest Chrome, Firefox, Safari, Edge Chromium or Chromium) and open: http://localhost:5000/swagger
- first register a user account, then log in and after that you can manage your TODO items and account
## 6. Test and Documentation (Swagger) <a name="test-and-documentation-swagger"></a>
The documentation of the API was created with OpenAPI/Swagger (Swashbuckle). When you run the app, you can navigate to http://localhost:5000/swagger. On this URL you can read the documentation, and you can also test the API. (You do not need additional tools such as Curl or Postman.)

The app uses JWT authentication. It means that when you registered an account and got the token, you need to click to the "Authorize" button on the top right corner and insert "Bearer \<yourtoken\>".
(e.g. Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJmMTUxNWEyZS04MjhlLTQ4MTktYmJkYy1kYTc0NDU0MDFjMzAiLCJqdGkiOiI4MzFiY2ZmZC0xNWMxLTQ5YzEtYWJiMy03NjYyNjU2YzMxYmYiLCJleHAiOjE1ODg2MzExMjQsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.qZCnKIzVth7hS6RrDxNXP3w12h-LeZptdV72eJYxsBw)

!Do not forget to put "Bearer" before your token!

Only after authorization can you manage you TODO items and account.
## 7. Limitations <a name="limitations"></a>
### 7.1. Exception/error handling <a name="exception-error-handling"></a>
This application needs to be extended with exception handling and more response values. There are some already known issues which may cause error when it is not used correctly. I only tested the app with correct input values.
## 8. Some further development possibilities <a name="some-further-development-possibilities"></a>
- token refreshing
- Facebook sign-in
- adding roles (admin, user)
- OpenID Connect & IdentityServer4
- SPA frontend (Blazor webassembly)
- automated unit and integration tests
## 9. Resources <a name="resources"></a>
There are several online source which I used to create this web app.\
Including but not limited to:
- Microsoft:
	- https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1
	- https://docs.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-3.1
	- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-3.1
	- https://docs.microsoft.com/en-us/ef/core/
	- https://docs.microsoft.com/en-us/dotnet/csharp/
- Tutorialspoint:
	- https://www.tutorialspoint.com/csharp/
	- https://www.tutorialspoint.com/asp.net_core/
	- https://www.tutorialspoint.com/asp.net_mvc/index.htm
	- https://www.tutorialspoint.com/entity_framework/index.htm
	- https://www.tutorialspoint.com/linq/index.htm
- TutorialsTeacher:
	- https://www.tutorialsteacher.com/core
	- https://www.tutorialsteacher.com/webapi/web-api-tutorials
	- https://www.tutorialsteacher.com/mvc/asp.net-mvc-tutorials
	- https://www.tutorialsteacher.com/csharp/csharp-tutorials
	- https://www.tutorialsteacher.com/linq/linq-tutorials
	- https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx
- JWT:
	- https://medium.com/@ozgurgul/asp-net-core-2-0-webapi-jwt-authentication-with-identity-mysql-3698eeba6ff8
	- https://dotnetdetail.net/asp-net-core-3-0-web-api-token-based-authentication-example-using-jwt-in-vs2019/
	- https://code-maze.com/authentication-aspnetcore-jwt-1/
	- https://fullstackmark.com/post/19/jwt-authentication-flow-with-refresh-tokens-in-aspnet-core-web-api
	- https://www.c-sharpcorner.com/article/jwt-json-web-token-authentication-in-asp-net-core/
	- https://www.c-sharpcorner.com/article/asp-net-core-2-0-bearer-authentication/
	- https://www.blinkingcaret.com/2017/09/06/secure-web-api-in-asp-net-core/
	- https://salslab.com/a/jwt-authentication-and-authorisation-in-asp-net-core-web-api
	- https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api

Thank to every hero on Stackoverflow and Github who helped me with their comments! (Not all heroes wear capes.)

## 10. Appendix <a name="appendix"></a>
### Useful .NET Core cli commands:
**to create a web API:**\
dotnet new webapi

**to install tools:**\
dotnet tool install --global \<toolname\>\
e.g. dotnet tool install --global dotnet-ef

**to update tools:**\
dotnet tool update --global \<toolname\>\
e.g. dotnet tool update --global dotnet-ef

**to install packages:**\
dotnet add package \<packagename\>\
e.g. dotnet add package Swashbuckle.AspNetCore.Newtonsoft

**to run the API:**\
dotnet run

**to build the API:**\
dotnet build

**code generation:**\
e.g. dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers

**database migration:**\
dotnet ef migrations add InitialCreate\
dotnet ef database update
