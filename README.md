# .NET CHALLENGE - PwcDotnet

## 🔹 Índice

1. [Reglas de negocio](#reglas-de-negocio)
2. [Casos de Uso / Funcionalidades](#casos-de-uso--funcionalidades)
3. [Configuración del entorno](#configuración-del-entorno)
4. [Migraciones y persistencia](#migraciones-y-persistencia)
5. [Tests](#tests)
6. [Endpoints disponibles](#endpoints-disponibles)
7. [Swagger y documentación](#swagger-y-documentación)
8. [Seed de datos](#seed-de-datos)
9. [Logs](#logs)
10. [Eventos de Dominio](#eventos-de-dominio)
11. [Principios de diseño aplicados](#principios-de-diseño-aplicados)
12. [Patrones utilizados](#patrones-utilizados)
13. [Angular Challenge](#angular-challenge)

---

## Reglas de negocio

Las reglas de negocio están contenidas dentro de los **Aggregates** y **Value Objects** del dominio. Por ejemplo:

- Un `Rental` no puede ser creado si el auto está reservado o en mantenimiento.
- Un `Rental` no puede ser cancelado si ya empezó.
- Las fechas de un `RentalPeriod` deben ser válidas (End > Start).
- El `Customer` y el `Car` deben ser válidos y existentes.

---

## Casos de Uso / Funcionalidades

Implementados como **Commands** y **Queries** dentro de la capa `Application`:

1. `RegisterRentalCommand`
2. `ModifyRentalCommand`
3. `CancelRentalCommand`
4. `CheckCarAvailabilityQuery`
5. `GetUpcomingCarServicesQuery`
6. `GetTopRentedCarsQuery`
7. `GetTopCarsByBrandModelTypeQuery`
8. `GetDailyStatsQuery`
9. `RegisterCustomerCommand`
10. `LoginCommand`
11. `GetAllRentalsQuery`

---

## Configuración del entorno

Requisitos:
- .NET 9 SDK
- Visual Studio 2022+ o Rider (opcional)

Ejecutar:
```bash
cd src/PwcDotnet.WebAPI

# Si deseás restaurar paquetes y correr la app:
dotnet restore
dotnet run
```

---

## Migraciones y persistencia

Persistencia implementada con EF Core. Se puede configurar para usar SQL Server o InMemory.

Para aplicar migraciones:
```bash
cd src/PwcDotnet.Infrastructure

dotnet ef migrations add Initial --startup-project ../PwcDotnet.WebAPI --context RentalDbContext

dotnet ef database update --startup-project ../PwcDotnet.WebAPI
```

---

## Tests

Se implementaron pruebas **unitarias** y **de integración** utilizando:

- `xUnit`
- `Moq`
- `FluentAssertions`
- `WebApplicationFactory` para integración

Archivo .http disponible para pruebas manuales: `PwcDotnet.WebAPI.http`

### Ejecutar tests + cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coveragereport
```

✅ La cobertura supera el **90%** en la funcionalidad `RegisterRental`

---

## Endpoints disponibles

| Metodo | Path | Descripción |
|--------|------|--------------|
| POST | `/auth/login` | Login del usuario y devolución de JWT |
| POST | `/auth/register` | Registro de nuevo usuario |
| GET | `/auth/me` | Devuelve datos del usuario actual |
| POST | `/rentals/register` | Registra un nuevo alquiler |
| PUT | `/rentals/modify` | Modifica un alquiler existente |
| PUT | `/rentals/cancel` | Cancela un alquiler |
| GET | `/dashboard/top-used-cars?from=...&to=...&locationId=...` | Top autos más alquilados |
| GET | `/dashboard/top-by-brand?from=...&to=...` | Top por marca y modelo |
| GET | `/dashboard/daily-stats?from=...&to=...` | Métricas diarias |
| GET | `/cars/availability?start=...&end=...&carType=...` | Disponibilidad de autos |
| GET | `/cars/upcoming-services?from=...` | Autos con servicios próximos |
| GET | `/customers` | Lista de clientes |
| GET | `/rentals` | Lista de rentals |

---

## Swagger y documentación

La documentación OpenAPI se encuentra disponible en:

```
https://localhost:{PORT}/swagger
```

Incluye:
- JWT Bearer Authentication
- Schema por endpoint

---

## Seed de datos

Al iniciar la aplicación se ejecuta un seed que crea:

- Usuarios y roles (`admin@admin.com` / `Admin123!`)
- Customers, Cars, Rentals y Locations (solo datos mínimos para probar)

---

## Logs

Se utiliza **Serilog**:

- Consola con formato enriquecido
- Archivo local (`logs/log-*.txt`)
- Enriquecido con datos de contexto, thread, traceId, spanId

---

## Eventos de Dominio

Actualmente se dispara:

- `RentalCreatedDomainEvent` cuando se registra un nuevo alquiler.

En futuro PR se conectará con **Azure Durable Functions** para enviar emails o ejecutar flujos externos.

---

## Principios de diseño aplicados

- SOLID
- KISS
- DRY
- YAGNI (You Ain't Gonna Need It)
- Dependency Injection
- Separation of Concerns
- CQRS (Command Query Responsibility Segregation)
- Fail Fast / Early return

---

## Patrones utilizados

- Repository Pattern
- Value Objects
- Aggregates & Domain Events (DDD)
- Mediator Pattern (via MediatR)
- Factory Method (para creación de entidades)
- Logging y Exception Middleware

---

# Angular Challenge

:construction: En progreso - se agregará frontend Angular y se completará esta sección.

