# Arquitectura - Hotel Booking Platform

## 🏗️ Diagrama General

```
┌─────────────────────────────────────────────────────────┐
│                     Frontend (Next.js)                   │
│              React + TypeScript + TailwindCSS            │
└────────────────────┬────────────────────────────────────┘
                    │ HTTP/REST
┌────────────────────▼────────────────────────────────────┐
│                   API Layer (.NET 8)                     │
│  Controllers + Middleware + ApiVersioning (v1.0)       │
├─────────────────────────────────────────────────────────┤
│  Application Layer (CQRS Pattern)                       │
│  ┌──────────────────┐    ┌──────────────────┐           │
│  │    Commands      │    │     Queries      │           │
│  │  (Write Side)    │    │   (Read Side)    │           │
│  │  EF Core (Orm)   │    │  Dapper (SQL)    │           │
│  └──────────────────┘    └──────────────────┘           │
│  MediatR | FluentValidation | AutoMapper                │
├─────────────────────────────────────────────────────────┤
│  Domain Layer                                           │
│  Entities | ValueObjects | DomainEvents | Interfaces  │
├─────────────────────────────────────────────────────────┤
│  Infrastructure Layer                                   │
│  ┌──────────────────────────────────────────────────┐   │
│  │  EF Core DbContext   │  Dapper Repositories    │   │
│  │  (Write Optimized)   │  (Read Optimized)       │   │
│  │                      │  Paging & Sorting       │   │
│  └──────────────────────────────────────────────────┘   │
│  Unit of Work Pattern | Transactions | Concurrency      │
└─────────────────────────────────────────────────────────┘
                         │
         ┌───────────────┴───────────────┐
         │                               │
    ┌────▼─────┐              ┌──────────▼──────┐
    │ SQL SERVER│              │ Serilog Logs    │
    │  Database │              │ +Tracing        │
    └───────────┘              └─────────────────┘
```

## 🔄 Flujo CQRS

### Commands (Escritura con EF Core)

```
Request → Controller → Command → Validator → Handler
                                  ↓
                         Unit of Work
                                  ↓
                         EF Core DbContext
                                  ↓
                         Transacción + Commit
                                  ↓
                         Domain Event
```

### Queries (Lectura con Dapper)

```
Request → Controller → Query → Handler
                               ↓
                         Dapper (Raw SQL)
                               ↓
                         DTOs (Optimizados)
                               ↓
                         Response
```

## 🏛️ Capas de Arquitectura Limpia

### 1. Domain Layer (`HotelBooking.Domain`)
- **Responsabilidad**: Lógica de negocio puro
- **Dependencias**: Ninguna (no depende de otros proyectos)
- **Contiene**:
  - Entidades: `Hotel`, `RoomType`, `Booking`, `Guest`, `Payment`, etc.
  - ValueObjects: Enumeraciones y tipos de valor
  - Interfaces: `IRepository`, `IUnitOfWork`
  - Excepciones de dominio
  - Eventos de dominio

### 2. Application Layer (`HotelBooking.Application`)
- **Responsabilidad**: Orquestación de casos de uso (CQRS)
- **Dependencias**: Domain
- **Contiene**:
  - **Commands**: Acciones de escritura (CreateHotel, CreateBooking, ConfirmBooking, etc.)
  - **Queries**: Acciones de lectura (GetHotels, GetBookings, SearchAvailability, etc.)
  - **CommandHandlers**: Implementan lógica de comando con EF Core
  - **QueryHandlers**: Implementan lógica de consulta con Dapper
  - **Validators**: FluentValidation para validar input
  - **DTOs**: Objetos de transferencia de datos
  - **Mappings**: Conversiones entidad → DTO

### 3. Infrastructure Layer (`HotelBooking.Infrastructure`)
- **Responsabilidad**: Implementación técnica (DB, Logging, etc.)
- **Dependencias**: Domain + Application
- **Contiene**:
  - **DbContext**: EF Core context (escritura)
  - **Repositories**: Implementación de IRepository (EF Core)
  - **QueryHandlers**: Implementación con Dapper (lectura)
  - **Unit of Work**: Gestión de transacciones
  - **Migrations**: Scripts de base de datos

### 4. API Layer (`HotelBooking.Api`)
- **Responsabilidad**: Orquestación REST
- **Dependencias**: Application + Infrastructure
- **Contiene**:
  - **Controllers**: Endpoints REST versionados
  - **Middleware**: Logging, Correlation ID, Error Handling
  - **Program.cs**: Configuración de servicios (DI)

---

## 🔒 Control de Concurrencia

### Estrategia: Concurrency Token (RowVersion)

Para evitar **overbooking**, usamos `RowVersion` en:
- `RoomInventory`: Token de fila para control de disponibilidad
- `Booking`: Token de fila para evitar modificaciones simultáneas

**Flujo**:
1. Leer `RoomInventory` CON su `RowVersion`
2. Validar disponibilidad
3. Actualizar reserva con el `RowVersion` original
4. Si `RowVersion` cambió → `DBConcurrencyException` → Reintentar

```csharp
var inventory = await context.RoomInventories.FirstOrDefaultAsync(
    r => r.Id == roomId);

// EF Core cotejará el RowVersion automáticamente en SaveChanges()
inventory.AvailableRooms -= bookedRooms;
await context.SaveChangesAsync(); // Lanza excepción si RowVersion no coincide
```

---

## 🔑 Pattern: Idempotencia

Para garantizar que operaciones repetidas con el mismo `Idempotency-Key` devuelvan el mismo resultado:

```
Header: Idempotency-Key: UUID

1. Verificar si ya existe IdempotencyRecord con esta key
2. Si existe y no está expirado → devolver respuesta guardada
3. Si no existe → procesar la operación
4. Guardar: Key + Request Hash + Response Status + Response Body
```

**Tabla**: `IdempotencyRecords`

---

## 🎯 Entidades y Relaciones

```
Hotel (1) ──── (*) RoomType
         ├──── (*) Booking
         └──── (*) AuditLog

RoomType (1) ──── (*) RoomInventory
         ├──── (*) RatePlan
         └──── (*) Booking

Booking (1) ──── (*) Payment
        ├──── (1) Guest
        ├──── (1) Hotel
        └──── (1) RoomType

Guest (1) ──── (*) Booking

RoomInventory: Almacena disponibilidad por fecha y tipo de habitación
(HotelId, RoomTypeId, Date, AvailableRooms, ReservedRooms, RowVersion)
```

---

## 📦 Paquetes NuGet Principales

| Paquete | Propósito | Capa |
|---------|-----------|------|
| **Entity Framework Core** | ORM para escritura | Infrastructure |
| **Dapper** | Mapper SQL para lectura | Infrastructure |
| **MediatR** | CQRS Pattern | Application |
| **FluentValidation** | Validación de input | Application |
| **Serilog** | Logging estructurado | Api/Infrastructure |
| **Asp.Versioning** | Versionado de API | Api |
| **Swagger** | Documentación interactiva | Api |

---

## 🚀 Operaciones Críticas (Transacciones)

### CreateBooking (Operación Crítica)

```
BEGIN TRANSACTION
  1. Validar hotel, guest, room type
  2. Calcular precio total
  3. LOCK RoomInventory (SELECT ... FOR UPDATE)
  4. Verificar disponibilidad (AvailableRooms >= requested)
  5. Actualizar RoomInventory (AvailableRooms --, ReservedRooms ++)
  6. Crear Booking
  7. Guardar IdempotencyRecord
  8. PublishDomainEvent(BookingCreated)
COMMIT TRANSACTION
```

**Manejo de Excepciones**:
- `DbUpdateConcurrencyException` → Reintentar (2-3 intentos)
- `DbUpdateException` → Rollback + Error 422 (Unprocessable Entity)

---

## 🔍 Observabilidad

### Logging Estructurado (Serilog)

```json
{
  "Timestamp": "2026-03-09T10:30:45.1234567Z",
  "Level": "Information",
  "CorrelationId": "9f7d8c5a-2e3b-4a9d-8f7c-5e3a2b1c4d8f",
  "Message": "Booking created successfully",
  "UserId": "user-123",
  "BookingId": "booking-456",
  "Hotel": "Hotel Paradise",
  "DurationMs": 234
}
```

### CorrelationId (Middleware)

Cada request genera un `CorrelationId` único que se propaga en:
- Todos los logs
- Respuestas HTTP (Header: `X-Correlation-Id`)
- Trazas distribuidas

### Métricas (OpenTelemetry)

- Request count
- Request duration (ms)
- Error rate
- Database query count

---

## 🗄️ Schema SQL Clave

### RoomInventory (Control de Concurrencia)

```sql
CREATE TABLE RoomInventories (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    RoomTypeId UNIQUEIDENTIFIER NOT NULL,
    Date DATE NOT NULL,
    AvailableRooms INT NOT NULL,
    ReservedRooms INT NOT NULL,
    RowVersion ROWVERSION NOT NULL,  -- Concurrency Control
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_RoomInventory_RoomType FOREIGN KEY (RoomTypeId) 
        REFERENCES RoomTypes(Id),
    CONSTRAINT CK_AvailableRooms_NonNegative CHECK (AvailableRooms >= 0),
    CONSTRAINT CK_ReservedRooms_NonNegative CHECK (ReservedRooms >= 0),
    CONSTRAINT UQ_RoomType_Date UNIQUE (RoomTypeId, Date)
);

CREATE INDEX IX_RoomInventory_RoomTypeId_Date 
    ON RoomInventories (RoomTypeId, Date);
```

### Booking (Con RowVersion)

```sql
CREATE TABLE Bookings (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    HotelId UNIQUEIDENTIFIER NOT NULL,
    RoomTypeId UNIQUEIDENTIFIER NOT NULL,
    GuestId UNIQUEIDENTIFIER NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    NumberOfRooms INT NOT NULL,
    NumberOfGuests INT NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL, -- 1=Pending, 2=Confirmed, 3=Cancelled, 4=Completed
    RowVersion ROWVERSION NOT NULL,  -- Concurrency Control
    CreatedAt DATETIME2 NOT NULL,
    ConfirmedAt DATETIME2 NULL,
    CancelledAt DATETIME2 NULL,
    CONSTRAINT FK_Booking_Hotel FOREIGN KEY (HotelId) REFERENCES Hotels(Id),
    CONSTRAINT FK_Booking_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(Id),
    CONSTRAINT FK_Booking_Guest FOREIGN KEY (GuestId) REFERENCES Guests(Id),
    CONSTRAINT CK_CheckInOut CHECK (CheckOutDate > CheckInDate)
);

CREATE INDEX IX_Booking_GuestId ON Bookings (GuestId);
CREATE INDEX IX_Booking_HotelId ON Bookings (HotelId);
CREATE INDEX IX_Booking_RoomTypeId ON Bookings (RoomTypeId);
CREATE INDEX IX_Booking_Status ON Bookings (Status);
```

---

## 🔄 Eventos de Dominio

### BookingCreated

```csharp
public class BookingCreatedEvent : IDomainEvent
{
    public int AggregateId { get; }              // Booking ID
    public int HotelId { get; }
    public int GuestId { get; }
    public DateTime CheckInDate { get; }
    public DateTime CheckOutDate { get; }
    public int NumberOfRooms { get; }
    public DateTime OccurredAt { get; }
}
```

**Handlers potenciales**:
- Crear Payment pendiente
- Enviar email de confirmación
- Actualizar analytics
- Registrar en AuditLog

---

## 🧪 Estrategia de Testing

### Unit Tests
- Validación de RatePlan.CalculateTotalPrice()
- Lógica de RoomInventory.TryReserveRooms()
- Result Pattern success/failure

### Integration Tests
- CreateBooking completo (transacción)
- Concurrencia: 10 threads intentando bookingid simultáneamente
- Idempotencia: mismo request → mismo response

---

## 📊 Límites y Restricciones de Negocio

| Restricción | Valor | Implementación |
|------------|-------|-----------------|
| Máximo noches por reserva | 30 | Validator |
| Capacidad máxima por tipo | N/A | RatePlan.Capacity |
| Inventario no puede ser negativo | 0 | Check constraint SQL |
| Expiración IdempotencyRecord | 24 horas | Cleanup job |
| Máximo huéspedes por reserva | N/A | RoomType.Capacity |

---

## 🔐 Seguridad

- ✅ SQL Injection: Parametrized (EF Core + Dapper)
- ✅ XSRF: Token validado por ASP.NET Core
- ✅ Rate Limiting: Middleware (opcional en Bonus)
- ✅ Concurrencia: RowVersion tokens
- ✅ Transacciones: Unit of Work con rollback automático

---

## 🚢 Deployment

```bash
# Build & Push
docker build -t hotel-booking-api:1.0 -f src/HotelBooking.Api/Dockerfile .
docker-compose up --build

# Todo funciona con:
# - SQL Server 2022 en container
# - API en localhost:5000
# - Frontend en localhost:3000
```

---

**Última actualización**: Marzo 2026
