#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Web/CrawlerStorage.WebAPI/CrawlerStorage.WebAPI.csproj", "Web/CrawlerStorage.WebAPI/"]
COPY ["Common/CrawlerStorage.Common.Constants/CrawlerStorage.Common.Constants.csproj", "Common/CrawlerStorage.Common.Constants/"]
COPY ["Services/CrawlerStorage.Services.Automapper/CrawlerStorage.Services.Automapper.csproj", "Services/CrawlerStorage.Services.Automapper/"]
COPY ["Data/CrawlerStorage.Data.Models/CrawlerStorage.Data.Models.csproj", "Data/CrawlerStorage.Data.Models/"]
COPY ["Data/CrawlerStorage.Data.Common/CrawlerStorage.Data.Common.csproj", "Data/CrawlerStorage.Data.Common/"]
COPY ["Services/CrawlerStorage.Services/CrawlerStorage.Services.csproj", "Services/CrawlerStorage.Services/"]
COPY ["Data/CrawlerStorage.Data/CrawlerStorage.Data.csproj", "Data/CrawlerStorage.Data/"]
COPY ["Web/CrawlerStorage.WebAPI.Infrastructure/CrawlerStorage.WebAPI.Infrastructure.csproj", "Web/CrawlerStorage.WebAPI.Infrastructure/"]
RUN dotnet restore "./Web/CrawlerStorage.WebAPI/./CrawlerStorage.WebAPI.csproj"
COPY . .
WORKDIR "/src/Web/CrawlerStorage.WebAPI"
RUN dotnet build "./CrawlerStorage.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CrawlerStorage.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrawlerStorage.WebAPI.dll"]