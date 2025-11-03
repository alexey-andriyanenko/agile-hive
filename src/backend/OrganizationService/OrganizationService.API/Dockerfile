FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5202

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrganizationService.API/OrganizationService.API.csproj", "OrganizationService.API/"]
COPY ["OrganizationService.Application/OrganizationService.Application.csproj", "OrganizationService.Application/"]
COPY ["OrganizationService.Infrastructure/OrganizationService.Infrastructure.csproj", "OrganizationService.Infrastructure/"]
COPY ["OrganizationService.Domain/OrganizationService.Domain.csproj", "OrganizationService.Domain/"]
COPY ["OrganizationService.Contracts/OrganizationService.Contracts.csproj", "OrganizationService.Contracts/"]
RUN dotnet restore "OrganizationService.API/OrganizationService.API.csproj"
COPY . .
WORKDIR "/src/OrganizationService.API"
RUN dotnet build "./OrganizationService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrganizationService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrganizationService.API.dll"]
