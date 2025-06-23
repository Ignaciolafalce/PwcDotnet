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
11. [Arquitectura utilizada](#arquitectura-utilizada)
12. [Estructura de capas](#estructura-de-capas)
13. [Principios de diseño aplicados](#principios-de-diseño-aplicados)
14. [Patrones utilizados](#patrones-utilizados)
15. [Angular Challenge](#angular-challenge)

---

## Reglas de negocio

- Un Rental no puede ser creado si el auto está reservado en esas fechas.
- Un Rental no puede ser creado si el auto tiene servicios programados en esas fechas.
- Un Rental no puede ser cancelado si ya ha comenzado.
- Las fechas de un RentalPeriod deben cumplir que End > Start.
- El Customer y Car deben existir y ser válidos.
- Solo Rentals activos pueden ser modificados.
- Solo Rentals con fecha de inicio en el futuro pueden cancelarse.

---

## Casos de Uso / Funcionalidades

### Rental

- **Registrar alquiler**: Crea un nuevo alquiler si el auto está libre (RegisterRentalCommand)
- **Modificar alquiler**: Cambia fechas o auto (ModifyRentalCommand)
- **Cancelar alquiler**: Solo si no ha iniciado aún (CancelRentalCommand)
- **Listar alquileres**: Trae todos los rentals activos o existentes (GetAllRentalsQuery)

- **Registrar cliente**: Permite registrar un nuevo cliente (RegisterCustomerCommand)
- **Listar clientes**: Devuelve los clientes existentes (GetAllCustomersQuery)

- **Disponibilidad de autos**: Chequea autos disponibles en cierto rango (CheckCarAvailabilityQuery)
- **Servicios próximos**: Devuelve autos con servicios próximos (GetUpcomingCarServicesQuery)

- **Top más alquilados**: Ranking de autos más alquilados (GetTopRentedCarsQuery)
- **Top por marca/tipo**: Ranking agrupado (GetTopCarsByBrandModelTypeQuery)
- **Métricas diarias**: Cantidades por día y por ubicación (GetDailyStatsQuery)

- **Login**: Devuelve JWT si las credenciales son correctas (TokenCommand)
- **Me**: Retorna info del usuario actual autenticado
- **Register**: Crea un nuevo usuario y devuelve su token (RegisterCommand)

✅ **RegisterRentalCommand** fue testeado con más del 90% de cobertura (unit + integration).

---

## Configuración del entorno

Requisitos:

- .NET 9 SDK
- Visual Studio 2022+ o Rider (opcional)

Ejecutar:

```bash
cd src/PwcDotnet.WebAPI

dotnet restore
dotnet run
```
`AzureDurableFunctions:Enable` está activado en `appsettings.json`, también deberás ejecutar el proyecto `PwcDotnet.AzureDurableFunctions` pudiendo poner como startup de la soluccion los dos proyectos incluso.

Si estas en visual studio asegurate por lo menos de que tu Stratup project sea minimamente PwcDotne.WebAPI (en caso de usar azuredurablefunctions local agregar el proyecto al startup de la solucion).


---

## Migraciones y persistencia

Por defecto, el sistema utiliza una base de datos **en memoria** (`InMemoryDatabase`) para facilitar las pruebas rápidas y el testing sin necesidad de un servidor externo.  
Si deseás usar una base de datos **persistente** (como SQL Server), seguí los siguientes pasos:

#### Configurar la base de datos

Editá el archivo `appsettings.json` para desactivar la base en memoria:

```json
"UseInMemoryDb": true // o false si querés usar SQL Server
```

Si `false`(Persistir en SqlServer), agregá tu connection string en:

```json
"UseInMemoryDb": false,
"ConnectionStrings": {
  "PwcConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=PWCChallengeDb;Trusted_Connection=True;"
}
```

### Aplicar migraciones:

```bash
# Update de la migracion del proyecto de infrastructura

dotnet ef migrations add InitialCreate --project ../PwcDotnet.Infrastructure --startup-project .
dotnet ef database update --project ../PwcDotnet.Infrastructure --startup-project .
```
> Asegurate de que el proyecto `PwcDotnet.WebAPI` esté seteado como `Startup Project` y que tenga acceso al archivo `appsettings.json`

---

## Tests

Se implementaron pruebas unitarias y de integración con:

- xUnit
- Moq
- FluentAssertions
- WebApplicationFactory (integration)

### Ejecutar tests
```bash
# Ejecutar tests unitarios
dotnet test tests/PwcDotnet.UnitTests

# Ejecutar tests de integración
dotnet test tests/PwcDotnet.IntegrationTests
```

> Tambien existen 📁TestCases de endopints en el `TestCases/PwcDotnet.WebAPI.http` file del proyecto PwcDotnet.WebAPI. Ideal para probar todos los endpoints de forma rápida.

#### 🧪 Unit Tests asociado al challenge

- `RentalBusinessRulesTests`: Valida reglas de negocio del agregado Rental.
- `RegisterRentalCommandHandlerTests`: Valida distintos flujos del handler (OK, auto reservado, auto en service, etc.)
- `RentalCreatedDomainEventTests`: Verifica que se genere el evento correctamente al crear un alquiler.

#### 🌐 Integration Tests asociados al challenge

- Endpoint `/rentals/register` probado end-to-end (semilla, login, request, verificación).

### Ejecutar tests + cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coveragereport
```

🔎 **Cobertura del caso de uso RegisterRental supera el 90%**.

---

## Endpoints disponibles

| Metodo | Path                                                      | Descripción                           |
| ------ | --------------------------------------------------------- | ------------------------------------- |
| POST   | `/auth/login`                                             | Login del usuario y devolución de JWT |
| POST   | `/auth/register`                                          | Registro de nuevo usuario             |
| GET    | `/auth/me`                                                | Devuelve datos del usuario actual     |
| POST   | `/rentals/register`                                       | Registra un nuevo alquiler            |
| PUT    | `/rentals/modify`                                         | Modifica un alquiler existente        |
| PUT    | `/rentals/cancel`                                         | Cancela un alquiler                   |
| GET    | `/dashboard/top-used-cars?from=...&to=...&locationId=...` | Top autos más alquilados              |
| GET    | `/dashboard/top-by-brand?from=...&to=...`                 | Top por marca y modelo                |
| GET    | `/dashboard/daily-stats?from=...&to=...`                  | Métricas diarias                      |
| GET    | `/cars/availability?start=...&end=...&carType=...`        | Disponibilidad de autos               |
| GET    | `/cars/upcoming-services?from=...`                        | Autos con servicios próximos          |
| GET    | `/customers`                                              | Lista de clientes                     |
| GET    | `/rentals`                                                | Lista de rentals                      |

---

## Swagger y documentación

Disponible en:

```
https://localhost:{PORT}/swagger
```

Incluye:

- JWT Bearer Authentication
- Schema por endpoint con input/output

---

## Seed de datos

Se ejecuta automáticamente al correr la API. Contiene:

- Usuario Admin: `admin@admin.com` / `Admin123!`
- Customers, Rentals y Cars
- Autos con servicios próximos

- Se incluyen datos semilla en memoria (`SeedData.cs` en PwcDotnet.Infrastructure/Data) si activás In-Memory en `appsettings.json`.
- Podés comentar la línea `SeedData.InitializeAsync()` en el Program.cs dE PwcDotnet.WebAPI si no querés usarlo.

---

## Logs

Implementado con **Serilog**:

- Logs enriquecidos (threadId, traceId, etc.)
- Consola
- Archivos locales: `logs/log-*.txt`
- Preparado para OpenTelemetry

---

## Eventos de Dominio

🔄 `RentalCreatedDomainEvent` se dispara al registrar un nuevo alquiler.

> ☁️ Integrado con Azure Durable Functions (AzLocal) para ejecutar side-effects -> envio de email del challenge.
> 📬 El email del cliente se recupera dinámicamente desde el repositorio antes de disparar la Durable Function.

### Configuración

```json
"AzureDurableFunctions": {
  "Enable": true,
  "Url": "http://localhost:7081/api"
}
```

> Si `Enable` está en `true`, el DomainEvent llamará al endpoint HTTP `SendRentalEmailOrchestration_HttpStart` y disparará una orquestación Durable.

---

## Arquitectura utilizada

📐 Basado en Clean Architecture + DDD:


- **Separación de responsabilidades** (Separation of Concerns)
- **Mantenibilidad**
- **Testing**
- **Alta cohesión y bajo acoplamiento**
- **Escalabilidad** y modularidad
- **Inversión de dependencias** (Dependency Inversion Principle)
- **Inversión de control** (Inversion of Control Principle)
- **Independencia de frameworks**

---

## Estructura de capas

```
src/
├── PwcDotnet.Domain                -> Entidades, ValueObjects, reglas del negocio
├── PwcDotnet.Application           -> Casos de uso (CQRS), validaciones, interfaces
├── PwcDotnet.Infrastructure        -> EF Core, configuraciones, repositorios
├── PwcDotnet.WebAPI                -> Endpoints Minimal API, Swagger, logging, DI
├── PwcDotnet.AzureDurableFunctions               -> Orchestrators, activities (ej SendEmailOrchestrator and activities)
├── PwcDotnet.AngularSPA               -> SPA Client

tests/
├── PwcDotnet.UnitTests             -> Tests de dominio y aplicación
├── PwcDotnet.IntegrationTests      -> Tests end-to-end con WebApplicationFactory
```

---

## 🧠 Caching

Se aplicaron dos mecanismos de cacheo para mejorar la performance del sistema:

---

### ✅ 1. Response Caching (Minimal API)

Se implementó un **middleware personalizado** para simular el atributo `[ResponseCache]`, ya que en Minimal APIs no está disponible por defecto.

#### 📦 Implementación

```csharp
// Extension en WebAPI.Extensions.EndpointCachingExtensions.cs
public static class EndpointCachingExtensions
{
    public static RouteHandlerBuilder WithResponseCache(this RouteHandlerBuilder builder, int seconds)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var httpContext = context.HttpContext;
            httpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(seconds)
            };
            return await next(context);
        });
    }
}
```

#### 💪 Ejemplo de uso

```csharp
group.MapGet("/", GetAllAsync).WithResponseCache(1);
```

> Este ejemplo aplica un cache público por 1 segundo al endpoint `/customers`.

---

### ✅ 2. In-Memory Caching en capa de Aplicación

Se utilizó `IMemoryCache` para almacenar temporalmente los resultados de la query `GetAllCustomersQuery`, mejorando la velocidad y reduciendo lecturas innecesarias a base de datos.

#### 📦 Implementación

```csharp
public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
{
    var customerDtoList = await _cache.GetOrCreateAsync("customers_cache", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3);

        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FullName = c.FullName,
            Email = c.Email
        }).ToList();
    });

    return customerDtoList ?? new List<CustomerDto>();
}
```

> Se utiliza una key `"customers_cache"` con una expiración absoluta de 3 segundos.

> 💡 Ambos mecanismos son independientes y complementarios:
>
> - `IMemoryCache`: actúa en la capa de **Aplicación**.
> - `ResponseCache`: actúa sobre la **respuesta HTTP** (capa Web/API).



## Principios de diseño aplicados

- **SOLID** (especialmente SRP, DIP, OCP)
- **KISS** (Keep It Simple)
- **YAGNI** (You Aren't Gonna Need It)
- **DRY** (Don't Repeat Yourself)
- **Separation of Concerns**
- **Inversion of Control Principle**
- **Explicit Dependencies**
- **DDD** 
---

## Patrones utilizados
- **DDD Patterns** (Aggregates, Entities, ValueObjets, etc)
- **CQRS** (Command Query Responsibility Segregation)
- **Repository Pattern** (incluyendo un repositorio genérico)
- **Dependency Injection** (nativa de .NET)
- **Validation Behavior** (usando MediatR y FluentValidation)
- **Extension Methods** (para configuración)
- **Unit of Work** (a través del contexto de EF Core)
- **Minimal APIs** (.NET 8)
- Domain Events
- Mediator Pattern (MediatR)
- Exception Middleware

---

# Angular Challenge 🚧

Sección en construcción. Aquí se documentará el frontend Angular cuando se agregue a la solución.

---

Gracias por leer 🚀

