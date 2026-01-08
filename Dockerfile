FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["ProjetoFiap.API/ProjetoFiap.API.csproj", "ProjetoFiap.API/"]
COPY ["ProjetoFiap.Application/ProjetoFiap.Application.csproj", "ProjetoFiap.Application/"]
COPY ["ProjetoFiap.Domain/ProjetoFiap.Domain.csproj", "ProjetoFiap.Domain/"]
COPY ["ProjetoFiap.Infrastructure/ProjetoFiap.Infrastructure.csproj", "ProjetoFiap.Infrastructure/"]
RUN dotnet restore "ProjetoFiap.API/ProjetoFiap.API.csproj"
COPY . .
WORKDIR "/src/ProjetoFiap.API"
RUN dotnet build "ProjetoFiap.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjetoFiap.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjetoFiap.API.dll"]
