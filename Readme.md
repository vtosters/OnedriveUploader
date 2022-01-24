# OnedriveUploader
Позволяет заливать файлы на **OneDrive Business** и получать **прямые** ссылки на них

## Требования
- .NET 6

[Создайте своё приложение в Azure](https://docs.microsoft.com/ru-ru/onedrive/developer/rest-api/getting-started/app-registration?view=odsp-graph-online) и предоставьте следующие разрешения api:
- Files.ReadWrite.All
- Sites.ReadWrite.All
- User.Read

## Конфигурация
Нужно поставить свои значения в `appsettings.json`

### Onedrive
- Username - логин
- Password - пароль
- TenantId - Идентификатор каталога из созданного вами приложения
- ClientId - Идентификатор приложения

### UploaderPassword
Пароль, требуемый для входа в это веб-приложение

## Запуск
`dotnet OnedriveUploader.dll`
или через Docker-файл

## API
Загрузить файл через REST API

**POST**: /api/upload?token=**UploaderPassword**

`Content-Type: multipart/form-data`