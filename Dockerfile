FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Instalação do LibreOffice
RUN apt-get update && \
    apt-get install -y libreoffice && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "Solucao.sln" "Solucao.sln"

COPY "Solucao.API/Solucao.API.csproj" "Solucao.API/Solucao.API.csproj"
COPY "Solucao.Application/Solucao.Application.csproj" "Solucao.Application/Solucao.Application.csproj"
COPY "Solucao.CrossCutting/Solucao.CrossCutting.csproj" "Solucao.CrossCutting/Solucao.CrossCutting.csproj"
COPY "Solucao.Test/Solucao.Test.csproj" "Solucao.Test/Solucao.Test.csproj"


RUN dotnet restore "Solucao.sln"

COPY . .

WORKDIR "/src/Solucao.API"
RUN dotnet build "Solucao.API.csproj" -c Release -o /app/build

RUN dotnet test

FROM build AS publish
RUN dotnet publish "Solucao.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Solucao.API.dll"]