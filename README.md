# FinanceTracker

Трекер личных финансов на ASP.NET Core с Clean Architecture.

---

## Функционал

- **Пользователи** — регистрация и вход через JWT.
- **Счета** — создание, обновление, удаление (наличные, карта).
- **Транзакции** — фиксация доходов и расходов по категориям, обновление баланса.
- **Бюджеты** — месячные лимиты на категории с проверкой превышения.
- **Аналитика** — сводка расходов за месяц по категориям с сравнением с лимитами.
- **Администрирование** — просмотр и удаление любых данных (роль Admin).

---

## Технологии

- .NET 10, ASP.NET Core
- Entity Framework Core + PostgreSQL
- JWT-аутентификация
- BCrypt (хеширование паролей)
- Swagger с поддержкой JWT
- xUnit, FluentAssertions, Moq (unit-тесты)

---

## Архитектура

Проект построен по Clean Architecture (Роберт Мартин). Слои:

| Слой | Назначение |
|------|-----------|
| **Domain** | Сущности (Account, Transaction, Category, Budget, User) и перечисления |
| **Application** | Use Cases (сервисы), интерфейсы репозиториев, DTO, Mapper |
| **Infrastructure** | Реализация репозиториев (EF Core), JWT-генератор, хеширование паролей, DbContext |
| **API** | HTTP-контроллеры, Swagger, JWT-настройка, DI-регистрация |
| **Shared** | ApiResponse\<T\>, кастомные исключения |

Зависимости направлены от внешних слоёв к внутренним. Domain не зависит ни от кого.

---

## Паттерны проектирования

| Паттерн | Применение | Обоснование |
|---------|-----------|-------------|
| **Repository** | Интерфейсы в Application, реализации в Infrastructure | Изоляция доступа к данным, упрощение тестирования |
| **Strategy** | IPasswordHasher (BCrypt), ITokenGenerator (JWT) | Замена алгоритмов без изменения бизнес-логики |
| **Chain of responsibility** | ExceptionMiddleware в слое API | Единая точка перехвата исключений из всех слоёв, преобразование в HTTP-ответы с правильными кодами |

---

## Установка и запуск

1. Установить .NET 10 SDK и PostgreSQL.

2. Создать `appsettings.Development.json` в `FinanceTracker.API`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=FinanceTracker;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-32-characters-long!!!",
    "Issuer": "FinanceTracker",
    "Audience": "FinanceTracker",
    "ExpiryHours": 24
  }
}
```
3. Запустить:
```bash
dotnet run --project FinanceTracker.API
```
4. Открыть Swagger:
```bash
https://localhost:7090/swagger
```

## Тесты

40 unit-тестов (xUnit + Moq + FluentAssertions). Запуск:
```bash
dotnet test
