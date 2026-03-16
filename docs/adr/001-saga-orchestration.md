# ADR-001: SAGA Orchestration

## Status
Accepted

## Context
The system needs to coordinate reservations across multiple services (flight, car, hotel, payment) with eventual consistency guarantees. If any step fails, previous steps must be undone (compensation).

Two approaches exist for implementing the SAGA pattern: **Orchestration** and **Choreography**.

## Options Evaluated

### Option 1: Orchestration
A central service (Orchestrator) coordinates all participants, sending commands and receiving responses.

### Option 2: Choreography
Each service reacts to events and publishes new events. There is no central coordinator.

## Decision
**Orchestration.**

## Rationale

1. **Visibility**: The entire flow is visible in a single place (the Orchestrator). For a portfolio, this is essential — reviewers can understand the flow by reading one file.

2. **Compensation control**: The Orchestrator knows exactly which step failed and which compensations to trigger, in the correct reverse order.

3. **Flexible booking**: Since a booking can have varying item combinations (flight only, flight+hotel, etc.), the Orchestrator can build the SAGA dynamically. With choreography, each service would need to know about all possible combinations.

4. **Kafka learning**: The Orchestrator clearly exercises the request/reply pattern via Kafka (publishes commands, consumes responses).

5. **Debugging**: With choreography, tracing a booking flow requires looking at logs from N services. With orchestration, the Orchestrator has the complete history.

## Consequences

- The Orchestrator is a single point of coordination (not necessarily of failure if it has replicas)
- Logical coupling: the Orchestrator knows the flow of all participants
- Mitigation: keep the Orchestrator lean, delegating business rules to participant services
