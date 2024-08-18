FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BATTARI-api.csproj", "./"]
RUN dotnet restore "./BATTARI-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BATTARI-api.csproj" -o /app/build -c Debug

FROM build AS publish
RUN dotnet publish "BATTARI-api.csproj" -o /app/publish -c Debug

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["user.db", "/app/"]
ENTRYPOINT ["dotnet", "BATTARI-api.dll"]

