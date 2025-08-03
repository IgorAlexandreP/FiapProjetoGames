FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FiapProjetoGames.API/FiapProjetoGames.API.csproj", "src/FiapProjetoGames.API/"]
COPY ["src/FiapProjetoGames.Application/FiapProjetoGames.Application.csproj", "src/FiapProjetoGames.Application/"]
COPY ["src/FiapProjetoGames.Domain/FiapProjetoGames.Domain.csproj", "src/FiapProjetoGames.Domain/"]
COPY ["src/FiapProjetoGames.Infrastructure/FiapProjetoGames.Infrastructure.csproj", "src/FiapProjetoGames.Infrastructure/"]
RUN dotnet restore "src/FiapProjetoGames.API/FiapProjetoGames.API.csproj"
COPY . .
WORKDIR "/src/src/FiapProjetoGames.API"
RUN dotnet build "FiapProjetoGames.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FiapProjetoGames.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "FiapProjetoGames.API.dll"] 