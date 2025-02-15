# Базовый образ для runtime (используем Alpine для уменьшения размера)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем только файлы проектов
COPY ["Web/RifleAssembly.Authentication.Web/RifleAssembly.Authentication.Web.csproj", "Web/RifleAssembly.Authentication.Web/"]

# Восстанавливаем зависимости (кэшируется, пока .csproj не меняется)
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "Web/RifleAssembly.Authentication.Web/RifleAssembly.Authentication.Web.csproj"

# Копируем весь код
COPY . .

# Сборка проекта
WORKDIR "/src/Web/RifleAssembly.Authentication.Web"
RUN dotnet build "RifleAssembly.Authentication.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RifleAssembly.Authentication.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Web/RifleAssembly.Authentication.Web/Keys/public_key.xml /app/keys/public_key.xml

ENTRYPOINT ["dotnet", "RifleAssembly.Authentication.Web.dll"]