# TODOtnet - REST API
## Table of Content
1. [Introduction](#introduction)
2. [Used SDK version](#used-sdk-version)
3. [Used tools](#used-tools)
4. [Used packages](#used-packages)
5. [Documentation and Test (Swagger)](#documentation-and-test-swagger)
6. [How to run/test the api](#how-to-run-test-the-api)
	6.1. [Docker](#docker)
	6.2. [SDK](#sdk)
7. [Limitations](#limitations)
    7.1. [Exception/error handling](#exception-error-handling)
8. [Some Further development possibilities](#some-further-development-possibilities)
	8.1. [Unit and Integration Tests](#unit-and-integration-tests)
	8.2. [Facebook sign-in](#facebook-sign-in)
	8.3. [Adding roles](#adding-roles)
9. [Appendix](#appendix)

## 1. Introduction <a name="introduction"></a>
ASP.NET Core web api with Entity Core ORM & SQLite database and Identity Core/JWT authentication
## 2. Used SDK version <a name="used-sdk-version"></a>
## 3. Used tools <a name="used-tools"></a>
## 4. Used packages <a name="used-packages"></a>
## 5. Documentation and Test (Swagger) <a name="documentation-and-test-swagger"></a>
## 6. How to run/test the api <a name="how-to-run-test-the-api"></a>
### 6.1. Docker <a name="docker"></a>
### 6.2. SDK <a name="sdk"></a>
## 7. Limitations <a name="limitations"></a>
### 7.1. Exception/error handling <a name="exception-error-handling"></a>
## 8. Some Further development possibilities <a name="some-further-development-possibilities"></a>
### 8.1. Unit and Integration Tests <a name="unit-and-integration-tests"></a>
### 8.2. Facebook sign-in <a name="facebook-sign-in"></a>
### 8.3. Adding roles <a name="adding-roles"></a>
## 9. Appendix
### Useful .NET Core commands:
#### to create an api
dotnet new webapi

#### to install tools
dotnet tool install --global dotnet-ef\
dotnet tool install --global dotnet-aspnet-codegenerator

#### to update tools
dotnet tool update --global dotnet-ef

#### to install packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer\
dotnet add package Microsoft.EntityFrameworkCore.SQLite\
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design\
dotnet add package Microsoft.EntityFrameworkCore.Design\
dotnet add package Microsoft.AspNetCore.Identity\
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson\
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection\
dotnet add package Swashbuckle.AspNetCore\
dotnet add package Swashbuckle.AspNetCore.Newtonsoft

#### to run the api
sudo dotnet run

#### to build the api
dotnet build

#### code generation
dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers

#### database migration
dotnet ef migrations add InitialCreate\
dotnet ef database update
