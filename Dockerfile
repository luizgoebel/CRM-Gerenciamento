FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/CRM-API.sln ./
COPY src/CRM.API/CRM.API.csproj CRM.API/
COPY src/CRM.Application/CRM.Application.csproj CRM.Application/
COPY src/CRM.Core/CRM.Core.csproj CRM.Core/
COPY src/CRM.Domain/CRM.Domain.csproj CRM.Domain/
COPY src/CRM.Infrastructure/CRM.Infrastructure.csproj CRM.Infrastructure/
COPY src/CRM.Tests/CRM.Tests.csproj CRM.Tests/
RUN dotnet restore "CRM-API.sln"
COPY src/ .
WORKDIR /src/CRM.API
RUN dotnet publish CRM.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CRM.API.dll"]
