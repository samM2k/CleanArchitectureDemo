# Clean Architecture Demo — Expense Approval Module

This repository demonstrates a practical implementation of **Clean Architecture** within a modular monolith.  
The goal of this project is to show how a domain-focused system can remain portable, testable, and adaptable while still integrating cleanly into a traditional host application.

The example domain is an **Expense Approval system**, chosen because it is widely understood and contains enough business complexity to justify architectural boundaries without becoming artificially complicated.

---

# Architecture Overview

This solution follows the core principles of Clean Architecture:

- **Dependencies flow inward**
- **Domain is isolated from infrastructure**
- **Application orchestrates business use cases**
- **Infrastructure adapts external concerns**
- **Presentation is replaceable**
- **Host composes everything**

## Solution Structure

```
CleanArchitectureDemo
│
├── Host
│   → Executable entry point (monolith host)
│
└── ExpenseApproval
    ├── Domain
    ├── Application
    ├── Infrastructure
    └── Api
```

---

# Layer Responsibilities

## Domain
The Domain layer contains the core business model and rules.

It is intentionally free of infrastructure concerns such as:

- databases
- frameworks
- serialization
- HTTP
- dependency injection

### Contains:
- Entities (e.g., `Expense`, `ApprovalStep`)
- Value objects
- Domain services
- Business invariants
- Domain exceptions

The domain answers:

> *“Given the rules of the business, is this operation allowed?”*

---

## Application
The Application layer orchestrates use cases and coordinates domain behavior.

It does **not** implement technical details — it describes *what must happen*, not *how* it happens.

### Contains:
- Commands / use case inputs
- Application services
- Policy application
- Repository interfaces

### Why is Company Policy in the Application Layer?

Company expense policies are organizational rules that may evolve independently of the core domain model.  
The domain defines how an expense behaves, while the application determines **which policy is applied at runtime**.

Keeping policies outside the domain prevents policy volatility from destabilizing core business entities.

---

## Infrastructure
Infrastructure implements the technical details required by the application.

Examples:

- EF Core persistence
- Mapping between domain models and persistence stores
- Repository implementations
- Dependency injection registration
- SQLite configuration

### Important Design Decision

The domain is **not shaped to accommodate EF Core**.

Instead, Infrastructure adapts to the domain via:

```
Domain <-> Mapper <-> Store Models <-> EF Core
```

This ensures the domain remains persistence-agnostic and portable.

---

## API (Presentation Adapter)
The API project exposes HTTP endpoints but contains no business logic.

Responsibilities:

- Accept DTOs
- Translate DTOs → Application Commands
- Return HTTP responses
- Surface domain errors appropriately

The API is an adapter — not part of the business core.

It can be replaced by:

- gRPC
- messaging
- CLI
- background jobs
- another web framework

without modifying the domain.

---

## Host (Composition Root)

The Host represents the executable application.

It is responsible for:

- Wiring dependency injection
- Loading module controllers
- Configuring EF providers
- Initializing SQLite
- Enabling Swagger
- Starting the web server

### Why is the Host separate?

This demonstrates how a bounded context can:

✅ integrate into an existing monolith  
✅ remain modular  
✅ later be extracted into a microservice  

without rewriting business logic.

---

# Module Composition

The ExpenseApproval module exposes a single registration method:

```csharp
services.AddExpenseApproval(...)
```

This allows the Host to compose the module without needing to understand its internals.

This pattern supports:

- modular monoliths
- plug-in architectures
- future service extraction

---

# Persistence Strategy

This demo uses:

## SQLite (In-Memory)

Reasons:

- relational behavior
- foreign key enforcement
- realistic SQL translation
- zero setup
- fast startup

The connection is intentionally kept open for the lifetime of the host to preserve the in-memory database.

---

# Error Handling Philosophy

`DomainException` represents **business rule violations**, not technical failures.

Examples:

- Cannot approve an expense out of order
- Alcohol is not reimbursable
- Expense must be submitted before approval

Upper layers can translate these exceptions into appropriate HTTP responses.

---

# Swagger / OpenAPI

Swagger UI is enabled to make the API self-documenting.

XML documentation is generated from DTOs and controllers so request and response schemas are described directly from code.

Launch the project and navigate to:

```
/swagger
```

---

# Example Workflow

Using the provided HTTP file or Swagger:

### 1. Create an expense
```
POST /api/expenses
```

### 2. Submit the expense
```
POST /api/expenses/{id}/submit
```

### 3. Approve
```
POST /api/expenses/{id}/approve
```

### 4. Reject (optional)
```
POST /api/expenses/{id}/reject
```

---

# Architectural Goals Demonstrated

This project intentionally highlights several senior-level architectural practices:

### ✔ Domain purity
The domain has zero knowledge of EF Core or HTTP.

### ✔ Explicit mapping
Infrastructure adapts persistence models instead of leaking them inward.

### ✔ Replaceable adapters
The API is not the application — it is merely one way to interact with it.

### ✔ Modular composition
The host loads the module rather than embedding its logic.

### ✔ Policy separation
Organizational rules live outside the domain to reduce volatility.

### ✔ Client-generated keys
Entities control their identity rather than relying on database generation.

---

# Why Expense Approval?

The domain is:

- familiar
- realistic
- rule-driven
- non-trivial
- easy to reason about

This allows architectural decisions to stand out without domain confusion.

---

# Running the Project

1. Set **Host** as the startup project.
2. Run the application.
3. Navigate to:

```
https://localhost:{port}/swagger
```

---

# Future Enhancements (Not Implemented, but Supported)

This architecture intentionally leaves room for:

- optimistic concurrency
- domain events
- outbox pattern
- background processing
- tenant-specific policies
- service extraction
- read models / CQRS
- integration messaging

None of these require rewriting the domain.

---

# Final Notes

This project is not intended to be a production-ready expense system.

It is designed to demonstrate **architectural clarity**, **layer discipline**, and **realistic modular composition** in a way that mirrors how mature systems evolve over time.

The emphasis is not on framework usage — but on **protecting the domain and controlling dependency flow**.

---
