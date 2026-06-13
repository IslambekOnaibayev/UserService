# ── Stage 1: Build React ─────────────────────────────────────────────────────
FROM node:20-alpine AS frontend
WORKDIR /frontend
COPY WebAPI/ClientApp/package*.json ./
RUN npm ci
COPY WebAPI/ClientApp/ ./
RUN npm run build
# vite outDir '../wwwroot' → /wwwroot

# ── Stage 2: Build .NET ──────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore layer (cached unless .csproj files change)
COPY Directory.Packages.props Directory.Build.props ./
COPY Core/Core.csproj               Core/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY UseCases/UseCases.csproj       UseCases/
COPY WebAPI/WebAPI.csproj           WebAPI/
RUN dotnet restore WebAPI/WebAPI.csproj

# Copy source + built frontend
COPY . .
COPY --from=frontend /wwwroot WebAPI/wwwroot/

RUN dotnet publish WebAPI/WebAPI.csproj \
    -c Release -o /app/publish --no-restore

# ── Stage 3: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "WebAPI.dll"]
