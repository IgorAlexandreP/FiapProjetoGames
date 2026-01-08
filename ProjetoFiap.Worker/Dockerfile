FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["ProjetoFiap.Worker/ProjetoFiap.Worker.csproj", "ProjetoFiap.Worker/"]
COPY ["ProjetoFiap.Domain/ProjetoFiap.Domain.csproj", "ProjetoFiap.Domain/"]
RUN dotnet restore "ProjetoFiap.Worker/ProjetoFiap.Worker.csproj"
COPY . .
WORKDIR "/src/ProjetoFiap.Worker"
RUN dotnet build "ProjetoFiap.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjetoFiap.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjetoFiap.Worker.dll"]
