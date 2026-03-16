# ADR-006: Firebase Realtime Database for Notifications

## Status
Accepted

## Context
The system needs a real-time notification mechanism so that a future frontend can track SAGA progress without polling.

## Options Evaluated

### Option 1: SignalR
.NET library for real-time communication via WebSocket. Server-side, requires its own infrastructure.

### Option 2: Server-Sent Events (SSE)
Unidirectional HTTP protocol (server → client). Simple but limited.

### Option 3: Firebase Realtime Database
Google managed service with SDKs for multiple platforms. Automatic WebSocket.

## Decision
**Firebase Realtime Database** (Spark/free plan).

## Rationale

1. **Generous free tier**: The Spark plan offers 1GB storage, 10GB/month download, and 100 simultaneous connections — more than enough for a POC.

2. **Zero infrastructure**: No WebSocket server to manage. Firebase is serverless.

3. **Multi-platform SDK**: Any frontend (React, Angular, Flutter, React Native) can consume updates using the official Firebase SDK.

4. **Cloud integration in portfolio**: Demonstrates the ability to integrate with cloud services, something valued in interviews.

5. **.NET backend**: The `FirebaseAdmin` package allows the Notification.Service to publish updates directly to Firebase.

## Flow

```
Kafka Event → Notification.Service → Firebase Realtime DB → Frontend (automatic WebSocket)
```

## Consequences

- External service dependency (Google Cloud)
- Requires Firebase Console project creation (initial manual setup)
- No impact on Docker Compose (Firebase is cloud-based)
- If more advanced features are needed in the future, migrating to SignalR is straightforward
