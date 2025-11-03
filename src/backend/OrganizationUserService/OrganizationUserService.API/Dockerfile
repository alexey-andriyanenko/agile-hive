FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5204

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrganizationUserService.API/OrganizationUserService.API.csproj", "OrganizationUserService.API/"]
COPY ["OrganizationUserService.Application/OrganizationUserService.Application.csproj", "OrganizationUserService.Application/"]
COPY ["OrganizationUserService.Contracts/OrganizationUserService.Contracts.csproj", "OrganizationUserService.Contracts/"]
COPY ["OrganizationUserService.Domain/OrganizationUserService.Domain.csproj", "OrganizationUserService.Domain/"]
COPY ["OrganizationUserService.Infrastructure/OrganizationUserService.Infrastructure.csproj", "OrganizationUserService.Infrastructure/"]
RUN dotnet restore "OrganizationUserService.API/OrganizationUserService.API.csproj"
COPY . .
WORKDIR "/src/OrganizationUserService.API"
RUN dotnet build "./OrganizationUserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrganizationUserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrganizationUserService.API.dll"]
