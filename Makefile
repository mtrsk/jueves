ENVIRONMENT ?= "development"

.SHELLARGS = -euo pipefail

# Include extra environment variables
include .env
export

.DEFAULT_GOAL := build

clean:
	docker compose down -v --remove-orphans

backup:
	echo "TODO"

migrate:
	echo "TODO"

build:
	dotnet build
	dotnet run --project src/App/App.fsproj

run:
	docker compose up

