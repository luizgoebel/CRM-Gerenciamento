FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CRM.API/CRM.API.csproj", "src/CRM.API/"]
RUN dotnet restore "src/CRM.API/CRM.API.csproj"
COPY . .

WORKDIR /src/src/CRM.API
RUN dotnet build "CRM.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.API.dll"]
