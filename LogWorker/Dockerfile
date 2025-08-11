FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamos csproj primero para cachear restore
COPY ./Metrics/Metrics.csproj ./Metrics/
COPY ./LogWorker/LogWorker.csproj ./LogWorker/

RUN dotnet restore ./LogWorker/LogWorker.csproj

# Copiamos el resto del código
COPY ./Metrics/ ./Metrics/
COPY ./LogWorker/ ./LogWorker/

# DEBUG: asegurar que el archivo está en el contexto y con el casing correcto
RUN test -f ./LogWorker/Logs/LogOptions.cs || (echo "❌ NO se encontró LogWorker/Logs/LogOptions.cs" && ls -la ./LogWorker && ls -la ./LogWorker/Logs && exit 1)

RUN dotnet publish ./LogWorker/LogWorker.csproj -c $BUILD_CONFIGURATION -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "LogWorker.dll"]
