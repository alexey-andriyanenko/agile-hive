FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5209

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TagService.API/TagService.API.csproj", "TagService.API/"]
COPY ["TagService.Application/TagService.Application.csproj", "TagService.Application/"]
COPY ["TagService.Contracts/TagService.Contracts.csproj", "TagService.Contracts/"]
COPY ["TagService.Infrastructure/TagService.Infrastructure.csproj", "TagService.Infrastructure/"]
COPY ["TagService.Domain/TagService.Domain.csproj", "TagService.Domain/"]
RUN dotnet restore "TagService.API/TagService.API.csproj"
COPY . .
WORKDIR "/src/TagService.API"
RUN dotnet build "./TagService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TagService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TagService.API.dll"]
