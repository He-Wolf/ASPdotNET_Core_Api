# Todoapi
ASP.NET Core web api with Entity database and Identity authentication

# Useful .NET Core commands:
## creating the api
dotnet new webapi

dotnet tool install --global dotnet-ef / dotnet tool update --global dotnet-ef

dotnet tool install --global dotnet-aspnet-codegenerator

dotnet add package Microsoft.EntityFrameworkCore.SqlServer

dotnet add package Microsoft.EntityFrameworkCore.SQLite

dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet add package Microsoft.AspNetCore.Identity

dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson

dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

## running the api
sudo dotnet run --urls "http://*:5000;https://*:5001"

## if running and build fails (Linux)
sudo rm -rf obj

sudo rm -rf bin

dotnet build

## code generation

dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers

## database migration
dotnet ef migrations add InitialCreate

dotnet ef database update
