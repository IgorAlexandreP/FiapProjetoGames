# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore src/FiapProjetoGames.API/FiapProjetoGames.API.csproj
RUN dotnet publish src/FiapProjetoGames.API/FiapProjetoGames.API.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "FiapProjetoGames.API.dll"] 