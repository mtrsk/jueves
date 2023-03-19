ARG DOTNET_VERSION=6.0
# ------------
# Build Image
# ------------
FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION as build

# Install tools
RUN apt-get update \
    && apt-get install \
        -y --no-install-recommends sqlite3

# Setup build directory & copy source code
WORKDIR /app
COPY ./src /app/src
COPY jueves.sln bootstrap.sh ./
RUN mkdir database

RUN chmod 700 bootstrap.sh
RUN ./bootstrap.sh

# ----------------
# Publishing Image
# ----------------
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# -------------
# Runtime Image
# -------------
FROM mcr.microsoft.com/dotnet/aspnet:$DOTNET_VERSION as runtime

RUN apt-get update \
    && apt-get install \
        -y --no-install-recommends sqlite3

COPY --from=publish /app/publish /app
WORKDIR /app
RUN mkdir database
COPY ./assets ./assets
COPY ./docker_entrypoint.sh .
RUN chmod 700 ./docker_entrypoint.sh
ENTRYPOINT [ "/app/docker_entrypoint.sh" ]
