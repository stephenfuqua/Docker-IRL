# syntax=docker/dockerfile:1

# Comments are provided throughout this file to help you get started.
# If you need more help, visit the Dockerfile reference guide at
# https://docs.docker.com/go/dockerfile-reference/

# Want to help us make this template better? Share your feedback here: https://forms.gle/ybq9Krt8jtBL3iCk7

ARG NODE_VERSION=20.18.1

################################################################################
# Use node image for base image for all stages.
FROM node:${NODE_VERSION}-alpine as base

# Set working directory for all build stages.
WORKDIR /usr/src/app


################################################################################
# Create a stage for installing production dependecies.
FROM base as deps

# Download dependencies as a separate step to take advantage of Docker's caching.
# Leverage a cache mount to /root/.npm to speed up subsequent builds.
# Leverage bind mounts to package.json and package-lock.json to avoid having to copy them
# into this layer.
RUN --mount=type=bind,source=package.json,target=package.json \
    --mount=type=bind,source=package-lock.json,target=package-lock.json \
    --mount=type=cache,target=/root/.npm \
    npm ci

################################################################################
# Create a stage for building the application.
FROM deps as build

# Copy the rest of the source files into the image.
COPY . .
# Run the build script.
RUN npm run build

# Switch to NGiNX for static web hosting.
FROM nginx:stable-alpine3.20 as final

# Copy the built application to the NGiNX image.
COPY --from=build /usr/src/app/dist /usr/src/app/dist

# Copy the NGiNX configuration file.
COPY default.conf.template /etc/nginx/templates/

# Expose the port that the application listens on.
EXPOSE 80

# No ENTRYPOINT or CMD required - NGiNX will start automatically.
