## 1. Core Rule

* **Dependency Direction**: Dependencies must point inward: `Presentation → Application → Domain`, `Infrastructure → Application/Domain`, `Bootstrap → Feature Modules`.
* **Stability Rule**: Domain is the most stable and independent layer. UI and Infrastructure are replaceable.
* **Bootstrap Rule**: Bootstrap is the outermost composition layer. It wires everything together.
* **Feature Rule**: Features expose contracts, not internals.

## 2. Layer Rules

* **Domain**: Contains business rules. May depend only on `Foundation.Domain`. Must not depend on Unity, Presentation, Application implementations, Infrastructure, Bootstrap, Addressables, PlayerPrefs, network clients, or DI containers. Must be testable as plain C#.
* **Application**: Contains use cases and orchestration. May depend on `Domain`, `Contracts`, `Foundation.Application`, `Foundation.Domain`. Owns ports/interfaces like `IInventoryRepository`, `IShopGateway`, `IAnalyticsPort`. Must not depend on Presentation, Infrastructure implementations, Bootstrap, Unity scene objects, MonoBehaviour, UI Toolkit, Addressables implementation, or concrete HTTP clients.
* **Presentation**: Contains UI and presentation logic. May depend on `Application`, `Contracts`, `Foundation.Presentation`, and Domain DTOs/value objects when needed. May use Unity/UI Toolkit APIs. Must not depend on Infrastructure implementations, Bootstrap, concrete backend clients, save adapters, or analytics adapters.
* **Infrastructure**: Contains external adapters. May depend on `Application`, `Domain`, `Contracts`, `Foundation.Infrastructure`, Unity APIs, and external SDKs. Implements Application ports. Must not depend on Presentation, ViewModels, Views, Screens, or Bootstrap runtime logic. Must not contain business rules.
* **Composition**: Wires dependencies for one feature. May depend on all layers of the same feature. Must not contain business logic.
* **Bootstrap**: Application Composition Root. May depend on Foundation, all feature packages, Unity runtime, and DI container. Feature packages must not depend on Bootstrap.

## 3. Cross-Feature Dependencies

* **Allowed**: Features may depend on another feature only through `Contracts`.

  * `FeatureA.Application → FeatureB.Contracts`
  * `FeatureA.Presentation → FeatureB.Contracts`
  * `FeatureA.Infrastructure → FeatureB.Contracts`
* **Forbidden**: A feature must not reference another feature's `Domain`, `Application`, `Infrastructure`, `Presentation`, or `Composition`.
* **Contracts**: Public API of a feature. May contain events, DTOs, requests, responses, public read models, stable identifiers. Must not contain business rules, infrastructure implementation, UI logic, MonoBehaviours, scene references, or Addressables implementation details.

## 4. Foundation Rules

* **Purpose**: Foundation contains shared abstractions only.
* **Allowed**: `Feature.* → Foundation`.
* **Forbidden**: `Foundation → Feature.*`.
* **Constraint**: Foundation must remain small. Do not move code to Foundation only because it is convenient.

## 5. Assembly Definition Rules

* **Required**: Every package must have asmdefs.
* **Basic Structure**: `Company.Game.Foundation`, `Company.Game.Bootstrap`, `Company.Game.Feature.Inventory`, `Company.Game.Feature.Inventory.Editor`, `Company.Game.Feature.Inventory.Tests.Editor`, `Company.Game.Feature.Inventory.Tests.Runtime`.
* **Large Feature Structure**: Split by layer: `Contracts`, `Domain`, `Application`, `Infrastructure`, `Presentation`, `Composition`.
* **Small Feature Exception**: A single runtime asmdef is acceptable, but dependency rules still apply logically.
* **Bootstrap References**: Bootstrap should reference feature `Composition` assemblies, not feature internals.

## 6. Forbidden References

* **Layer Violations**:

  * `Domain → Application`
  * `Domain → Infrastructure`
  * `Domain → Presentation`
  * `Application → Infrastructure`
  * `Application → Presentation`
  * `Presentation → Infrastructure`
  * `Infrastructure → Presentation`
* **Boundary Violations**:

  * `Feature → Bootstrap`
  * `Foundation → Feature`
  * `FeatureA.Domain → FeatureB.Domain`
  * `FeatureA.Application → FeatureB.Application`
  * `FeatureA.Presentation → FeatureB.Presentation`

## 7. UI & ViewModel Rules

* **UI Toolkit**: Belongs to Presentation only.
* **Allowed**: `Presentation → UnityEngine.UIElements`, `Presentation → Application`, `Presentation → Contracts`.
* **Forbidden**: `Application → UnityEngine.UIElements`, `Domain → UnityEngine.UIElements`, `Infrastructure → ViewModel/View/ScreenFactory`.
* **UXML/USS**: Should live near their screen or component code.
* **ViewModels**: Belong to Presentation. May depend on Application use cases, queries, commands, Contracts, and Foundation Presentation abstractions. Must not depend on Infrastructure repositories, backend clients, save adapters, Addressables providers, Bootstrap, or another feature's internals. Must not contain domain business rules.

## 8. Event Rules

* **Domain Events**: Internal to Domain.
* **Application Events**: May be published by Application.
* **Cross-Feature Events**: Must live in Contracts.
* **Allowed**: `Inventory.Application` publishes `Inventory.Contracts.Events.InventoryItemEquippedEvent`; other features or Infrastructure may subscribe to that contract event.
* **Forbidden**: Presentation publishing UI click events to the global bus, Domain entities publishing MessagePipe events directly, or features listening to another feature's internal DomainEvents.
* **Event Meaning**: Events should represent facts that already happened.

## 9. Scene Rules

* **Location**: Scenes belong to `Assets/Scenes`.
* **Scene Objects May Reference**: Presentation components, scene entry points, Bootstrap scene components, Unity assets.
* **Scene Objects Must Not Contain**: Business rules, Application orchestration, or Infrastructure implementation details.
* **Scene Entry Points**: May initialize scene-local dependencies only. Global application initialization belongs to Bootstrap.

## 10. Infrastructure Adapter Rules

* **Core Rule**: Infrastructure adapters implement Application ports.
* **Allowed**:

  * `UseCase → IInventoryRepository`
  * `InventoryRemoteRepository → IInventoryRepository`
* **Forbidden**:

  * `UseCase → InventoryRemoteRepository`
  * `ViewModel → InventoryRemoteRepository`
  * `Domain → InventoryRemoteRepository`

## 11. Dependency Injection Rules

* **Registration Location**: DI registration belongs to Composition or Bootstrap.
* **Allowed**: Feature installer registers feature use cases and adapters. Bootstrap installs feature installers.
* **Forbidden**: Domain resolving services from container, ViewModel resolving dependencies from global service locator, UseCase creating concrete Infrastructure implementation directly.
* **Injection Style**: Prefer constructor injection.

## 12. Unity Exceptions

* **Allowed**: Presentation may use `MonoBehaviour`; Infrastructure may use Unity APIs for platform integration; Bootstrap may reference feature installers; Editor assemblies may reference runtime assemblies.
* **Hard Constraint**: Exceptions must not violate Domain purity.
* **Default Rule**: When in doubt, keep Domain pure and move Unity-specific logic outward.

## 13. Review Checklist

Before adding a dependency, ensure:

1. The dependency points inward.
2. The feature depends only on another feature's Contracts.
3. Domain is independent from Unity.
4. Application is independent from Infrastructure implementations.
5. Presentation is independent from concrete Infrastructure.
6. Shared code is generic enough for Foundation.
7. The code can be tested without Unity runtime.
8. The dependency is visible in asmdef references.

If not, reconsider the design.
