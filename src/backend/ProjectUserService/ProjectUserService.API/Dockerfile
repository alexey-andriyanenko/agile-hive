FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5205

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjectUserService.API/ProjectUserService.API.csproj", "ProjectUserService.API/"]
COPY ["ProjectUserService.Application/ProjectUserService.Application.csproj", "ProjectUserService.Application/"]
COPY ["ProjectUserService.Infrastructure/ProjectUserService.Infrastructure.csproj", "ProjectUserService.Infrastructure/"]
COPY ["ProjectUserService.Contracts/ProjectUserService.Contracts.csproj", "ProjectUserService.Contracts/"]
COPY ["ProjectUserService.Domain/ProjectUserService.Domain.csproj", "ProjectUserService.Domain/"]
RUN dotnet restore "ProjectUserService.API/ProjectUserService.API.csproj"
COPY . .
WORKDIR "/src/ProjectUserService.API"
RUN dotnet build "./ProjectUserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectUserService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectUserService.API.dll"]
