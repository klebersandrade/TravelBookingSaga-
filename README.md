# TravelBookingSaga

Travel booking system with **SAGA Pattern**, **Apache Kafka**, **CQRS**, and **Microservices** in **.NET 10**.

## Architecture

```
                        ┌──────────────┐
                        │  API Gateway │  ◄── HTTP ──  👤 User
                        │    (YARP)    │
                        └──────┬───────┘
                               │ HTTP
                 ┌─────────────▼──────────────┐
                 │    Booking Orchestrator     │
                 │    (SAGA State Machine)     │
                 └─────────────┬──────────────┘
                               │
                    ┌──────────┴──────────┐
                    │    Apache Kafka     │
                    │ (commands / replies)│
                    └──┬─────┬─────┬─────┤
                       │     │     │     │
              ┌────────▼┐ ┌─▼──────┐ ┌──▼────────┐ ┌▼──────────┐
              │ Flight  │ │  Car   │ │   Hotel   │ │  Payment  │
              │ Service │ │Rental  │ │  Service  │ │  Service  │
              └────┬────┘ └───┬────┘ └─────┬─────┘ └─────┬─────┘
                   │          │            │              │
                   └──────────┴─────┬──────┴──────────────┘
                                    │ Kafka (events)
                           ┌────────▼─────────┐
                           │  Notification    │
                           │    Service       │
                           └────────┬─────────┘
                                    │
                           ┌────────▼─────────┐
                           │ Firebase Realtime │──► 🌐 Frontend
                           │       DB         │
                           └──────────────────┘

    ┌─────────────────────────────────────────────┐
    │  Infra: PostgreSQL  │  Redis  │  Jaeger     │
    └─────────────────────────────────────────────┘
```

## Stack

| Technology | Purpose |
|------------|---------|
| .NET 10 | Platform |
| Apache Kafka (KRaft) | Messaging |
| PostgreSQL | Database |
| Redis | Cache / Read Store (CQRS) |
| Firebase Realtime DB | Real-time notifications |
| YARP | API Gateway |
| OpenTelemetry + Jaeger | Distributed tracing |
| Docker Compose | Infrastructure |

## Patterns

- **SAGA (Orchestration)** - Distributed transactions with automatic compensation
- **CQRS** - Command and query separation with MediatR
- **Event Sourcing** - In Booking.Orchestrator
- **Database per Service** - Isolated PostgreSQL schemas
- **API Gateway** - Single entry point

## Business Rule

A booking requires **at least 1** item: flight, car, or hotel. The Orchestrator builds the SAGA dynamically.

## Getting Started

```bash
# Start all infrastructure
docker-compose up -d

# Or local build
cd booking_api
dotnet build
```

## Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/v1/bookings` | Start a new booking (SAGA) |
| GET | `/api/v1/bookings/{id}` | Booking status |
| GET | `/api/v1/bookings/{id}/history` | Event history |
| GET | `/api/v1/flights/availability` | Available flights |
| GET | `/api/v1/cars/availability` | Available cars |
| GET | `/api/v1/hotels/availability` | Available hotels |

## Development Tools

| Tool | URL | Purpose |
|------|-----|---------|
| Kafka UI | http://localhost:8080 | Browse topics and messages |
| Jaeger | http://localhost:16686 | Distributed tracing |
| Swagger | http://localhost:5000/swagger | API documentation |

## Documentation

- [Architecture Overview](docs/architecture/overview.md)
- [SAGA Pattern](docs/architecture/saga-pattern.md)
- [CQRS](docs/architecture/cqrs.md)
- [Kafka](docs/architecture/kafka.md)
- [Database](docs/architecture/database.md)
- [ADRs](docs/adr/index.md)

## Project Structure

```
TravelBookingSaga/
├── booking_api/                        # .NET Solution
│   ├── TravelBookingSaga.sln
│   ├── src/
│   │   ├── BuildingBlocks/
│   │   │   ├── TravelBooking.Common/        # Contracts
│   │   │   └── TravelBooking.Infrastructure/ # Kafka, Redis, DB
│   │   ├── Services/
│   │   │   ├── Booking.Orchestrator/        # SAGA state machine
│   │   │   ├── Flight.Service/
│   │   │   ├── CarRental.Service/
│   │   │   ├── Hotel.Service/
│   │   │   ├── Payment.Service/
│   │   │   └── Notification.Service/
│   │   └── Gateway/ApiGateway/
│   └── tests/
│       ├── UnitTests/                       # Unit tests
│       └── IntegrationTests/                # Integration tests (future)
├── docs/                               # Documentation (GitHub Pages)
└── docker/                             # Docker Compose
```
