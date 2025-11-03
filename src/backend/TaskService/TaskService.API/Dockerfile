FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5207

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TaskService.API/TaskService.API.csproj", "TaskService.API/"]
COPY ["TaskService.Application/TaskService.Application.csproj", "TaskService.Application/"]
COPY ["TaskService.Contracts/TaskService.Contracts.csproj", "TaskService.Contracts/"]
COPY ["TaskService.Infrastructure/TaskService.Infrastructure.csproj", "TaskService.Infrastructure/"]
COPY ["TaskService.Domain/TaskService.Domain.csproj", "TaskService.Domain/"]
RUN dotnet restore "TaskService.API/TaskService.API.csproj"
COPY . .
WORKDIR "/src/TaskService.API"
RUN dotnet build "./TaskService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskService.API.dll"]
