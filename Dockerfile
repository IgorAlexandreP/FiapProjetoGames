# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/FiapProjetoGames.API/FiapProjetoGames.API.csproj", "src/FiapProjetoGames.API/"]
COPY ["src/FiapProjetoGames.Application/FiapProjetoGames.Application.csproj", "src/FiapProjetoGames.Application/"]
COPY ["src/FiapProjetoGames.Domain/FiapProjetoGames.Domain.csproj", "src/FiapProjetoGames.Domain/"]
COPY ["src/FiapProjetoGames.Infrastructure/FiapProjetoGames.Infrastructure.csproj", "src/FiapProjetoGames.Infrastructure/"]

RUN dotnet restore "src/FiapProjetoGames.API/FiapProjetoGames.API.csproj"

COPY . .

WORKDIR /src/FiapProjetoGames.API
RUN dotnet build "FiapProjetoGames.API.csproj" -c Release -o /app/build

# Etapa 2 - Publicação
FROM build AS publish
WORKDIR /src/FiapProjetoGames.API
RUN dotnet publish "FiapProjetoGames.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3 - Runtime final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "FiapProjetoGames.API.dll"]
