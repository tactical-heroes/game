# Dependency Rules

## Core Rule

Dependencies point inward:

```text
Presentation -> Application -> Domain
Infrastructure -> Application
Infrastructure -> Domain
Bootstrap -> Feature Composition
```

Domain is the most stable layer. UI and Infrastructure are replaceable.

## Layer Rules

* Domain contains business rules. It may depend only on domain-level shared
  abstractions. It must not depend on Unity, DI containers, Infrastructure,
  Presentation, Bootstrap, Addressables, PlayerPrefs, or network clients.
* Application contains use cases and orchestration. It may depend on Domain,
  Contracts, and Foundation abstractions. It owns ports such as repositories
  and gateways. It must not depend on Presentation or Infrastructure
  implementations.
* Presentation contains UI Toolkit views, ViewModels, screen state, and screen
  factories. It may use Unity APIs. It must not depend on concrete
  Infrastructure implementations or Bootstrap.
* Infrastructure implements Application ports and external adapters. It may use
  Unity APIs and SDKs. It must not depend on Presentation or contain business
  rules.
* Composition wires dependencies for one feature. It may depend on all layers
  of the same feature. It must not contain business logic.
* Bootstrap wires the application. Feature packages must not depend on it.

## Cross-Feature Rules

Allowed:

```text
FeatureA.* -> FeatureB.Contracts
```

Forbidden:

```text
FeatureA.* -> FeatureB.Domain
FeatureA.* -> FeatureB.Application
FeatureA.* -> FeatureB.Infrastructure
FeatureA.* -> FeatureB.Presentation
FeatureA.* -> FeatureB.Composition
```

Contracts may contain public DTOs, events, requests, responses, read models,
and stable identifiers. They must not contain business rules, UI logic,
MonoBehaviours, scene references, or infrastructure details.

## Foundation Rules

Allowed:

```text
Feature.* -> Foundation
Bootstrap -> Foundation
```

Forbidden:

```text
Foundation -> Feature.*
```

Foundation stays small and generic. Move code there only after real reuse and
a stable abstraction exist.

## Unity Rules

Unity APIs are allowed in:

* Presentation, for UI and MonoBehaviour adapters.
* Infrastructure, for platform integration and Unity-backed adapters.
* Bootstrap, for startup and scene objects.
* Editor assemblies.

Unity APIs are not allowed in Domain. Application may use Unity only through
approved abstractions, not scene objects or UI types.

## Assembly Enforcement

Every package should have asmdefs. Large features should split assemblies by
layer. Small features may use one runtime asmdef while still following the
logical dependency rules.

See `docs/conventions/asmdef.md`.

## Review Checklist

Before adding a dependency, verify:

1. It points inward.
2. Cross-feature access goes through Contracts.
3. Domain remains pure C#.
4. Application depends on ports, not adapters.
5. Presentation does not know concrete infrastructure.
6. Shared code really belongs in Foundation.
7. The dependency is visible in asmdef references.
