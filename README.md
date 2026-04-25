# Journey Mentor - Subscription Billing System

A C# .NET Core implementation demonstrating domain-driven design (DDD) and clean architecture principles for a subscription billing platform.


## How to Run
1. Ensure you have the **.NET 10 SDK** installed.
2. Clone the repository.
3. Open a terminal in the root folder and run:
   ```   dotnet run --project SubscriptionBilling
   ```
4. Once running, navigate to `http://localhost:[YOUR_PORT]/swagger` to interact with the API.
5. To run the tests:
   ```
   dotnet test
   ```

---

## Design Decisions & Architecture

### 1. Clean Architecture
The solution follows Clean Architecture with strict layer separation and minimal cross-layer dependencies:

- **Domain Layer** – Core business logic, aggregates, entities, value objects, and domain events. Zero external dependencies.
- **Application Layer** – Use case orchestration through Commands and Queries (CQRS pattern).
- **Infrastructure Layer** – Data persistence using EF Core with in-memory database.
- **API Layer** – HTTP entrypoint with minimal endpoints.

### 2. Design Patterns
### DDD (Domain-Driven Design)
Avoided the "Anemic Domain Model" anti-pattern. Instead of having dumb property bags, I moved business rules inside the Aggregates (e.g., `Invoice.MarkAsPaid()` handles its own validation logic). This ensures the domain is always in a valid state.

### 3. CQRS with MediatR
-Used **MediatR** to separate read and write operations. 
- **Commands** (Create/Cancel/Pay) modify state.
- **Queries** (GetInvoices) fetch data without side effects.
This makes the code much easier to maintain and scale as the system grows.

### 4. Domain Events
The system publishes domain events to decouple aggregates and enable complex workflows:
- `SubscriptionActivated` – Triggered when a subscription is created
- `InvoiceGenerated` – Raised whenever a new invoice is created
- `PaymentReceived` – Published when an invoice is paid

Event handlersorchestrate cross-aggregate operations. For instance, when `SubscriptionActivated` is published, an Application layer handler automatically generates the first invoice.


### 5. Manual DTO Mapping

Intentionally avoids reflection-based mappers (e.g., AutoMapper) to maintain explicit control over data shape transformations and reduce abstraction overhead.

---
## API Endpoints

- `POST /customers` – Create a new customer
- `POST /subscriptions` – Create and activate a subscription
- `DELETE /subscriptions/{id}` – Cancel a subscription
- `POST /invoices/{id}/pay` – Mark an invoice as paid
- `GET /invoices` – Retrieve invoices with filtering options

- 
## Capabilities
This implementation includes:
- **Three aggregates** with enforced business invariants (Customer, Subscription, Invoice)
- **CQRS pattern** via MediatR for clean separation of commands and queries
- **Domain events** for decoupled workflows (SubscriptionActivated, InvoiceGenerated, PaymentReceived)
- **Comprehensive unit tests** covering domain logic and edge cases
- **RESTful API** with Swagger documentation for easy exploration

# PostMan Documentation: https://documenter.getpostman.com/view/38775037/2sBXqGqgyW
