# Стадия восстановления зависимостей
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Копируем только csproj файлы
COPY ["Web/RifleAssembly.Authentication.Web/RifleAssembly.Authentication.Web.csproj", "Web/RifleAssembly.Authentication.Web/"]

# Восстанавливаем зависимости
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.local/share/NuGet/v3-cache \
    dotnet restore "Web/RifleAssembly.Authentication.Web/RifleAssembly.Authentication.Web.csproj"

# Стадия сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем результаты восстановления зависимостей
COPY --from=restore /src .

# Копируем остальной код
COPY . .

WORKDIR "/src/Web/RifleAssembly.Authentication.Web"

# Создаем секретный ключ и собираем проект
RUN --mount=type=secret,id=jwt_private_key \
    mkdir -p /app/keys && \
    cp /run/secrets/jwt_private_key /app/keys/private_key.xml && \
    chmod 400 /app/keys/private_key.xml && \
    dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Стадия публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RifleAssembly.Authentication.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
USER $APP_UID
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=publish /app/publish .

# Копируем ключи
COPY --from=build /app/keys/private_key.xml /app/keys/
COPY Web/RifleAssembly.Authentication.Web/Keys/public_key.xml /app/keys/

# Устанавливаем права доступа
RUN chmod 444 /app/keys/public_key.xml && \
    find . -type d -exec chmod 755 {} + && \
    find . -type f -exec chmod 644 {} + && \
    chmod +x RifleAssembly.Authentication.Web.dll

# Запуск приложения
ENTRYPOINT ["dotnet", "RifleAssembly.Authentication.Web.dll"]