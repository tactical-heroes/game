# Composition Root

## Purpose

This document describes how dependency injection and runtime composition are implemented.

The project uses Dependency Injection to:

* Make dependencies explicit.
* Improve testability.
* Reduce coupling.
* Enable modular architecture.
* Avoid hidden runtime dependencies.

All dependency registration must happen inside the Composition Root.

---

# What Is Composition Root

Composition Root is the place where:

* Concrete implementations are created.
* Interfaces are bound to implementations.
* Feature modules are installed.
* Runtime scopes are created.

The Composition Root is the only place where concrete implementations should be known.

Example:

```text
Application
    ↓
IInventoryRepository

Composition Root
    ↓
InventoryRemoteRepository
```

Application knows only the interface.

Composition Root decides which implementation is used.

---

# Dependency Injection Container

The project uses:

```text
VContainer
```

Responsibilities:

* Constructor injection.
* Lifetime management.
* Scope management.
* Runtime composition.
* Entry points.

Avoid service locators.

Avoid manual dependency resolution.

---

# Root Composition Structure

```text
AppLifetimeScope
    ↓
Foundation Registration
    ↓
Feature Installers
    ↓
Application Startup
```

Bootstrap owns creation of the root scope.

---

# AppLifetimeScope

AppLifetimeScope is the root container.

Responsibilities:

* Create application scope.
* Register foundation services.
* Install feature modules.
* Configure messaging.
* Configure routing.
* Configure application-wide services.

Example:

```text
Bootstrap
    ↓
AppLifetimeScope
```

Exactly one root scope should exist.

---

# Lifetime Hierarchy

The project uses hierarchical scopes.

```text
Application Scope
    ↓
Scene Scope
    ↓
Screen Scope
```

---

# Application Scope

Created during bootstrap.

Contains:

```text
Router

Messaging

Analytics

Audio

Localization

Session

Authentication

Global Services
```

Lifetime:

```text
Application Start
    ↓
Application Exit
```

Application Scope owns global services.

---

# Scene Scope

Optional.

Created for complex scenes.

Examples:

```text
Battle

WorldMap

Tutorial
```

Contains:

```text
Scene Services

Scene Factories

Scene Controllers
```

Lifetime:

```text
Scene Load
    ↓
Scene Unload
```

---

# Screen Scope

Optional.

Created for large UI screens.

Examples:

```text
Inventory

Shop

Profile

BattlePass
```

Contains:

```text
ViewModel

Screen State

Screen Commands
```

Lifetime:

```text
Screen Open
    ↓
Screen Close
```

---

# Dependency Direction

Dependencies are created outward.

Example:

```text
ViewModel
    ↓
UseCase
    ↓
Repository Interface
```

Composition Root creates:

```text
InventoryViewModel
    ↓
EquipItemUseCase
    ↓
InventoryRepository
```

Application never creates dependencies manually.

---

# Feature Installation

Every feature provides an installer.

Example:

```text
InventoryModuleInstaller

ShopModuleInstaller

ProfileModuleInstaller
```

Responsibilities:

* Register Use Cases.
* Register Repositories.
* Register Event Handlers.
* Register Screen Factories.
* Register Services.

Feature installers own feature composition.

---

# Installer Rule

Installers may reference:

```text
Contracts

Domain

Application

Infrastructure

Presentation
```

Installers are allowed to know everything.

Nothing else should know everything.

---

# Dependency Registration Example

Example:

```text
Application:
    IInventoryRepository

Infrastructure:
    InventoryRemoteRepository
```

Composition Root:

```text
IInventoryRepository
    ↓
InventoryRemoteRepository
```

Application remains infrastructure-agnostic.

---

# Constructor Injection

Use constructor injection whenever possible.

Preferred:

```text
InventoryViewModel
(
    EquipItemUseCase equipItemUseCase
)
```

Avoid:

```text
Service Locator

Static Resolve()

Global Container Access
```

Dependencies should be visible.

---

# Field Injection

Avoid field injection.

Bad:

```text
[SerializeField]
private InventoryRepository repository;
```

Business dependencies should not be hidden.

Use constructor injection.

---

# Method Injection

Allowed only when:

* Dependency is optional.
* Dependency changes during execution.
* Runtime payloads are passed.

Use sparingly.

---

# Service Locator Rule

Forbidden:

```text
Container.Resolve<T>()

Global.Resolve<T>()

ServiceLocator.Resolve<T>()
```

outside Composition Root.

Reason:

* Hidden dependencies.
* Harder testing.
* Harder maintenance.

---

# Singleton Rule

Do not create manual singletons.

Forbidden:

```text
Instance

GlobalManager

DontDestroyOnLoad Singleton
```

Use DI lifetimes instead.

---

# Lifetimes

Preferred lifetimes:

```text
Singleton

Scoped

Transient
```

---

# Singleton Lifetime

Use for:

```text
Router

Messaging

Localization

Analytics

Audio

Session
```

Singletons belong to Application Scope.

---

# Scoped Lifetime

Use for:

```text
Battle Services

Scene Services

Screen State

ViewModels
```

Scoped lifetime is preferred over Singleton whenever possible.

---

# Transient Lifetime

Use for:

```text
Factories

Mappers

Commands

Short-lived Objects
```

Use sparingly.

---

# Messaging

Messaging is registered centrally.

Recommended:

```text
MessagePipe
```

Registered in AppLifetimeScope.

Features may register:

```text
Publishers

Subscribers

Request Handlers
```

through installers.

---

# Routing

Router is registered in Application Scope.

Responsibilities:

```text
Open Screen

Close Screen

Navigate

Pass Payload
```

There should be exactly one application router.

---

# Factories

Factories are preferred over direct instantiation.

Good:

```text
InventoryScreenFactory
```

Bad:

```text
new InventoryScreen()
```

inside business logic.

Factories belong to Presentation or Infrastructure.

---

# Feature Boundaries

Composition Root must respect feature boundaries.

Allowed:

```text
Inventory Installer
    ↓
Inventory Components
```

Forbidden:

```text
Inventory Installer
    ↓
Shop Internals
```

Cross-feature dependencies should go through Contracts.

---

# Application Startup

Startup sequence:

```text
Create Root Scope
    ↓
Register Foundation
    ↓
Install Features
    ↓
Initialize Infrastructure
    ↓
Initialize Messaging
    ↓
Initialize Routing
    ↓
Resolve Startup Route
    ↓
Open Initial Screen
```

The order should remain deterministic.

---

# Testing

Composition should be testable.

Recommended tests:

```text
Installer Tests

Container Build Tests

Startup Tests

Module Registration Tests
```

Every installer should be verifiable in isolation.

---

# Review Checklist

Before registering a dependency ask:

1. Is the dependency owned by this feature?
2. Is the lifetime correct?
3. Can constructor injection be used?
4. Is there a hidden dependency?
5. Can this be scoped instead of singleton?
6. Does this violate feature boundaries?

If any answer is no:

Reconsider the design.

---

# Summary

Composition Root owns dependency creation.

Use this rule:

```text
Application defines abstractions.

Infrastructure provides implementations.

Composition Root wires everything together.

No code outside Composition Root should know how dependencies are constructed.
```
