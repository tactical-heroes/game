# Dependency Injection

## Purpose

The project uses VContainer for explicit dependency wiring.
DI makes ownership, lifetimes, and test seams visible.

## Core Rule

Dependencies must be explicit.

Prefer:

```text
Constructor parameters -> registered dependencies -> clear lifetime
```

Avoid:

* Service locators.
* Hidden static singletons.
* `new` for concrete infrastructure in UseCases or ViewModels.
* Container access from Domain, Application logic, or Presentation logic.

## Registration Location

Register dependencies in:

* Bootstrap application scope.
* Feature Composition installers.
* Scene scope installers.
* Screen scope factories or installers.

Do not register dependencies from Domain entities, UseCases, ViewModels, Views,
or infrastructure adapters.

## Injection Style

Use constructor injection for plain C# classes:

```csharp
public sealed class EquipItemUseCase
{
    public EquipItemUseCase(IInventoryRepository repository)
    {
        _repository = repository;
    }
}
```

MonoBehaviours should stay thin. Use serialized references for scene objects and
inject only orchestration dependencies needed by the adapter.

## Lifetimes

Choose the smallest lifetime that works:

| Lifetime  | Use for 														|
| --- 		| --- 															|
| Singleton | Stateless app-wide services, configuration, stable clients 	|
| Scoped 	| Scene services, screen ViewModels, screen state, local caches |
| Transient | Short-lived stateless objects 								|

Do not make ViewModels, scene-local services, or screen-local resources
application singletons.

## Feature Installers

Every feature with runtime services should provide a Composition installer.
The installer registers feature-owned:

* UseCases.
* Ports and adapters.
* Event handlers.
* Screen factories.
* Presentation services.

Bootstrap installs feature installers instead of registering feature internals
directly.

## Common Registrations

Use abstractions at layer boundaries:

```text
IInventoryRepository -> InventoryRemoteRepository
IInventoryIconProvider -> AddressablesInventoryIconProvider
IInventoryScreenFactory -> InventoryScreenFactory
```

UseCases usually depend on ports. ViewModels usually depend on UseCases,
routers, and presentation services.

## Disposal And Cancellation

Any object that owns subscriptions, handles, async operations, or external
resources must be disposed with its scope.

Screen-owned async operations should be cancelled when the screen closes.
Scene-owned operations should be cancelled when the scene unloads.

## Factories

Use factories when creation needs runtime parameters, Unity objects, scoped
lifetime, or Addressables handles.

Do not use factories to hide a service locator.

## Testing

UseCases and ViewModels should be testable with fakes without building the real
container. Add container build tests for installers and root composition when
registrations become non-trivial.

## Review Checklist

Before adding a dependency, verify:

1. The dependency is explicit.
2. The registration owner is correct.
3. The lifetime matches ownership.
4. No layer resolves from the container directly.
5. Disposal and cancellation are covered.
