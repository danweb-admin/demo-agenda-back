FROM mcr.microsoft.com/dotnet/aspnet:6.0-bookworm-slim AS base

WORKDIR /app
EXPOSE 80

# Instalação do LibreOffice
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libreoffice \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*


FROM mcr.microsoft.com/dotnet/sdk:6.0-bookworm-slim AS build

WORKDIR /src

# Copia a solução
COPY "Solucao.sln" "Solucao.sln"

# Copia os projetos
COPY "Solucao.API/Solucao.API.csproj" "Solucao.API/Solucao.API.csproj"
COPY "Solucao.Application/Solucao.Application.csproj" "Solucao.Application/Solucao.Application.csproj"
COPY "Solucao.CrossCutting/Solucao.CrossCutting.csproj" "Solucao.CrossCutting/Solucao.CrossCutting.csproj"
COPY "Solucao.Test/Solucao.Test.csproj" "Solucao.Test/Solucao.Test.csproj"

# Restore
RUN dotnet restore "Solucao.sln"

# Copia o restante dos arquivos
COPY . .

# Build
WORKDIR "/src/Solucao.API"

RUN dotnet build "Solucao.API.csproj" \
    -c Release \
    -o /app/build

# Testes
RUN dotnet test


FROM build AS publish

RUN dotnet publish "Solucao.API.csproj" \
    -c Release \
    -o /app/publish


FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Solucao.API.dll"]