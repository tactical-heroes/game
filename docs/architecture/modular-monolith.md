# Modular Monolith

## Purpose

This document describes how the project is divided into modules.

The project is implemented as a modular monolith.

The goal is to achieve:

* Strong feature ownership.
* Explicit dependencies.
* Low coupling.
* High maintainability.
* Easy testing.
* Ability to evolve features independently.

---

# What Is A Modular Monolith

The application is deployed as a single Unity client.

There is:

* One executable application.
* One deployment artifact.
* One runtime process.

Internally the application is divided into isolated modules.

Each module represents a business capability.

Examples:

```text
Inventory
Shop
Profile
Matchmaking
Battle
Quests
Settings
Friends
Guilds
```

A module owns everything related to its business capability.

---

# Module Structure

Each module is implemented as a dedicated package.

Example:

```text
Packages/
└── com.company.game.feature.inventory/
```

Every module contains:

```text
Runtime/
├── Contracts/
├── Domain/
├── Application/
├── Infrastructure/
├── Presentation/
└── Composition/
```

The module is the primary ownership boundary.

---

# Module Responsibilities

A module owns:

* UI
* Use Cases
* Business Rules
* Persistence Adapters
* Backend Adapters
* Events
* Dependency Registration

Whenever possible functionality should remain inside the module that owns it.

---

# Feature Ownership

Ownership is more important than technical concerns.

Bad:

```text
UI/
Application/
Domain/
Infrastructure/
```

Developers must navigate multiple global folders to understand a feature.

Good:

```text
Inventory/
Shop/
Profile/
Battle/
```

All code related to a feature remains close together.

---

# Internal Module Layers

Every module contains the following layers:

```text
Presentation
    ↓
Application
    ↓
Domain

Infrastructure
    ↑
```

The layers exist inside the module.

The layers do not exist globally.

---

# Contracts Layer

The Contracts layer contains the public API of the module.

Example:

```text
Contracts/
├── Events/
├── Dtos/
├── Requests/
└── Responses/
```

Contracts are the only layer that other modules may depend on.

Purpose:

* Public DTOs.
* Public Events.
* Public Requests.
* Public Read Models.

Contracts should remain stable.

---

# Domain Layer

The Domain layer contains business rules.

Examples:

```text
Inventory
InventorySlot
Item
ItemStack
Currency
Price
```

Domain contains:

* Entities.
* Aggregates.
* Value Objects.
* Domain Services.
* Domain Events.

Domain should be pure C#.

Domain must not reference Unity APIs.

---

# Application Layer

The Application layer contains use cases.

Examples:

```text
GetInventoryItemsUseCase
EquipItemUseCase
PurchaseItemUseCase
```

Application contains:

* Commands.
* Queries.
* Use Cases.
* Coordinators.
* Ports.

Application orchestrates business scenarios.

Application depends on Domain.

---

# Infrastructure Layer

Infrastructure contains external adapters.

Examples:

```text
InventoryApiClient
InventoryRepository
InventoryLocalCache
AddressablesInventoryProvider
```

Infrastructure implements ports defined by Application.

Infrastructure communicates with:

* Backend.
* Save Systems.
* Addressables.
* Analytics.
* Audio.
* External SDKs.

Infrastructure does not contain business rules.

---

# Presentation Layer

Presentation contains UI.

Examples:

```text
UXML
USS
View
ViewModel
ScreenFactory
Navigation
```

Presentation communicates with Application.

Presentation never communicates directly with Infrastructure.

Presentation does not contain business rules.

---

# Composition Layer

Composition contains dependency registration.

Example:

```text
InventoryModuleInstaller
```

Responsibilities:

* Register Use Cases.
* Register Infrastructure.
* Register Event Handlers.
* Register ViewModels.
* Register Factories.

Composition may reference all layers of the same module.

---

# Foundation Package

The project contains a Foundation package.

Example:

```text
com.company.game.foundation
```

Foundation contains only shared abstractions.

Examples:

```text
Result
Entity
ValueObject
IClock
IGuidProvider
Navigation Contracts
Messaging Contracts
```

Foundation is not a dumping ground.

Code should only move to Foundation when:

1. It is shared by multiple modules.
2. It is generic.
3. It has no business ownership.

---

# Shared Code Rules

Before moving code into Foundation ask:

```text
Does this belong to a specific feature?
```

If the answer is yes:

Keep it in the feature.

If the answer is no:

Consider moving it to Foundation.

Prefer duplication over premature sharing.

---

# Creating New Modules

Create a new module when:

* A new business capability appears.
* Ownership becomes unclear.
* Existing modules become too large.
* A new team may eventually own the feature.

Do not create modules for technical reasons.

Bad examples:

```text
NetworkModule
DatabaseModule
UtilsModule
ManagersModule
```

Good examples:

```text
Inventory
Shop
Profile
Battle
Matchmaking
Guilds
```

Modules should represent business capabilities.

---

# Cross-Module Communication

Modules communicate using Contracts.

Preferred mechanisms:

* Public Events.
* Public DTOs.
* Public Requests.
* Public Feature APIs.

Example:

```text
Inventory.Contracts
    ↓
Achievements.Application
```

Allowed:

```text
Inventory.Contracts
```

Forbidden:

```text
Inventory.Domain
Inventory.Application
Inventory.Infrastructure
Inventory.Presentation
```

No module may reference another module's internals.

---

# Event Communication

Modules may publish public events.

Example:

```text
InventoryItemEquippedEvent
PurchaseCompletedEvent
QuestCompletedEvent
```

Events should represent facts.

Events should not represent user intentions.

Good:

```text
PurchaseCompletedEvent
```

Bad:

```text
BuyButtonClickedEvent
```

---

# Dependency Direction

Allowed:

```text
Presentation → Application

Application → Domain

Infrastructure → Application

Infrastructure → Domain

Bootstrap → All Modules
```

Forbidden:

```text
Domain → Infrastructure

Domain → Presentation

Application → Presentation

Feature A Internals → Feature B Internals
```

---

# Example Module

```text
com.company.game.feature.inventory/

Runtime/
├── Contracts/
│   ├── Events/
│   │   └── InventoryItemEquippedEvent.cs
│   │
│   └── Dtos/
│       └── InventoryItemDto.cs
│
├── Domain/
│   ├── Inventory.cs
│   ├── Item.cs
│   └── InventoryRules.cs
│
├── Application/
│   ├── Ports/
│   │   └── IInventoryRepository.cs
│   │
│   └── UseCases/
│       └── EquipItem/
│           └── EquipItemUseCase.cs
│
├── Infrastructure/
│   └── Repositories/
│       └── InventoryRepository.cs
│
├── Presentation/
│   └── UiToolkit/
│       └── Screens/
│           └── InventoryScreen/
│
└── Composition/
    └── InventoryModuleInstaller.cs
```

This is the recommended structure for all business features.

---

# Guiding Principle

When adding new code ask:

```text
Which business capability owns this?
```

The answer determines the module.

Ownership first.

Architecture second.
