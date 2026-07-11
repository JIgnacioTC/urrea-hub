# ─── Build stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Restore dependencies (cached layer)
COPY UrreaHub.sln .
COPY UrreaHub.Api/UrreaHub.Api.csproj             UrreaHub.Api/
COPY UrreaHub.Application/UrreaHub.Application.csproj UrreaHub.Application/
COPY UrreaHub.Domain/UrreaHub.Domain.csproj       UrreaHub.Domain/
COPY UrreaHub.Infrastructure/UrreaHub.Infrastructure.csproj UrreaHub.Infrastructure/
RUN dotnet restore

# Build & publish
COPY . .
RUN dotnet publish UrreaHub.Api/UrreaHub.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─── Runtime stage ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

COPY --from=build /app/publish .

# Data folder for CSV seeding (read-only in prod is fine)
COPY UrreaHub.Api/Data ./Data

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

ENTRYPOINT ["dotnet", "UrreaHub.Api.dll"]
