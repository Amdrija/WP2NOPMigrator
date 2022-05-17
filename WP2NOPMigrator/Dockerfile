FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WP2NOPMigrator/WP2NOPMigrator.csproj", "WP2NOPMigrator/"]
RUN dotnet restore "WP2NOPMigrator/WP2NOPMigrator.csproj"
COPY . .
WORKDIR "/src/WP2NOPMigrator"
RUN dotnet build "WP2NOPMigrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WP2NOPMigrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WP2NOPMigrator.dll"]
