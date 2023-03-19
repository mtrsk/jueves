ENVIRONMENT ?= "development"

SHELL = bash

.SHELLARGS = -euo pipefail

# Include extra environment variables
include .env
export

.DEFAULT_GOAL := build

clean:
	docker compose down -v --remove-orphans

backup:
	echo "TODO"

bootstrap:
	./bootstrap.sh

build:
	dotnet build
	dotnet run --project src/App/App.fsproj

docker:
	docker-compose build --progress=plain

run:
	docker compose up

