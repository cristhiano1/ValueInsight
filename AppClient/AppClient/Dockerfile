# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
# runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# build frontend with Node
FROM node:18 AS node-build
WORKDIR /src/team-compass

# copia los ficheros de frontend necesarios para cachear dependencias
COPY team-compass/package*.json ./
COPY team-compass/bun.lock* ./

# install deps (usa npm ci si hay lock, si no cae, hace npm install)
RUN npm ci --silent || npm install --silent

# copia el resto del frontend y crea build (ajusta si usas bun)
COPY team-compass ./
RUN npm run build

# build .NET app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# restore
COPY ["AppClient.csproj", "./"]
RUN dotnet restore "AppClient.csproj"

# copia todo el proyecto
COPY . .

# copiar los assets frontend ya construidos al wwwroot del proyecto
COPY --from=node-build /src/team-compass/dist ./wwwroot

# build y publish
RUN dotnet build "AppClient.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build AS publish
RUN dotnet publish "AppClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Asegurar que los assets frontend se copien también en la imagen final
COPY --from=node-build /src/team-compass/dist ./wwwroot
ENTRYPOINT ["dotnet", "AppClient.dll"]