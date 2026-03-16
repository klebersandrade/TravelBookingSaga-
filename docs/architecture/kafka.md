# Apache Kafka

## Why Kafka?

Kafka was chosen as the message broker for two main reasons:

1. **Learning opportunity**: The developer already has experience with RabbitMQ and Azure ServiceBus. Kafka is an opportunity to expand the skillset.
2. **Technical characteristics**: Kafka offers a persistent distributed log, event replay (ideal for Event Sourcing), and high throughput.

> See [ADR-002](../adr/002-kafka-over-rabbitmq.md) for the full comparison.

## Core Concepts

### Topics

A topic is an **append-only log** where messages are written. Unlike RabbitMQ (where messages are consumed and removed), in Kafka messages **remain** for a configurable period (retention).

### Partitions

Each topic is divided into partitions. Partitions enable:
- **Parallelism**: Multiple consumers can read from different partitions simultaneously
- **Ordering**: Messages within a partition are ordered (FIFO)

In this project, we use `bookingId` as the **partition key** to ensure all messages for the same booking go to the same partition (maintaining order).

### Consumer Groups

Consumers are organized into **consumer groups**. Each partition is assigned to exactly one consumer within the group. This ensures each message is processed by only one consumer in the group.

### Offsets

The offset is a message's position in the partition log. Each consumer group maintains its own offset, allowing:
- Processing messages at its own pace
- Reprocessing messages (by moving the offset backwards)

## Comparison: Kafka vs RabbitMQ vs ServiceBus

| Aspect | RabbitMQ | Azure ServiceBus | Apache Kafka |
|--------|----------|-------------------|-------------|
| Model | Message Queue | Message Queue | Distributed log |
| Message after consumption | Removed | Removed | Retained (retention) |
| Ordering | Not guaranteed* | Within session | Within partition |
| Message replay | No | No | Yes |
| Throughput | Medium | Medium | Very high |
| Operational complexity | Low | Managed (cloud) | High |
| Ideal for | RPC, task queues | Enterprise cloud | Event streaming |

*RabbitMQ with priority queues can offer partial ordering.

## Project Topics

### Naming Convention

Pattern: `{domain}.{entity}.{action}` (lowercase, dot-separated)

### Topic Map

| Topic | Producer | Consumer(s) | Purpose |
|-------|----------|-------------|---------|
| `booking.saga.commands` | Orchestrator | Flight, Car, Hotel, Payment | SAGA commands to participants |
| `booking.saga.replies` | Flight, Car, Hotel, Payment | Orchestrator | Participant replies |
| `flight.reservation.events` | Flight.Service | Notification, Query handlers | Flight domain events |
| `car.reservation.events` | CarRental.Service | Notification | Car rental domain events |
| `hotel.reservation.events` | Hotel.Service | Notification | Hotel domain events |
| `payment.transaction.events` | Payment.Service | Notification | Payment events |
| `booking.deadletter` | Any | Monitoring | Failed messages |

### Partitioning Strategy

| Topic | Partition Key | Reason |
|-------|--------------|--------|
| `booking.saga.commands` | `bookingId` | Ensures command ordering per booking |
| `booking.saga.replies` | `bookingId` | Ensures reply ordering per booking |
| Domain events | Entity ID | Load distribution |

**Number of partitions**: 3 per topic (sufficient for POC, demonstrates the concept).

### Consumer Groups

| Consumer Group | Service | Topic(s) |
|---------------|---------|----------|
| `orchestrator-group` | Booking.Orchestrator | `booking.saga.replies` |
| `flight-group` | Flight.Service | `booking.saga.commands` |
| `car-group` | CarRental.Service | `booking.saga.commands` |
| `hotel-group` | Hotel.Service | `booking.saga.commands` |
| `payment-group` | Payment.Service | `booking.saga.commands` |
| `notification-group` | Notification.Service | All `*.events` |

**Note:** Flight, Car, Hotel, and Payment share the `booking.saga.commands` topic. Each service filters messages by the `messageType` field in the payload and ignores irrelevant ones.

## Confluent.Kafka (chosen library)

We chose to use `Confluent.Kafka` directly instead of MassTransit to maximize learning of Kafka concepts.

> See [ADR-003](../adr/003-confluent-kafka-over-masstransit.md)

### Custom Wrapper

We created a thin abstraction layer in the `TravelBooking.Infrastructure` project:

```
Infrastructure/Kafka/
├── KafkaProducer.cs       # Generic wrapper over IProducer<string, string>
├── KafkaConsumer.cs       # Generic wrapper over IConsumer<string, string>
├── KafkaConfiguration.cs  # Extension methods for DI
└── Serialization/
    └── JsonSerializer.cs  # Message serialization/deserialization
```

This allows:
- Centralized configuration (bootstrap servers, serialization, error handling)
- Easier testing (mock the wrapper, not Confluent.Kafka)
- Simple API for consuming services

## Docker Compose: Kafka KRaft

We use Kafka in **KRaft mode** (without ZooKeeper), which is the future of Kafka (ZooKeeper deprecated since Kafka 3.x):

```yaml
kafka:
  image: confluentinc/cp-kafka:7.6.0
  environment:
    KAFKA_NODE_ID: 1
    KAFKA_PROCESS_ROLES: broker,controller
    KAFKA_CONTROLLER_QUORUM_VOTERS: 1@kafka:29093
    # ... more KRaft configurations
```

### Kafka UI

We include **Provectus Kafka UI** to browse topics, messages, and consumer groups via browser:

```yaml
kafka-ui:
  image: provectuslabs/kafka-ui:latest
  ports:
    - "8080:8080"
```

Access: http://localhost:8080
