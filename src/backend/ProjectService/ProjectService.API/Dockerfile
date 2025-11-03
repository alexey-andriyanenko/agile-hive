FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5203

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjectService.API/ProjectService.API.csproj", "ProjectService.API/"]
COPY ["ProjectService.Application/ProjectService.Application.csproj", "ProjectService.Application/"]
COPY ["ProjectService.Domain/ProjectService.Domain.csproj", "ProjectService.Domain/"]
COPY ["ProjectService.Contracts/ProjectService.Contracts.csproj", "ProjectService.Contracts/"]
COPY ["ProjectService.Infrastructure/ProjectService.Infrastructure.csproj", "ProjectService.Infrastructure/"]
RUN dotnet restore "ProjectService.API/ProjectService.API.csproj"
COPY . .
WORKDIR "/src/ProjectService.API"
RUN dotnet build "./ProjectService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectService.API.dll"]
