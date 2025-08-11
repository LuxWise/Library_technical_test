# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ./Metrics/Metrics.csproj ./Metrics/
COPY ./LogWorker/LogWorker.csproj ./LogWorker/

RUN dotnet restore ./LogWorker/LogWorker.csproj

COPY ./Metrics/ ./Metrics/
COPY ./LogWorker/ ./LogWorker/

RUN dotnet publish ./LogWorker/LogWorker.csproj -c $BUILD_CONFIGURATION -o /app

# runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "LogWorker.dll"]
