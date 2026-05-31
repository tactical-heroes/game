# Assembly Definitions (asmdef)

## Purpose

This document defines how Assembly Definitions (`.asmdef`) are organized.

Assembly Definitions are used to:

* Enforce architecture boundaries.
* Reduce compilation times.
* Make dependencies explicit.
* Prevent accidental coupling.
* Improve maintainability.

Architecture rules must be enforced through assemblies whenever possible.

---

# Why Assembly Definitions

Without asmdef files Unity compiles most project code into large shared assemblies.

This creates several problems:

* Slow compilation.
* Hidden dependencies.
* Accidental coupling.
* Difficult architecture enforcement.

Assembly Definitions solve these issues.

---

# Core Principle

Dependencies should be visible.

If a dependency exists:

```text
Assembly A
    ↓
Assembly B
```

then Assembly A must explicitly reference Assembly B.

Nothing should happen implicitly.

---

# Assembly Strategy

The project uses:

```text
Package
    ↓
Assemblies
```

Packages are ownership boundaries.

Assemblies are dependency boundaries.

---

# Small Feature Strategy

For small features:

```text
Feature Package
    ↓
Single Runtime Assembly
```

Example:

```text
Company.Game.Feature.Settings
```

Structure:

```text
Runtime/
Editor/
Tests/
```

Single runtime assembly is acceptable when:

* The feature is small.
* The feature has low complexity.
* The feature is unlikely to grow significantly.

---

# Large Feature Strategy

For large features:

```text
Contracts
Domain
Application
Infrastructure
Presentation
Composition
```

Each layer receives its own assembly.

Example:

```text
Company.Game.Feature.Inventory.Contracts

Company.Game.Feature.Inventory.Domain

Company.Game.Feature.Inventory.Application

Company.Game.Feature.Inventory.Infrastructure

Company.Game.Feature.Inventory.Presentation

Company.Game.Feature.Inventory.Composition
```

This is the preferred approach for long-lived business features.

---

# Foundation Assemblies

Foundation should be split by responsibility.

Recommended:

```text
Company.Game.Foundation.Domain

Company.Game.Foundation.Application

Company.Game.Foundation.Presentation

Company.Game.Foundation.Infrastructure
```

Optional:

```text
Company.Game.Foundation.Editor
```

Avoid creating a giant Foundation assembly.

---

# Bootstrap Assemblies

Bootstrap typically contains:

```text
Company.Game.Bootstrap
```

Optional:

```text
Company.Game.Bootstrap.Editor
```

Bootstrap is usually small enough for a single runtime assembly.

---

# Recommended Inventory Example

```text
Company.Game.Feature.Inventory.Contracts

Company.Game.Feature.Inventory.Domain

Company.Game.Feature.Inventory.Application

Company.Game.Feature.Inventory.Infrastructure

Company.Game.Feature.Inventory.Presentation

Company.Game.Feature.Inventory.Composition
```

---

# Contracts Assembly

Contains:

```text
Events
DTOs
Requests
Responses
Public Read Models
```

Allowed references:

```text
Foundation.Domain
Foundation.Application
```

Should remain lightweight.

---

# Domain Assembly

Contains:

```text
Entities
Aggregates
ValueObjects
DomainServices
DomainEvents
```

Allowed references:

```text
Foundation.Domain
```

Forbidden:

```text
UnityEngine

Infrastructure

Presentation

Application
```

Domain should compile without Unity runtime.

---

# Application Assembly

Contains:

```text
UseCases
Commands
Queries
Ports
ApplicationServices
```

Allowed references:

```text
Contracts

Domain

Foundation.Application

Foundation.Domain
```

Forbidden:

```text
Infrastructure

Presentation

Unity UI
```

Application owns interfaces.

Infrastructure implements them.

---

# Infrastructure Assembly

Contains:

```text
Repositories
Backend Clients
Analytics Adapters
Addressables Adapters
Persistence Adapters
```

Allowed references:

```text
Contracts

Domain

Application

Foundation.Infrastructure

Foundation.Application
```

Forbidden:

```text
Presentation
```

Infrastructure implements Application ports.

---

# Presentation Assembly

Contains:

```text
ViewModels
Views
Screens
Navigation
ScreenFactories
UI Components
```

Allowed references:

```text
Application

Contracts

Foundation.Presentation
```

Forbidden:

```text
Infrastructure
```

Presentation should not know about concrete adapters.

---

# Composition Assembly

Contains:

```text
Installers

Dependency Registration

Module Registration
```

Allowed references:

```text
Contracts

Domain

Application

Infrastructure

Presentation
```

Composition is the only layer allowed to see everything inside the feature.

---

# Assembly Dependency Diagram

```text
Contracts

Domain
    ↓

Application
    ↓

Presentation

Infrastructure
    ↓

Composition
```

More explicitly:

```text
Presentation → Application

Application → Domain

Infrastructure → Application

Infrastructure → Domain

Composition → All
```

---

# Cross-Feature References

Allowed:

```text
Shop.Application
    ↓
Inventory.Contracts
```

Allowed:

```text
Achievements.Application
    ↓
Inventory.Contracts
```

Forbidden:

```text
Shop.Application
    ↓
Inventory.Application
```

Forbidden:

```text
Shop.Domain
    ↓
Inventory.Domain
```

Features communicate through Contracts only.

---

# Editor Assemblies

Every package may contain:

```text
Company.Game.Feature.Inventory.Editor
```

Editor assemblies contain:

```text
Inspectors

Validators

Import Tools

Menu Items
```

Editor assemblies may reference runtime assemblies.

Runtime assemblies must never reference editor assemblies.

---

# Test Assemblies

Every feature should contain:

```text
Company.Game.Feature.Inventory.Tests.Editor

Company.Game.Feature.Inventory.Tests.Runtime
```

---

# Editor Test Assemblies

Contains:

```text
Domain Tests

Application Tests

Editor Tool Tests
```

Allowed references:

```text
Domain

Application

Contracts
```

Editor tests should be preferred whenever possible.

---

# Runtime Test Assemblies

Contains:

```text
Presentation Tests

Infrastructure Tests

Integration Tests
```

Allowed references:

```text
Presentation

Infrastructure

Composition
```

Use Runtime tests only when Unity lifecycle is required.

---

# Assembly Naming

Runtime:

```text
Company.Game.Feature.Inventory.Domain

Company.Game.Feature.Inventory.Application

Company.Game.Feature.Inventory.Infrastructure

Company.Game.Feature.Inventory.Presentation

Company.Game.Feature.Inventory.Composition
```

Editor:

```text
Company.Game.Feature.Inventory.Editor
```

Tests:

```text
Company.Game.Feature.Inventory.Tests.Editor

Company.Game.Feature.Inventory.Tests.Runtime
```

Use PascalCase.

Assembly names should match namespaces.

---

# Namespace Alignment

Assembly:

```text
Company.Game.Feature.Inventory.Domain
```

Namespace:

```csharp
namespace Company.Game.Features.Inventory.Domain
{
}
```

Assembly and namespace should align whenever possible.

---

# When To Split Assemblies

Create separate assemblies when:

* Feature complexity grows.
* Compilation time becomes noticeable.
* Architecture boundaries matter.
* Multiple developers work on the feature.
* Long-term maintenance is expected.

---

# When NOT To Split Assemblies

Do not create six assemblies for:

```text
Settings

Credits

About Screen

Simple Prototype Features
```

Use a single runtime assembly.

Prefer simplicity.

---

# Assembly Review Checklist

Before creating a reference ask:

1. Does the dependency point inward?
2. Can this dependency be replaced with Contracts?
3. Can this dependency be replaced with an interface?
4. Is Domain still independent?
5. Is Presentation still isolated from Infrastructure?
6. Is this dependency visible in asmdef?

If any answer is no:

Reconsider the design.

---

# Summary

Assemblies are architectural boundaries.

Use this rule:

```text
Packages define ownership.

Assemblies define dependencies.

Compiler-enforced boundaries are preferred over documentation-only boundaries.
```

If architecture rules cannot be enforced through asmdef references, they will eventually be violated.
