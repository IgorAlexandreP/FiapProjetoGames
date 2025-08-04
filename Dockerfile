FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/FiapProjetoGames.API/FiapProjetoGames.API.csproj", "src/FiapProjetoGames.API/"]
COPY ["src/FiapProjetoGames.Application/FiapProjetoGames.Application.csproj", "src/FiapProjetoGames.Application/"]
COPY ["src/FiapProjetoGames.Domain/FiapProjetoGames.Domain.csproj", "src/FiapProjetoGames.Domain/"]
COPY ["src/FiapProjetoGames.Infrastructure/FiapProjetoGames.Infrastructure.csproj", "src/FiapProjetoGames.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/FiapProjetoGames.API/FiapProjetoGames.API.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/src/FiapProjetoGames.API"
RUN dotnet build "FiapProjetoGames.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FiapProjetoGames.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Expose port 8080 for Railway
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080

# Start the application
ENTRYPOINT ["dotnet", "FiapProjetoGames.API.dll"] 