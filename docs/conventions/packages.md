# Packages

## Purpose

This document defines how packages are organized in the project.

The project uses local Unity packages as the primary architectural boundary.

Packages represent ownership boundaries.

Packages are more important than folders.

---

# Why Packages

The project uses local UPM packages because they provide:

* Clear ownership.
* Explicit dependencies.
* Assembly isolation.
* Faster compilation.
* Better modularity.
* Easier testing.
* Easier extraction of features.
* Better long-term maintainability.

Features should be isolated through packages whenever possible.

---

# Package Categories

The project contains three types of packages:

```text
Bootstrap
Foundation
Feature Packages
```

---

# Bootstrap Package

There is exactly one bootstrap package.

Example:

```text
com.company.game.bootstrap
```

Responsibilities:

* Application startup.
* Dependency registration.
* Composition root.
* Initial routing.
* Global application lifetime.
* Environment initialization.

Bootstrap is the outermost layer.

All feature packages may be installed from Bootstrap.

Feature packages must not depend on Bootstrap.

---

# Foundation Package

There is usually one foundation package.

Example:

```text
com.company.game.foundation
```

Purpose:

* Shared abstractions.
* Shared primitives.
* Shared contracts.
* Shared infrastructure abstractions.
* Shared presentation abstractions.

Foundation should remain small.

Foundation is not a feature.

Foundation does not own business logic.

---

# Feature Packages

Feature packages represent business capabilities.

Examples:

```text
com.company.game.feature.inventory

com.company.game.feature.shop

com.company.game.feature.profile

com.company.game.feature.matchmaking

com.company.game.feature.battle

com.company.game.feature.guilds
```

A feature package owns:

* Domain
* Application
* Infrastructure
* Presentation
* Contracts
* Dependency registration

A feature owns everything required to implement its business capability.

---

# Package Naming

Use the following convention:

```text
com.company.game.<area>
```

Examples:

```text
com.company.game.bootstrap

com.company.game.foundation

com.company.game.feature.inventory

com.company.game.feature.shop

com.company.game.feature.profile
```

Feature names should represent business capabilities.

---

# Good Package Names

```text
Inventory
Shop
Profile
Battle
Matchmaking
Friends
Guilds
Quests
BattlePass
Settings
```

---

# Bad Package Names

```text
Utils

Managers

CoreLogic

Network

Database

GameplayStuff

CommonFeatures
```

Packages should describe business ownership.

Packages should not describe technical implementation.

---

# When To Create A New Package

Create a new package when:

* A new business capability appears.
* Ownership becomes unclear.
* A feature becomes large enough to evolve independently.
* A feature may eventually be maintained by a separate team.
* The feature has its own UI, Application, Domain, and Infrastructure.

Examples:

```text
Inventory

Shop

Battle

Guilds
```

These are good package candidates.

---

# When NOT To Create A New Package

Do not create a package because:

* A folder became large.
* A namespace became large.
* A technical concern appeared.
* A utility class appeared.

Bad examples:

```text
Network Package

Logging Package

Serialization Package

Managers Package
```

These usually belong to Foundation or Infrastructure abstractions.

---

# Package Ownership

Every file should have a clear owner.

Ask:

```text
Which feature owns this?
```

Example:

```text
InventoryItem
```

Owner:

```text
Inventory
```

Therefore:

```text
Inventory Package
```

Not:

```text
Foundation
```

---

# Shared Code Rule

The default location is:

```text
Feature Package
```

Not:

```text
Foundation
```

Code should move to Foundation only if:

1. It is used by multiple features.
2. It is generic.
3. It has no business ownership.
4. It can survive independently.

---

# Prefer Duplication Over Premature Sharing

Bad:

```text
SharedItemManager
```

used by:

```text
Inventory

Shop
```

after only one usage.

Good:

Keep implementations separate.

Move to Foundation only after a clear pattern emerges.

---

# Package Layout

Every package should follow:

```text
Package/
├── package.json
├── Runtime/
├── Editor/
└── Tests/
```

---

# Runtime Layout

Feature packages use:

```text
Runtime/
├── Contracts/
├── Domain/
├── Application/
├── Infrastructure/
├── Presentation/
└── Composition/
```

Details:

```text
docs/architecture/modular-monolith.md
```

---

# Editor Layout

Editor code belongs in:

```text
Editor/
```

Examples:

```text
Custom Inspectors

Import Tools

Validators

Menu Items
```

Editor code must never leak into Runtime assemblies.

---

# Tests Layout

Tests belong in:

```text
Tests/
├── Editor/
└── Runtime/
```

Editor tests:

```text
Domain

Application

Editor Tools
```

Runtime tests:

```text
Presentation

Infrastructure

Integration
```

---

# Package Dependencies

Allowed:

```text
Feature → Foundation

Bootstrap → Feature

Bootstrap → Foundation
```

Allowed:

```text
Inventory → Inventory.Contracts

Shop → Inventory.Contracts
```

Forbidden:

```text
Inventory → Shop.Domain

Inventory → Shop.Application

Inventory → Shop.Infrastructure

Inventory → Shop.Presentation
```

Feature packages must not depend on internal implementation of another feature.

---

# Public API

Every feature should expose a public API through Contracts.

Example:

```text
Contracts/
├── Events/
├── Dtos/
├── Requests/
└── Responses/
```

Other packages may reference Contracts.

Nothing else.

---

# Package Manifest

Every package must contain:

```json
{
  "name": "com.company.game.feature.inventory",
  "version": "1.0.0",
  "displayName": "Inventory Feature",
  "unity": "6000.0"
}
```

Use semantic versioning.

---

# Package Extraction Rule

A feature package should be extractable.

Ask:

```text
Could this package be moved into another repository?
```

If the answer is no:

The package likely depends on too many internal details.

Reduce coupling.

---

# Review Checklist

Before creating a package ask:

1. Is this a business capability?
2. Does it have clear ownership?
3. Could it evolve independently?
4. Does it need its own UI/Application/Domain?
5. Is Foundation truly inappropriate?

If most answers are yes:

Create a package.

Otherwise:

Use an existing package.

---

# Summary

Packages are the primary architectural boundary.

Use this rule:

```text
One business capability
=
One feature package
```

Features own their code.

Foundation owns shared abstractions.

Bootstrap owns startup.

Nothing else should become a top-level package without a strong architectural reason.
