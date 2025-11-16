# Етап 1: Збірка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо .sln та .csproj файли
COPY LabWorkOrganization.sln .
COPY server/LabWorkOrganization.API/LabWorkOrganization.API.csproj server/LabWorkOrganization.API/
COPY server/LabWorkOrganization.Application/LabWorkOrganization.Application.csproj server/LabWorkOrganization.Application/
COPY server/LabWorkOrganization.Domain/LabWorkOrganization.Domain.csproj server/LabWorkOrganization.Domain/
COPY server/LabWorkOrganization.Infrastructure/LabWorkOrganization.Infrastructure.csproj server/LabWorkOrganization.Infrastructure/

# Відновлюємо залежності
RUN dotnet restore LabWorkOrganization.sln

# Копіюємо решту коду та публікуємо
COPY . .
WORKDIR "/src/server/LabWorkOrganization.API"
RUN dotnet publish -c Release -o /app/publish

# Етап 2: Виконання
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LabWorkOrganization.API.dll"]