# TravelBookingSaga

## Travel Booking System with SAGA Pattern

This project demonstrates the implementation of the **SAGA pattern** in a distributed travel booking system, using **microservices**, **Apache Kafka**, **CQRS**, and **Event Sourcing**.

### What does this system do?

It allows users to create a travel booking that may include **at least one** of the following items:

- Flight tickets
- Car rental
- Hotel accommodation

The system orchestrates all reservations in a coordinated manner. If any step fails, all previous steps are **automatically compensated** (rollback), ensuring **eventual consistency** in a distributed environment.

### Technology Stack

| Technology | Purpose |
|------------|---------|
| .NET 10 | Main platform |
| Apache Kafka | Messaging and event streaming |
| PostgreSQL | Database (write store) |
| Redis | Cache and read store (CQRS) |
| Firebase Realtime DB | Real-time notifications |
| Docker Compose | Local infrastructure |
| YARP | API Gateway |
| OpenTelemetry + Jaeger | Distributed tracing |

### Navigation

- [Architecture Overview](architecture/overview.md)
- [SAGA Pattern](architecture/saga-pattern.md)
- [CQRS and Event Sourcing](architecture/cqrs.md)
- [Apache Kafka](architecture/kafka.md)
- [Database Strategy](architecture/database.md)
- [ADRs - Architecture Decision Records](adr/index.md)
- [Kafka Learning Journal](learning-journal/kafka-concepts.md)

### Getting Started

```bash
docker-compose up -d
```

Access:
- **API Gateway**: http://localhost:5000
- **Kafka UI**: http://localhost:8080
- **Jaeger**: http://localhost:16686
- **Swagger**: http://localhost:5000/swagger
