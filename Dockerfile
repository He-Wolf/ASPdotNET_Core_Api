#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["web_api.csproj", "./"]
RUN dotnet restore "./web_api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "web_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "web_api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web_api.dll"]

#docker build -t todowebapi:v1 .
#docker run -it --rm -p 5000:5000 todowebapi:v1

#If you use Windows Docker, dotnet restore might not work because of network issues.
#Docker for Windows uses the network adapter with the lowest Interface metric value.
#When this adapter is disconnected, your container cannot connect to the internet.
#To see if this is your problem as well:
#Get-NetIPInterface -AddressFamily IPv4 | Sort-Object -Property InterfaceMetric -Descending
#If yes, set your connected active connection to the lowest value:
#Set-NetIPInterface -InterfaceAlias 'Wi-Fi' -InterfaceMetric 20