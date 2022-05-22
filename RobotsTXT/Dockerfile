FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RobotsTXT/RobotsTXT.csproj", "RobotsTXT/"]
RUN dotnet restore "RobotsTXT/RobotsTXT.csproj"
COPY . .
WORKDIR "/src/RobotsTXT"
RUN dotnet build "RobotsTXT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RobotsTXT.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RobotsTXT.dll"]
