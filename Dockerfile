ARG DOTNET_VERSION=6.0
# ------------
# Build Image
# ------------
FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION as build

# Install tools
RUN apt-get update \
    && apt-get install \
        -y --no-install-recommends make sqlite3

# Setup build directory & copy source code
WORKDIR /app
COPY ./src /app/src
COPY jueves.sln Makefile ./

RUN make bootstrap

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
        -y --no-install-recommends make sqlite3

RUN make bootstrap

COPY --from=publish /app/publish /app
WORKDIR /app
ENTRYPOINT [ "sh", "-c" ]
CMD [ "dotnet App.dll" ]
