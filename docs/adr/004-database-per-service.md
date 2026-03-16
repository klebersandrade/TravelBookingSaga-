# ADR-004: Database per Service with PostgreSQL

## Status
Accepted

## Context
In a microservice architecture, each service needs its own persistence to maintain autonomy and isolation.

## Options Evaluated

### Option 1: Shared database
All services access the same database and tables.

### Option 2: Database per Service (separate instances)
Each service has its own database instance.

### Option 3: Database per Service (separate schemas)
Same instance but isolated schemas per service.

## Decision
**Option 3 for POC** (separate schemas on the same PostgreSQL instance), documenting that in production it would be Option 2.

**PostgreSQL** as the engine for all services.

## Rationale

### Separate schemas (POC)
- Simplifies Docker Compose (one PostgreSQL instance)
- Maintains logical isolation between services
- Demonstrates the concept without the operational complexity of multiple instances

### PostgreSQL
- ACID compliance (essential for financial transactions in Payment.Service)
- Native JSONB support (ideal for the Orchestrator's event store)
- Widely used and free
- Excellent EF Core support (Npgsql)

## Consequences

- In the POC, a PostgreSQL crash affects all services (acceptable for development)
- Clearly document that in production each service would have its own database
- Initialization scripts create schemas automatically
