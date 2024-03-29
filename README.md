# OCS test case

[Тестовое задание](./docs/ТЗ.md)

[DevNotes](./docs/DevNotes.md)

## Установка

### Рекомендованный способ: docker compose

1. Переименовать `/src/OCS.Applications/env.template` в `.env`
2. Заполнить указанные в нём поля
3. Перейти в директорию `./src/OCS.Applications`
4. Выполнить `docker-compose up`

### Без контейнера

1. Установить dotnet-sdk-8
2. Установить переменную окружения `ConnectionStrings:ApplicationsApi` с корректной строкой подключения к PostgreSQL или задать её в `./src/OCS.Applications/OCS.Applications.Api/appsettings.json`
3. Выполнить 
   1. `dotnet build ./src/OCS.Applications/` (зависимости будут восстановлены автоматически)
   2. `dotnet run --project ./src/OCS.Applications/OCS.Applications.Api/OCS.Applications.Api.csproj`
