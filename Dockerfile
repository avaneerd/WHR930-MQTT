# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
COPY ./src .

RUN dotnet publish -c Release -o publish -r debian-arm

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim-arm32v7
WORKDIR /app
COPY --from=build /source/publish/ .
ENTRYPOINT ["./WHR930"]