# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY iERP.sln ./
COPY src/ ./src/

RUN dotnet restore src/iERP.Api/iERP.Api.csproj
RUN dotnet publish src/iERP.Api/iERP.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Railway injects PORT; fall back to 8080 for local docker runs
CMD ["sh", "-c", "dotnet iERP.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
