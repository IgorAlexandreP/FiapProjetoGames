FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
RUN dotnet restore src/FiapProjetoGames.API/FiapProjetoGames.API.csproj
RUN dotnet publish src/FiapProjetoGames.API/FiapProjetoGames.API.csproj -c Release -o out
EXPOSE 80
ENTRYPOINT ["dotnet", "out/FiapProjetoGames.API.dll"] 