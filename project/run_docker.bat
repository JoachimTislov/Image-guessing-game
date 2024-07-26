@echo off
REM Removes a pre-existing container if it exists
docker rm -f laubjutizvezbra

REM Build the Docker image
docker build -t laubjutizvezbra .

REM Run the Docker container
docker run -d -p 8080:80 --name laubjutizvezbra laubjutizvezbra