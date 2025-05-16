# Stage 1: Base Runtime Image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base

# Install native dependencies for voice + ffmpeg
RUN apt-get update && apt-get install -y \
    libopus0 \
    libsodium18 \
    ffmpeg \
 && apt-get clean && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Stage 2: Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Erika/Erika.csproj", "Erika/"]
RUN dotnet restore "Erika/Erika.csproj"
COPY . .
WORKDIR "/src/Erika"
RUN dotnet build "Erika.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Erika.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final container with app and dependencies
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 

# Use dotnet to run the app
ENTRYPOINT ["dotnet", "Erika.dll"]
