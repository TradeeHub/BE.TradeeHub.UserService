﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BE.TradeeHub.UserService/BE.TradeeHub.UserService.csproj", "BE.TradeeHub.UserService/"]
RUN dotnet restore "BE.TradeeHub.UserService/BE.TradeeHub.UserService.csproj"
COPY . .
WORKDIR "/src/BE.TradeeHub.UserService"
RUN dotnet build "BE.TradeeHub.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BE.TradeeHub.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BE.TradeeHub.UserService.dll"]
