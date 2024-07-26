# Image guessing game
(Navigate to: path/to/project/Dockerfile directory)

# Build commands:
## Build using pre made bash:
### Windows command prompt:
run_docker.bat

### Mac/Linux:
chmod +x run_docker.sh
./run_docker.sh

## Build using native docker commands:
docker build -t laubjutizvezbra .
docker run -d -p- 8080 --name laubjutizvezbra laubjutizvezbra

## When you have updated the codebase:
### Windows command prompt:
run_docker.bat

### Mac/Linux:
chmod +x run_docker.sh
./run_docker.sh

or

docker rm laubjutizvezbra
docker build -t laubjutizvezbra .
docker run -d -p- 8080 --name laubjutizvezbra laubjutizvezbra