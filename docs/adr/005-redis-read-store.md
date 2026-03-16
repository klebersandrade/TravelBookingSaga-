# ADR-005: Redis as Read Store (CQRS)

## Status
Accepted

## Context
The CQRS pattern suggests separating read and write models. We need a read store for the Booking.Orchestrator that allows fast queries on SAGA status.

## Options Evaluated

### Option 1: Same PostgreSQL (denormalized view/table)
Create a materialized view or table in the same database for queries.

### Option 2: MongoDB
Document-oriented NoSQL database as read store.

### Option 3: Redis
In-memory data store with support for complex data structures.

## Decision
**Redis** as read store in the Booking.Orchestrator.

## Rationale

1. **New learning**: The developer has never used Redis. Like Kafka, it's an opportunity to expand the skillset.

2. **Performance**: Microsecond responses for SAGA status queries. Ideal for status polling scenarios.

3. **Simplicity**: Key-value with automatic TTL. A completed SAGA's status can naturally expire.

4. **Genuine heterogeneous CQRS**: Demonstrates the CQRS concept with different databases for writing (PostgreSQL) and reading (Redis), which is the most common real-world production scenario.

5. **Mature package**: `StackExchange.Redis` is the most widely used .NET client with excellent documentation.

## Consequences

- Eventual consistency between PostgreSQL (event store) and Redis (projection)
- Redis is volatile by default — if restarted, projections are lost (rebuildable via event replay)
- One more container in Docker Compose
