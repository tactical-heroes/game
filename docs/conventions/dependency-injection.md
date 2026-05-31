# Dependency Injection

## Purpose

This document defines dependency injection rules used in the project.

The project uses Dependency Injection to make dependencies explicit, testable, and replaceable.

DI is not a replacement for architecture.

DI only wires architecture together.

---

# DI Container

The project uses:

```text
VContainer
```

VContainer is the standard dependency injection container for runtime code.

Do not introduce another DI container without architectural approval.

---

# Core Rule

Dependencies must be explicit.

Preferred:

```text
Constructor Injection
```

Forbidden:

```text
Service Locator

Global Resolve

Static Container Access
```

---

# Composition Location

Dependency registration belongs to:

```text
Bootstrap

Feature Composition

Scene Scope

Screen Scope
```

Do not register dependencies inside:

```text
Domain

UseCase

ViewModel

Repository

View
```

Objects use dependencies.

Composition creates dependencies.

---

# Constructor Injection

Constructor injection is the default.

Good:

```csharp
public sealed class InventoryViewModel
{
    private readonly EquipItemUseCase equipItemUseCase;

    public InventoryViewModel(EquipItemUseCase equipItemUseCase)
    {
        this.equipItemUseCase = equipItemUseCase;
    }
}
```

Bad:

```csharp
public sealed class InventoryViewModel
{
    public void Load()
    {
        EquipItemUseCase useCase = ServiceLocator.Resolve<EquipItemUseCase>();
    }
}
```

---

# MonoBehaviour Injection

MonoBehaviours should be thin.

Use MonoBehaviours mainly for:

```text
Unity lifecycle

Serialized references

UI Toolkit document access

Scene object references
```

Avoid putting business dependencies directly into random MonoBehaviours.

If a MonoBehaviour needs dependencies, inject them through the composition layer.

---

# Serialized References

Serialized references are allowed for Unity-authored content.

Examples:

```text
UIDocument

PanelSettings

AudioSource

Camera

Transform

ScriptableObject config
```

Serialized references are not a replacement for application dependencies.

Bad:

```text
[SerializeField] InventoryRepository
```

Good:

```text
[SerializeField] UIDocument
```

---

# Lifetimes

The project uses three main lifetimes:

```text
Singleton

Scoped

Transient
```

Choose the smallest lifetime that works.

---

# Singleton

Use Singleton only for application-wide services.

Examples:

```text
Router

Messaging

Session

Localization

Analytics

Audio

Global Config
```

Singleton lives for the full application lifetime.

Do not make ViewModels singleton.

Do not make scene-local services singleton.

---

# Scoped

Use Scoped for lifetime-bound services.

Examples:

```text
Scene Services

Screen ViewModels

Screen State

Battle Session

World Map Session
```

Scoped is preferred for most non-global state.

---

# Transient

Use Transient for short-lived stateless objects.

Examples:

```text
Mappers

Factories

Small Commands

Temporary Builders
```

Avoid transient objects that own resources unless disposal is clear.

---

# Feature Installers

Every feature package should provide a module installer.

Example:

```text
InventoryModuleInstaller
```

Installer responsibilities:

```text
Register UseCases

Register Repositories

Register ScreenFactories

Register EventHandlers

Register Feature Services
```

Installer must not contain business logic.

---

# Registration Ownership

Register dependencies where they are owned.

Examples:

```text
InventoryModuleInstaller registers Inventory dependencies.

ShopModuleInstaller registers Shop dependencies.

Bootstrap registers global dependencies.
```

Do not register all feature internals directly in Bootstrap.

Bootstrap should install feature installers.

---

# Interfaces And Implementations

Application defines ports.

Infrastructure implements ports.

Composition binds them.

Example:

```text
Application:
IInventoryRepository

Infrastructure:
InventoryRemoteRepository

Composition:
IInventoryRepository → InventoryRemoteRepository
```

UseCases depend on interfaces.

Not implementations.

---

# ViewModel Registration

ViewModels belong to Presentation.

ViewModels should usually be:

```text
Scoped
```

or created by:

```text
ScreenFactory
```

ViewModel lifetime should match Screen lifetime.

Avoid Singleton ViewModels.

---

# UseCase Registration

UseCases belong to Application.

UseCases are usually:

```text
Scoped
```

or:

```text
Transient
```

Use Singleton only if the UseCase is stateless and all dependencies are singleton-safe.

---

# Repository Registration

Repositories belong to Infrastructure.

Repository lifetime depends on implementation.

Examples:

```text
Remote API repository
    Singleton or Scoped

Screen-local cache repository
    Scoped

Temporary in-memory repository
    Scoped
```

Prefer explicit ownership.

---

# Event Handler Registration

Event handlers should be registered by the feature that owns the handler.

Examples:

```text
AchievementsModuleInstaller registers Achievement event handlers.

AnalyticsModuleInstaller registers Analytics event handlers.
```

Event publishers belong to the feature that publishes the event contract.

---

# Messaging Registration

MessagePipe is configured in the application scope.

Feature installers may register:

```text
Message Brokers

Publishers

Subscribers

Request Handlers
```

Do not create separate unrelated message buses per feature unless there is a clear scope reason.

---

# Scene Scope

Create Scene Scope when a scene has scene-local state.

Examples:

```text
Battle

WorldMap

Tutorial
```

Scene Scope owns:

```text
Scene services

Scene factories

Scene controllers

Scene-local object pools
```

Destroy Scene Scope when the scene unloads.

---

# Screen Scope

Create Screen Scope for complex screens.

Examples:

```text
Inventory

Shop

Profile

BattlePass
```

Screen Scope owns:

```text
ViewModel

ScreenState

Screen commands

Screen-local subscriptions
```

Destroy Screen Scope when the screen closes.

---

# Disposal

Any object that subscribes to events, owns handles, or owns resources must be disposable.

Examples:

```text
MessagePipe subscriptions

Addressables handles

CancellationTokenSource

Scene resources
```

Dispose according to scope lifetime.

---

# Cancellation

Async dependencies should accept cancellation tokens.

Examples:

```text
Screen closed

Scene unloaded

Application shutdown
```

Screen-owned async operations should be cancelled when screen scope is disposed.

---

# Factories

Use factories when runtime parameters are required.

Examples:

```text
InventoryScreenFactory

HeroViewFactory

BattleUnitFactory
```

Factories are preferred over manual `new` in arbitrary code.

---

# Manual New

Manual object creation is allowed for simple value objects and pure domain objects.

Allowed:

```text
new ItemId(...)

new Price(...)

new InventoryItemViewModel(...)
```

when no infrastructure dependency is needed.

Avoid manual creation of services.

---

# Service Locator

Forbidden outside composition.

Bad:

```text
Container.Resolve<T>()

ServiceLocator.Get<T>()

GlobalServices.Resolve<T>()
```

Reason:

```text
Hidden dependencies

Harder testing

Unclear ownership

Runtime failures
```

---

# Static Singletons

Avoid:

```text
AudioManager.Instance

GameManager.Instance

InventoryManager.Instance
```

Use DI lifetimes instead.

Static utility methods are allowed only for pure deterministic helpers.

---

# ScriptableObject Services

Do not use ScriptableObjects as hidden service locators.

ScriptableObjects are allowed for:

```text
Config

Static data

Editor-authored assets

Event channels when explicitly approved
```

They should not silently replace DI.

---

# Testing

DI should make testing easier.

Tests should use:

```text
Fake repositories

Fake gateways

Fake routers

Fake asset providers
```

instead of production infrastructure.

UseCase tests should not require the real container.

Container build tests are allowed separately.

---

# Container Build Tests

Composition should be validated.

Recommended tests:

```text
Root container builds

Feature installer builds

Critical services resolve

No missing registrations
```

These tests catch wiring errors.

---

# Anti-Patterns

Avoid:

```text
Global Managers

Hidden Service Locator

Singleton ViewModels

Repositories injected into Views

Addressables injected into Domain

Bootstrap registering every feature class manually

UseCases constructing repositories directly
```

---

# Review Checklist

Before adding a dependency ask:

1. Is the dependency explicit?
2. Is constructor injection possible?
3. Is the lifetime correct?
4. Is this registered in the owning module?
5. Is the dependency direction valid?
6. Can this be tested with a fake?
7. Does this avoid service locator?
8. Is disposal handled?

If any answer is no:

Refactor before merging.

---

# Summary

Use this rule:

```text
DI creates objects.

Architecture defines dependencies.

Scopes define lifetimes.

Features own registrations.

Bootstrap wires the application.
```
