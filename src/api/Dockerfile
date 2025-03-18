# Create a stage for building the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Create a working directory for the source code
WORKDIR /source

# Copy only the project files first to leverage Docker layer caching
COPY *.csproj ./
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -c Release --no-restore --use-current-runtime --self-contained false -o /app

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy only the published files from build stage
COPY --from=build /app .

# Add healthcheck
HEALTHCHECK --interval=30s --timeout=3s \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/health || exit 1

# Switch to non-privileged user
USER $APP_UID

ENTRYPOINT ["dotnet", "api.dll"]
