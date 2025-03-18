# Create a stage for restoring dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.20@sha256:a39f537eeeff2be72a27ee8d72ebde1cd2d4a76f3a59250272875760991c4aa5 AS deps

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

# Install additional utilities: globalization (required by Microsoft.Data.SqlClient), and replace `wget` with `curl`
FROM mcr.microsoft.com/dotnet/aspnet:v8.0-alpine3.20@sha256:4c514ff3c9764b79e0ee1f031e1ac6ec2a0253a66960cddbcd314cb26becf839 AS curl
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk del --no-cache wget && apk add --no-cache curl icu-data-full icu-libs

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
