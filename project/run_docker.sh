#!/bin/bash

# Removes a pre-existing container if it exists
docker rm -f laubjutizvezbra

# Build the Docker image
docker build -t laubjutizvezbra .

# Run the Docker container
docker run -d -p 8080:80 --name laubjutizvezbra laubjutizvezbra