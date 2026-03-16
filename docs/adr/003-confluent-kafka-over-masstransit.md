# ADR-003: Confluent.Kafka directly (no MassTransit)

## Status
Accepted

## Context
To integrate with Apache Kafka in .NET, there are two main approaches: using MassTransit (which abstracts the broker) or using Confluent.Kafka directly.

## Options Evaluated

### Option 1: MassTransit
High-level framework that abstracts the message broker. Supports RabbitMQ, Azure ServiceBus, Kafka, and others. Provides SAGA state machine, retry policies, outbox pattern, etc.

### Option 2: Confluent.Kafka (direct)
Official Kafka client for .NET. Low-level API that exposes all Kafka concepts: producers, consumers, offsets, partitions, serialization.

## Decision
**Confluent.Kafka directly**, with a thin custom abstraction layer.

## Rationale

1. **Maximize learning**: MassTransit hides fundamental Kafka concepts (offsets, partitions, consumer groups, rebalancing). Using Confluent.Kafka directly forces deep understanding.

2. **Demonstrate abstraction skills**: Creating a custom wrapper over Confluent.Kafka demonstrates software design ability — creating abstractions without over-engineering.

3. **Knowledge portability**: Concepts learned with Confluent.Kafka apply to any language/platform. MassTransit is .NET-specific.

4. **Transparency**: When troubleshooting, debugging Confluent.Kafka directly is more straightforward than debugging through MassTransit layers.

## Consequences

- More boilerplate code than MassTransit (mitigated by custom wrapper)
- No built-in SAGA state machine from MassTransit (we implement our own — which is precisely the project's point)
- No built-in retry/outbox (we implement manually what's needed)
- Accepted trade-off: more manual work in exchange for deep learning
