
# Use node image for base image for all stages.
FROM node:20.19.0-alpine3.21@sha256:8bda036ddd59ea51a23bc1a1035d3b5c614e72c01366d989f4120e8adca196d4 AS base

# Set working directory for all build stages.
WORKDIR /usr/src/app

# Create a stage for installing production dependecies.
FROM base AS deps

# Download dependencies as a separate step to take advantage of Docker's caching.
# Leverage a cache mount to /root/.npm to speed up subsequent builds.
# Leverage bind mounts to package.json and package-lock.json to avoid having to copy them
# into this layer.
RUN --mount=type=bind,source=package.json,target=package.json \
    --mount=type=bind,source=package-lock.json,target=package-lock.json \
    --mount=type=cache,target=/root/.npm \
    npm ci


# Create a stage for building the application.
FROM deps AS build

# Copy the rest of the source files into the image.
COPY . .
# Run the build script.
RUN npm run build

# Switch to NGiNX for static web hosting.
FROM nginx:stable-alpine3.20@sha256:d2c11a1e63f200585d8225996fd666436277a54e8c0ba728fa9afff28f075bd7 AS final

# Copy the built application to the NGiNX image.
COPY --from=build /usr/src/app/dist /usr/src/app/dist

# Copy the NGiNX configuration file.
COPY default.conf.template /etc/nginx/templates/

# Add healthcheck
HEALTHCHECK --interval=30s --timeout=3s \
    CMD curl -f http://localhost/health || exit 1

# Expose the port that the application listens on.
EXPOSE 80

# No ENTRYPOINT or CMD required - NGiNX will start automatically.
