# See https://aka.ms/customizecontainer to learn how to customize your debug container

# ----------------------------
# Base runtime image
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ----------------------------
# Build stage
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ?? IMPORTANTE: NO repetir carpeta porque el context ya es ./ValueInsight.Frontend
COPY ["ValueInsight.Frontend.csproj", "./"]
RUN dotnet restore "ValueInsight.Frontend.csproj"

COPY . .
RUN dotnet build "ValueInsight.Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ----------------------------
# Publish stage
# ----------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ValueInsight.Frontend.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ----------------------------
# Final stage
# ----------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ValueInsight.Frontend.dll"]