# ──────────────────────────────────────────────
#  Stage 1: Build
# ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as a distinct layer (better layer caching)
COPY C4Justice.Web/C4Justice.Web.csproj C4Justice.Web/
RUN dotnet restore C4Justice.Web/C4Justice.Web.csproj

# Copy everything else and publish
COPY C4Justice.Web/ C4Justice.Web/
WORKDIR /src/C4Justice.Web
RUN dotnet publish C4Justice.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ──────────────────────────────────────────────
#  Stage 2: Runtime
# ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Render injects the PORT env variable dynamically.
# ASP.NET Core reads ASPNETCORE_URLS, so we set it at container start.
# The connection string is provided via the environment variable
# ConnectionStrings__DefaultConnection on the Render dashboard.
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose a default port (Render overrides this with $PORT at runtime)
EXPOSE 10000

# Start the app bound to the Render-assigned $PORT
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet C4Justice.Web.dll"]
