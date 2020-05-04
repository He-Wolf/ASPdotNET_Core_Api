# Todoapi
ASP.NET Core web api with Entity Core ORM & SQLite database and Identity Core/JWT authentication

# Useful .NET Core commands:
## to create an api
dotnet new webapi

## to install tools
dotnet tool install --global dotnet-ef / dotnet tool update --global dotnet-ef\
dotnet tool install --global dotnet-aspnet-codegenerator

## to install packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer\
dotnet add package Microsoft.EntityFrameworkCore.SQLite\
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design\
dotnet add package Microsoft.EntityFrameworkCore.Design\
dotnet add package Microsoft.AspNetCore.Identity\
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson\
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection\
dotnet add package Swashbuckle.AspNetCore\
dotnet add package Swashbuckle.AspNetCore.Newtonsoft

## to run the api
sudo dotnet run

## to build the api
dotnet build

## code generation
dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers

## database migration
dotnet ef migrations add InitialCreate\
dotnet ef database update
