# SAGA Pattern

## What is the SAGA Pattern?

The SAGA pattern is a solution for managing **distributed transactions** in microservice architectures. In a monolith, we use ACID database transactions. In microservices, where each service has its own database, we need an alternative.

A SAGA is a sequence of **local transactions**. Each transaction updates a service and publishes an event/message to trigger the next transaction. If a transaction fails, the SAGA executes **compensating transactions** to undo previous changes.

## Orchestration vs Choreography

### Orchestration (this project's choice)

A central service (the **Orchestrator**) coordinates all participants. It knows the step order, sends commands to each service, and handles responses.

```mermaid
sequenceDiagram
    participant O as Orchestrator
    participant F as Flight Service
    participant C as CarRental Service
    participant H as Hotel Service

    O->>F: ReserveFlightCommand
    F-->>O: FlightReservedReply
    O->>C: ReserveCarCommand
    C-->>O: CarReservedReply
    O->>H: ReserveHotelCommand
    H-->>O: HotelReservedReply
```

**Advantages:**
- Centralized and easy-to-understand flow
- Precise control over execution order
- Compensations managed in a single place
- Simplified debugging and monitoring

**Disadvantages:**
- Risk of the Orchestrator becoming a "God Service"
- Single point of failure (mitigated with replicas)
- Logical coupling between Orchestrator and participants

### Choreography (alternative)

Each service reacts to events and publishes new events. There is no central coordinator.

```mermaid
sequenceDiagram
    participant F as Flight Service
    participant C as CarRental Service
    participant H as Hotel Service

    Note over F: Listens to BookingCreated
    F->>F: Reserves flight
    F->>C: Publishes FlightReserved
    Note over C: Listens to FlightReserved
    C->>C: Reserves car
    C->>H: Publishes CarReserved
    Note over H: Listens to CarReserved
    H->>H: Reserves hotel
```

**Advantages:**
- Full decoupling between services
- No single point of failure
- Each service is autonomous

**Disadvantages:**
- Hard-to-trace flow (spread across N services)
- Complex compensations (each service must know what to undo)
- Risk of circular dependencies
- Difficult to debug

### Why we chose Orchestration

For this project, Orchestration was chosen because:
1. The flow is **visible and documentable** in a single place
2. **Compensations** are controlled centrally and predictably
3. Facilitates **Kafka learning** (clear request/reply pattern)
4. More **didactic** for a portfolio (reviewers understand the flow by reading one file)

> See [ADR-001](../adr/001-saga-orchestration.md) for the full decision.

## Business Rule: Flexible Booking

A booking requires **at least 1** of the 3 reservation services:

| Combination | Valid? |
|-------------|--------|
| Flight only | Yes |
| Car only | Yes |
| Hotel only | Yes |
| Flight + car | Yes |
| Flight + hotel | Yes |
| Car + hotel | Yes |
| Flight + car + hotel | Yes |
| None | No |

The Orchestrator builds the SAGA **dynamically**, including only the steps needed for the requested items. `Payment` and `Notification` are always included.

## Full Flow: Happy Path

### Example: Flight + Car + Hotel

```mermaid
sequenceDiagram
    actor U as User
    participant GW as API Gateway
    participant O as Orchestrator
    participant F as Flight Service
    participant C as CarRental Service
    participant H as Hotel Service
    participant P as Payment Service
    participant N as Notification Service
    participant FB as Firebase

    U->>GW: POST /api/v1/bookings
    GW->>O: CreateBookingCommand

    Note over O: State: STARTED

    O->>F: ReserveFlightCommand (via Kafka)
    F-->>O: FlightReservedReply (via Kafka)
    Note over O: State: FLIGHT_RESERVED

    O->>C: ReserveCarCommand (via Kafka)
    C-->>O: CarReservedReply (via Kafka)
    Note over O: State: CAR_RESERVED

    O->>H: ReserveHotelCommand (via Kafka)
    H-->>O: HotelReservedReply (via Kafka)
    Note over O: State: HOTEL_RESERVED

    O->>P: ProcessPaymentCommand (via Kafka)
    P-->>O: PaymentCompletedReply (via Kafka)
    Note over O: State: PAYMENT_COMPLETED

    O->>F: ConfirmFlightCommand (via Kafka)
    O->>C: ConfirmCarCommand (via Kafka)
    O->>H: ConfirmHotelCommand (via Kafka)

    Note over O: State: BOOKING_COMPLETED

    O->>N: BookingCompletedEvent (via Kafka)
    N->>FB: Updates status in real-time
    FB-->>U: Real-time notification (WebSocket)
```

### Example: Flight Only

```mermaid
sequenceDiagram
    participant O as Orchestrator
    participant F as Flight Service
    participant P as Payment Service
    participant N as Notification Service

    Note over O: State: STARTED (flight only requested)

    O->>F: ReserveFlightCommand
    F-->>O: FlightReservedReply
    Note over O: State: FLIGHT_RESERVED

    O->>P: ProcessPaymentCommand
    P-->>O: PaymentCompletedReply
    Note over O: State: PAYMENT_COMPLETED

    O->>F: ConfirmFlightCommand
    Note over O: State: BOOKING_COMPLETED

    O->>N: BookingCompletedEvent
```

## Compensation Flow

### Example: Hotel fails after Flight and Car reserved

```mermaid
sequenceDiagram
    participant O as Orchestrator
    participant F as Flight Service
    participant C as CarRental Service
    participant H as Hotel Service
    participant N as Notification Service
    participant FB as Firebase

    Note over O: State: STARTED

    O->>F: ReserveFlightCommand
    F-->>O: FlightReservedReply ✅
    Note over O: State: FLIGHT_RESERVED

    O->>C: ReserveCarCommand
    C-->>O: CarReservedReply ✅
    Note over O: State: CAR_RESERVED

    O->>H: ReserveHotelCommand
    H-->>O: HotelReservationFailedReply ❌

    Note over O: State: COMPENSATING

    rect rgb(255, 200, 200)
        Note over O: Compensation (reverse order)
        O->>C: CancelCarCommand
        C-->>O: CarCancelledReply
        O->>F: CancelFlightCommand
        F-->>O: FlightCancelledReply
    end

    Note over O: State: BOOKING_FAILED

    O->>N: BookingFailedEvent
    N->>FB: Updates status (failure)
    FB-->>FB: Frontend receives failure notification
```

### Example: Payment fails (worst case - everything must compensate)

```mermaid
sequenceDiagram
    participant O as Orchestrator
    participant F as Flight Service
    participant C as CarRental Service
    participant H as Hotel Service
    participant P as Payment Service

    O->>F: ReserveFlightCommand → ✅
    O->>C: ReserveCarCommand → ✅
    O->>H: ReserveHotelCommand → ✅
    O->>P: ProcessPaymentCommand → ❌

    Note over O: COMPENSATING

    rect rgb(255, 200, 200)
        O->>H: CancelHotelCommand → ✅
        O->>C: CancelCarCommand → ✅
        O->>F: CancelFlightCommand → ✅
    end

    Note over O: BOOKING_FAILED
```

## SAGA State Machine

```mermaid
stateDiagram-v2
    [*] --> Started: CreateBooking

    Started --> FlightReserving: has flight?
    Started --> CarReserving: no flight, has car?
    Started --> HotelReserving: only hotel?

    FlightReserving --> FlightReserved: success
    FlightReserving --> Compensating: failure

    FlightReserved --> CarReserving: has car?
    FlightReserved --> HotelReserving: no car, has hotel?
    FlightReserved --> Paying: only flight

    CarReserving --> CarReserved: success
    CarReserving --> Compensating: failure

    CarReserved --> HotelReserving: has hotel?
    CarReserved --> Paying: no hotel

    HotelReserving --> HotelReserved: success
    HotelReserving --> Compensating: failure

    HotelReserved --> Paying

    Paying --> Paid: success
    Paying --> Compensating: failure

    Paid --> Confirming
    Confirming --> Completed: all confirmed
    Confirming --> Compensating: failure

    Compensating --> Failed: all compensated

    Completed --> [*]
    Failed --> [*]
```

## Key Concepts

### Idempotency

All handlers must be **idempotent**: processing the same message multiple times must have the same effect as processing it once. This is crucial because Kafka guarantees **at-least-once delivery**.

**Strategy**: Each command carries a `BookingId` + `CorrelationId`. The service checks whether it has already processed that command before executing.

### Timeout and Retry

- Each SAGA step has a **configurable timeout**
- If the Orchestrator doesn't receive a response within the timeout, it can:
  1. Resend the command (retry)
  2. Start compensation (after N retries)

### Compensation Order

Compensations are executed in **reverse order** of completed steps. This ensures we don't try to cancel something that depends on a later step.

### State Persistence

The Orchestrator persists the SAGA state at every transition. If the Orchestrator crashes and restarts, it can resume in-progress SAGAs from where they left off (Event Sourcing).
