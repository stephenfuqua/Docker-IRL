
ARG NODE_VERSION=20.18.1

# Use node image for base image for all stages.
FROM node:${NODE_VERSION}-alpine AS base

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
FROM nginx:stable-alpine3.20 AS final

# Copy the built application to the NGiNX image.
COPY --from=build /usr/src/app/dist /usr/src/app/dist

# Copy the NGiNX configuration file.
COPY default.conf.template /etc/nginx/templates/

# Expose the port that the application listens on.
EXPOSE 80

# No ENTRYPOINT or CMD required - NGiNX will start automatically.
