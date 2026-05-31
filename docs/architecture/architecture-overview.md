## Purpose

This document describes the high-level architecture of the project.

The goal of the architecture is to provide:

* Clear ownership boundaries.
* Low coupling between game features.
* High testability.
* Scalability for long-term development.
* Fast onboarding for new developers.
* Explicit dependency direction.
* Ability to evolve individual features independently.

---

# Architectural Style

The project follows a combination of:

* Modular Monolith
* Feature-First Structure
* Clean Architecture
* Domain-Driven Design (where complexity justifies it)
* MVVM for UI Toolkit
* Dependency Injection
* Event-Driven Communication between features

The application is deployed as a single Unity client.

Internally it is divided into isolated feature modules.

---

# High-Level Structure

```text
Application
│
├── Bootstrap
│
├── Foundation
│
└── Features
    │
    ├── Inventory
    ├── Shop
    ├── Profile
    ├── Matchmaking
    ├── Battle
    └── ...
```

Each feature owns:

* UI
* Application logic
* Business logic
* Infrastructure adapters
* Dependency registration

Features should remain independent whenever possible.

---

# Architectural Layers

Every feature may contain the following layers:

```text
Presentation
    ↓
Application
    ↓
Domain

Infrastructure
    ↑
```

---

## Presentation

Responsible for user interaction.

Contains:

* UI Toolkit screens
* UXML
* USS
* ViewModels
* Presenters
* Navigation adapters
* Screen factories

Presentation never contains business rules.

Presentation communicates with Application.

---

## Application

Responsible for orchestration.

Contains:

* Use Cases
* Commands
* Queries
* Coordinators
* Application Services
* Ports (Interfaces)

Application coordinates business scenarios.

Application communicates with Domain through domain models.

Application communicates with Infrastructure through ports.

---

## Domain

Responsible for business rules.

Contains:

* Entities
* Aggregates
* Value Objects
* Domain Services
* Domain Events
* Invariants
* Validation Rules

Domain must not depend on:

* Unity
* UI Toolkit
* Addressables
* Networking
* Save systems
* Dependency Injection frameworks

Domain should be testable as pure C#.

---

## Infrastructure

Responsible for external integrations.

Contains:

* Backend API clients
* Save systems
* Addressables providers
* Analytics adapters
* Audio adapters
* Repository implementations

Infrastructure implements interfaces defined by Application.

Infrastructure must not contain business rules.

---

# Modular Monolith

The project is implemented as a modular monolith.

A modular monolith means:

* One executable application.
* One Unity client.
* One deployment artifact.
* Multiple isolated feature modules.

Features communicate through explicit contracts.

Features should not directly depend on internal implementation details of other features.

For details see:

docs/architecture/modular-monolith.md

---

# Dependency Injection

The application uses dependency injection.

Goals:

* Explicit dependencies.
* Easier testing.
* Better modularity.
* Runtime composition.

The composition root is located in Bootstrap.

Features register their own dependencies through module installers.

For details see:

docs/architecture/composition-root.md

---

# Event Communication

Features may communicate using application events.

Events should represent facts that already happened.

Examples:

* InventoryItemEquippedEvent
* QuestCompletedEvent
* MatchFoundEvent

Events must not be used as a replacement for normal dependencies.

For details see:

docs/conventions/events.md

---

# User Interface

The project uses:

* UI Toolkit
* MVVM

UI Toolkit is considered part of Presentation.

ViewModels belong to Presentation.

ViewModels communicate with Application Use Cases.

For details see:

docs/unity/ui-toolkit.md

docs/unity/mvvm.md

---

# Project Structure

The project is organized around features.

Runtime code is stored inside local UPM packages.

Unity-authored content is stored in Assets.

For details see:

docs/conventions/folder-structure.md

docs/conventions/packages.md

---

# Testing Strategy

The project uses multiple test levels:

* Domain Tests
* Application Tests
* Integration Tests
* Runtime Tests
* UI Tests

Domain and Application layers should be testable without Unity runtime.

For details see:

docs/conventions/testing.md

---

# Architectural Principles

When adding new code:

1. Prefer feature ownership over global ownership.
2. Keep dependency direction explicit.
3. Avoid hidden coupling.
4. Prefer composition over inheritance.
5. Keep Domain independent from Unity.
6. Keep Infrastructure independent from UI.
7. Use events only for cross-feature notifications.
8. Keep shared code minimal.
9. Favor testability over convenience.
10. Optimize for maintainability rather than short-term speed.
