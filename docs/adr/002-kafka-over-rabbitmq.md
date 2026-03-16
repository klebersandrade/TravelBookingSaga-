# ADR-002: Kafka as Message Broker

## Status
Accepted

## Context
The system needs a message broker for asynchronous inter-service communication. The developer already has experience with RabbitMQ and Azure ServiceBus.

## Options Evaluated

### Option 1: RabbitMQ
Already known broker with push-based model and traditional exchanges/queues.

### Option 2: Azure ServiceBus
Azure managed service, already used in previous projects.

### Option 3: Apache Kafka
Distributed log with pull-based model, event replay, and high throughput.

## Decision
**Apache Kafka.**

## Rationale

1. **Learning objective (PDI)**: Expand skillset with a widely demanded technology. Kafka is one of the most sought-after competencies for microservice architectures and data streaming.

2. **Event Sourcing**: Kafka works as a natural append-only log. Messages remain in the topic for a configurable period, enabling replay — ideal for reconstructing SAGA state.

3. **Consumer Groups**: Multiple consumers can read from the same topic independently, each at their own pace. The Notification.Service can consume events without interfering with the SAGA flow.

4. **Portfolio differentiator**: Demonstrating Kafka proficiency significantly enhances the portfolio, being a highly valued skill in microservice architectures.

## Consequences

- Higher operational complexity than RabbitMQ (mitigated by Docker Compose)
- Learning curve: partition, offset, and consumer group concepts
- No native message routing like RabbitMQ exchanges (filter by messageType in consumer)
