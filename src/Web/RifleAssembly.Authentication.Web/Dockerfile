# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Web/RifleAssembly.Authentication.Web/RifleAssembly.Authentication.Web.csproj", "Web/RifleAssembly.Authentication.Web/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.local/share/NuGet/v3-cache \
    dotnet restore "Web/RifleAssembly.Authentication.Web/RifleAssembly.Authentication.Web.csproj"

COPY . .
WORKDIR "/src/Web/RifleAssembly.Authentication.Web"
RUN --mount=type=secret,id=jwt_private_key \
    mkdir -p /app/keys && \
    cp /run/secrets/jwt_private_key /app/keys/private_key.xml && \
    chmod 400 /app/keys/private_key.xml && \
    dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RifleAssembly.Authentication.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/keys/private_key.xml /app/keys/
COPY Web/RifleAssembly.Authentication.Web/Keys/public_key.xml /app/keys/

RUN chmod 444 /app/keys/public_key.xml && \
    find . -type d -exec chmod 755 {} + && \
    find . -type f -exec chmod 644 {} + && \
    chmod +x RifleAssembly.Authentication.Web.dll

ENTRYPOINT ["dotnet", "RifleAssembly.Authentication.Web.dll"]