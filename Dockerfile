# Etapa 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos .csproj primeiro
COPY ["src/FiapProjetoGames.API/FiapProjetoGames.API.csproj", "src/FiapProjetoGames.API/"]
COPY ["src/FiapProjetoGames.Application/FiapProjetoGames.Application.csproj", "src/FiapProjetoGames.Application/"]
COPY ["src/FiapProjetoGames.Domain/FiapProjetoGames.Domain.csproj", "src/FiapProjetoGames.Domain/"]
COPY ["src/FiapProjetoGames.Infrastructure/FiapProjetoGames.Infrastructure.csproj", "src/FiapProjetoGames.Infrastructure/"]

# Restaura dependências
RUN dotnet restore "src/FiapProjetoGames.API/FiapProjetoGames.API.csproj"

# Copia todo o restante do projeto
COPY . .

# Publica diretamente
RUN dotnet publish "src/FiapProjetoGames.API/FiapProjetoGames.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2 - Runtime final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia do publish
COPY --from=publish /app/publish .

# Expõe porta
EXPOSE 5000

# Entrypoint
ENTRYPOINT ["dotnet", "FiapProjetoGames.API.dll"]
