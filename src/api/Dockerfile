# Create a stage for restoring dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS deps

# Create a working directory for the source code
WORKDIR /source

# Copy only the project files first to leverage Docker layer caching
COPY *.csproj ./
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore

# Create a stage for building the application.
FROM deps AS build

# Copy the rest of the source code
COPY . .

# Build the application
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -c Release --use-current-runtime --self-contained false -o /app

# Install curl instead of wget
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS curl

RUN apk update && apk add curl && apk del wget

# Final stage
FROM curl AS final

WORKDIR /app

# Copy only the published files from build stage
COPY --from=build /app .

# Add healthcheck
HEALTHCHECK --interval=30s --timeout=3s \
    CMD curl -f http://localhost:${ASPNETCORE_HTTP_PORTS}/health || exit 1

# Expose the port that the application listens on.
EXPOSE ${ASPNETCORE_HTTP_PORTS}

# Switch to non-privileged user
USER $APP_UID

ENTRYPOINT ["dotnet", "api.dll"]
