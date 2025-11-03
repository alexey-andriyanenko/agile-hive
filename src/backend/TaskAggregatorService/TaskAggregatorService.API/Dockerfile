FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5208

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TaskAggregatorService.API/TaskAggregatorService.API.csproj", "TaskAggregatorService.API/"]
COPY ["TaskAggregatorService.Application/TaskAggregatorService.Application.csproj", "TaskAggregatorService.Application/"]
COPY ["TaskAggregatorService.Infrastructure/TaskAggregatorService.Infrastructure.csproj", "TaskAggregatorService.Infrastructure/"]
COPY ["TaskAggregatorService.Contracts/TaskAggregatorService.Contracts.csproj", "TaskAggregatorService.Contracts/"]
COPY ["TaskAggregatorService.Domain/TaskAggregatorService.Domain.csproj", "TaskAggregatorService.Domain/"]
RUN dotnet restore "TaskAggregatorService.API/TaskAggregatorService.API.csproj"
COPY . .
WORKDIR "/src/TaskAggregatorService.API"
RUN dotnet build "./TaskAggregatorService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskAggregatorService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskAggregatorService.API.dll"]
