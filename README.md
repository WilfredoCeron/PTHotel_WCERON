# Hotel Booking Platform

Una plataforma moderna de reservas de hoteles construida con **Clean Architecture**, **CQRS**, y **EF Core + Dapper**.

### Acceder a la aplicación:
- **Frontend**: http://localhost:3000
- **API**: http://localhost:5000
- **Base de Datos**: localhost:14330 (Usuario: sa, Password: StrongPassw0rd!)

## 🎯 Stack Tecnológico

### Backend
- **ASP.NET Core 8** - API REST
- **SQL Server** - Base de datos
- **Entity Framework Core** - Comandos (escritura)
- **Dapper** - Queries (lectura)
- **Serilog** - Logging estructurado
- **MediatR** - CQRS
- **FluentValidation** - Validaciones
- **xUnit** - Tests

### Frontend
- **Next.js 14** - Framework React
- **TypeScript** - Seguridad de tipos
- **TailwindCSS** - Estilos
- **Axios** - Cliente HTTP

### Docker
- **Docker** & **Docker Compose**
- **SQL Server 2022**

---

## 📋 Requisitos Previos

- Docker y Docker Compose instalados
- .NET 8 SDK (para desarrollo local)
- Node.js 18+ (para desarrollo del frontend)

---

## 🚀 Inicio Rápido

### Con Docker Compose (Recomendado)

```bash
docker-compose up --build
```

La aplicación estará disponible en:
- **API**: http://localhost:5000
- **Frontend**: http://localhost:3000
- **SQL Server**: localhost:1433

---

## 🏗️ Arquitectura

```
Domain Layer
    ↓
Application Layer (CQRS)
    ├── Commands (EF Core)
    └── Queries (Dapper)
    ↓
Infrastructure Layer
    ├── EF Core DbContext
    ├── Dapper Repository
    └── Unit of Work
    ↓
API Layer
    └── Controllers + Endpoints
```

### CQRS Pattern

- **Commands**: Operaciones de escritura con EF Core
- **Queries**: Operaciones de lectura con Dapper

### Concurrencia

Control de concurrencia mediante **Concurrency Token (RowVersion)** en reservas críticas.

---

## 📦 Estructura del Proyecto

```
HotelBookingPlatform/
├── src/
│   ├── HotelBooking.Domain/           # Entidades y lógica de negocio
│   ├── HotelBooking.Application/      # Commands, Queries, DTOs
│   ├── HotelBooking.Infrastructure/   # EF Core, Dapper, UnitOfWork
│   ├── HotelBooking.Api/              # Endpoints y configuración
│   └── HotelBooking.Tests/            # Tests unitarios e integración
├── frontend/                           # Aplicación Next.js
├── docker-compose.yml
├── README.md
├── ARCHITECTURE.md
└── postman-collection.json
```

---


## 🔄 Endpoints API

### Hotels
```
GET    /api/v1/hotels
POST   /api/v1/hotels
GET    /api/v1/hotels/{id}
PUT    /api/v1/hotels/{id}
DELETE /api/v1/hotels/{id}
```

### RoomType
```
GET    /api/v1/room-types?hotelId={id}
GET    /api/v1/room-types/{id}
POST   /api/v1/room-types
PUT    /api/v1/room-types/{id}
DELETE /api/v1/room-types/{id}
```

---

## 🔑 Características Principales

✅ **Clean Architecture** - Separación clara de responsabilidades  
✅ **CQRS** - Commands y Queries separados  
✅ **Unit of Work** - Transacciones centralizadas  
✅ **Result Pattern** - Manejo consistente de errores  
✅ **REST Completo** - Versionado + Idempotencia  
✅ **Observabilidad** - Logs estructurados + Trazas  
✅ **Control de Concurrencia** - Anti-overbooking  
✅ **Dockerización** - Reproducible en cualquier entorno  

---

## 📝 Variables de Entorno

Crear archivo `.env` en la raíz:

```env
# SQL Server
MSSQL_SA_PASSWORD=StrongPassw0rd!

# API
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=HotelBookingDb;User Id=sa;Password=StrongPassw0rd!;TrustServerCertificate=true;

# Frontend
NEXT_PUBLIC_API_URL=http://localhost:5000
```

---
