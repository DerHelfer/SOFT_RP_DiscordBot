# Discord Bot для SOFT RP

Discord бот для управления администраторами сервера с системой профилей.

## Запуск

```bash
dotnet run
```

## Настройка

1. Создайте файл `appsettings.json`:
```json
{
  "DiscordToken": "ВАШ_ТОКЕН_БОТА"
}
```

2. Получите токен бота в [Discord Developer Portal](https://discord.com/developers/applications)

## Команды

- `/addadmin` - Добавить администратора (требует SteamID формата STEAM_0:x:xxxxx)
- `/profile` - Показать профиль пользователя

## Структура проекта

```
├── Builders/
│   └── EmbedBuilders.cs     # Создание Discord embed'ов
├── Validators/
│   └── SteamIdValidator.cs  # Валидация SteamID
├── AdminProfile.cs          # Модель профиля администратора
├── ProfileModule.cs         # Discord команды
├── ProfileService.cs        # Сервис управления профилями
└── Program.cs              # Точка входа
```

## Требования

- .NET 8.0
- Discord.Net библиотека