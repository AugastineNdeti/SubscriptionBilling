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
I structured the solution into layers to ensure strict boundaries:
- **Domain:** Contains the business logic, entities, and domain events. It has zero dependencies on external libraries.
- **Application:** Handles the use cases (Commands/Queries) and orchestration.
- **Infrastructure:** Implements persistence using EF Core (In-Memory for this challenge).
- **Api:** The entry point using Minimal APIs/Controllers.

### 2. DDD (Domain-Driven Design)
I avoided the "Anemic Domain Model" anti-pattern. Instead of having dumb property bags, I moved business rules inside the Aggregates (e.g., `Invoice.MarkAsPaid()` handles its own validation logic). This ensures the domain is always in a valid state.

### 3. CQRS with MediatR
I used **MediatR** to separate read and write operations. 
- **Commands** (Create/Cancel/Pay) modify state.
- **Queries** (GetInvoices) fetch data without side effects.
This makes the code much easier to maintain and scale as the system grows.

### 4. Domain Events
To satisfy the rule "Activating a subscription generates the first invoice," I implemented **Domain Events**. 
- When a `Subscription` is created, it raises a `SubscriptionActivated` event.
- A handler in the Application layer listens for this event and automatically creates the initial `Invoice`. 
This keeps the Subscription and Invoice logic decoupled.

### 5. Manual Mapping
Per the requirements, I chose **not** to use AutoMapper. I implemented manual mapping in the Query handlers to ensure full control over the DTO shapes and to avoid the overhead/complexity of reflection-based mappers.

---

## Capabilities
This implementation includes:
- **Three aggregates** with enforced business invariants (Customer, Subscription, Invoice)
- **CQRS pattern** via MediatR for clean separation of commands and queries
- **Domain events** for decoupled workflows (SubscriptionActivated, InvoiceGenerated, PaymentReceived)
- **Comprehensive unit tests** covering domain logic and edge cases
- **RESTful API** with Swagger documentation for easy exploration
