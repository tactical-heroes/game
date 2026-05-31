# Events

## Purpose

This document defines how events and messaging are used in the project.

The project uses events for decoupled communication between modules.

Events must not replace explicit dependencies, use cases, or navigation.

---

# Core Rule

Events should represent facts that already happened.

Good:

```text
InventoryItemEquippedEvent

PurchaseCompletedEvent

QuestCompletedEvent
```

Bad:

```text
EquipButtonClickedEvent

OpenShopEvent

ChangeLabelTextEvent
```

Events are facts.

Not commands.

---

# Event Types

The project uses three event categories:

```text
Domain Events

Application Events

Integration / Contract Events
```

---

# Domain Events

Domain Events belong to Domain.

Purpose:

* Represent important business facts inside a feature.
* Keep domain logic explicit.
* Allow Application layer to react to domain changes.

Examples:

```text
ItemEquippedDomainEvent

QuestCompletedDomainEvent

PurchaseValidatedDomainEvent
```

Rules:

* Domain Events stay inside the owning feature.
* Domain Events are not public cross-feature contracts by default.
* Domain Events must not depend on MessagePipe.
* Domain entities must not publish events directly to a global bus.

Domain may create events.

Application decides how to handle or publish them.

---

# Application Events

Application Events belong to Application or Contracts.

Purpose:

* Notify other parts of the application about completed use cases.
* Trigger side effects after successful state changes.

Examples:

```text
InventoryChangedEvent

PlayerProfileUpdatedEvent

MatchFoundEvent
```

Application Events may be published through the project messaging system.

---

# Contract Events

Contract Events are public cross-feature events.

They live in:

```text
Runtime/Contracts/Events/
```

Example:

```text
Inventory.Contracts.Events.InventoryItemEquippedEvent
```

Other features may subscribe to Contract Events.

Contract Events are part of the feature public API.

Keep them stable.

---

# Event Location

Use this rule:

```text
Internal business fact
    → Domain/Events

Feature-level application fact
    → Application/Events

Cross-feature public fact
    → Contracts/Events
```

---

# Messaging Library

The project uses:

```text
MessagePipe
```

for application-level messaging.

Messaging is registered in Composition Root.

Feature modules register their own publishers, subscribers, and handlers.

---

# When To Use Events

Use events when:

* Multiple independent listeners may react.
* Sender does not need a result.
* Sender should not know receivers.
* Event represents a completed fact.
* Cross-feature communication is needed.

Good example:

```text
Inventory publishes InventoryItemEquippedEvent

Achievements listens and updates progress

Analytics listens and records event

UI listens and shows toast
```

---

# When NOT To Use Events

Do not use events when:

* A result is required.
* There is exactly one known dependency.
* The operation is a command.
* The operation is navigation.
* The event represents a button click.
* The event is high-frequency.
* Order of execution is critical.

Use explicit dependencies instead.

---

# Commands vs Events

Command:

```text
EquipItem
```

means:

```text
Please do this.
```

Event:

```text
ItemEquipped
```

means:

```text
This already happened.
```

Commands go to UseCases.

Events go to subscribers.

---

# Query vs Event

Query:

```text
GetInventoryItems
```

requires a result.

Event:

```text
InventoryChanged
```

does not require a result.

Do not use events for request/response operations.

---

# Navigation vs Event

Navigation should use Router.

Good:

```text
ViewModel
    ↓
Router.Open(Profile)
```

Bad:

```text
Publish(OpenProfileEvent)
```

Router owns navigation.

Events do not replace Router.

---

# UI Events

UI events should remain local.

Good:

```text
Button clicked
    ↓
View
    ↓
ViewModel
    ↓
UseCase
```

Bad:

```text
Button clicked
    ↓
Global Event Bus
```

Do not publish UI interactions globally.

Publish only completed application facts.

---

# Event Naming

Use past tense.

Good:

```text
InventoryItemEquippedEvent

PurchaseCompletedEvent

ProfileUpdatedEvent

QuestCompletedEvent
```

Bad:

```text
EquipInventoryItemEvent

CompletePurchaseEvent

UpdateProfileEvent

OpenShopEvent
```

Events describe what happened.

---

# Event Payload

Event payloads should be:

* Immutable.
* Minimal.
* Stable.
* Serializable when useful.

Good payload:

```text
ItemId

PlayerId

SlotId
```

Bad payload:

```text
InventoryRepository

VisualElement

MonoBehaviour

GameObject
```

Events must not carry infrastructure or UI objects.

---

# Event Dependencies

Events may depend on:

```text
Primitive values

Value objects

Stable identifiers

DTOs from Contracts
```

Events must not depend on:

```text
ViewModel

View

MonoBehaviour

Repository

Service implementation

Scene object

VisualElement
```

---

# Publishing Events

Events should usually be published by:

```text
Application UseCases
```

after successful operations.

Example:

```text
EquipItemUseCase
    ↓
Repository Save
    ↓
Publish InventoryItemEquippedEvent
```

Do not publish success events before the operation succeeds.

---

# Subscribing To Events

Subscribers should live in:

```text
Application

Infrastructure

Presentation
```

depending on their responsibility.

Examples:

```text
Achievements.Application
    reacts to InventoryItemEquippedEvent

Analytics.Infrastructure
    sends analytics

Inventory.Presentation
    shows toast
```

---

# Subscriber Responsibilities

Application subscriber:

```text
Updates application state

Triggers another use case

Updates progression
```

Infrastructure subscriber:

```text
Sends analytics

Writes logs

Synchronizes external services
```

Presentation subscriber:

```text
Refreshes UI

Shows notification

Updates local screen state
```

---

# Domain Event Handling

Domain Events should be collected by Application.

Example flow:

```text
Aggregate
    ↓
Creates Domain Event
    ↓
UseCase
    ↓
Handles Domain Event
    ↓
Publishes Contract Event if needed
```

Domain itself should not publish to MessagePipe.

---

# Event Bus Scope

Messaging belongs to Application Scope by default.

Screen-local events should not use global MessagePipe.

For screen-local interaction use:

```text
ViewModel methods

C# events

Bindable state

Local callbacks
```

---

# High-Frequency Events

Do not publish high-frequency gameplay data globally.

Bad examples:

```text
PlayerPositionChangedEvent every frame

HealthChangedEvent every frame

MouseMovedEvent

DragMovedEvent
```

For high-frequency data use:

```text
Direct references

Local observers

ECS events

Reactive properties

Specialized gameplay systems
```

---

# Event Ordering

Do not depend on subscriber execution order.

If order matters, use an explicit orchestrator.

Bad:

```text
Subscriber A must run before Subscriber B
```

Good:

```text
UseCase orchestrates required order directly
```

Events are for decoupling.

Not deterministic workflows.

---

# Error Handling

Event subscribers should handle their own failures.

A subscriber failure should not silently corrupt application state.

Critical workflows should not be implemented as event chains.

Use explicit use case orchestration for critical workflows.

---

# MessagePipe Registration

Publishers and subscribers are registered in feature installers.

Example:

```text
InventoryModuleInstaller
    registers InventoryItemEquippedEvent publisher

AchievementsModuleInstaller
    registers InventoryItemEquippedEvent subscriber
```

The Composition Root configures MessagePipe globally.

---

# Cross-Feature Example

```text
Inventory.Application
    publishes InventoryItemEquippedEvent

Achievements.Application
    subscribes

Analytics.Infrastructure
    subscribes

Presentation
    shows toast
```

The Inventory feature does not know about Achievements or Analytics.

---

# Forbidden Patterns

Avoid:

```text
Global Event For Every Action

ButtonClicked Global Events

Request/Response Over Event Bus

Event Chains For Critical Logic

Events Carrying GameObjects

Events Carrying Repositories

Events Depending On UI Toolkit
```

---

# Review Checklist

Before creating an event ask:

1. Is this a fact that already happened?
2. Can there be multiple independent listeners?
3. Does the sender not need a result?
4. Is the payload minimal?
5. Is this not a UI-only action?
6. Is this not navigation?
7. Is this not high-frequency?
8. Is this event in the correct layer?

If any answer is no:

Use a UseCase, Router, direct dependency, or local callback instead.

---

# Summary

Use events for decoupled facts.

Use this rule:

```text
Commands ask.

Queries return.

Events announce.

Router navigates.
```

Events are powerful.

Use them carefully.
