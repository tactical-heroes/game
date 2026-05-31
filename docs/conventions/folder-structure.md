# Folder Structure

## Purpose

This document defines the physical structure of the repository.

The goals are:

* Predictable navigation.
* Clear ownership.
* Consistent placement of assets and code.
* Separation of content and runtime logic.
* Scalability for long-term development.

Every new file should have an obvious location.

When in doubt, follow the rules in this document.

---

# Repository Layout

```text
/
├── Assets/
├── Packages/
├── BuildScripts/
├── docs/
├── .github/
└── AGENTS.md
```

---

# Assets Directory

The `Assets` directory contains Unity-authored content.

Rule:

```text
If Unity creates it, imports it, or serializes it,
it most likely belongs in Assets.
```

Examples:

* Scenes
* Prefabs
* Models
* Materials
* Textures
* Audio
* ScriptableObjects
* Addressables configuration
* Project configuration assets

---

# Assets Structure

```text
Assets/
├── Art/
├── Audio/
├── Scenes/
├── Settings/
├── AddressableAssetsData/
├── Localization/
├── StreamingAssets/
└── ThirdParty/
```

---

# Assets/Art

Contains visual content.

Examples:

```text
Assets/Art/
├── Characters/
├── Environment/
├── UI/
├── VFX/
├── Materials/
├── Textures/
└── Shaders/
```

Contains:

* Models
* Materials
* Textures
* Sprites
* Icons
* Visual Effects
* Shaders

Must not contain:

* Business logic
* Runtime code
* Feature implementation

---

# Assets/Audio

Contains audio content.

Examples:

```text
Assets/Audio/
├── Music/
├── SFX/
├── Voice/
├── FMOD/
└── Wwise/
```

Contains:

* Audio clips
* Audio banks
* Audio middleware assets

Must not contain:

* Gameplay code
* Runtime services

---

# Assets/Scenes

Contains all Unity scenes.

Recommended structure:

```text
Assets/Scenes/
├── Bootstrap/
│   └── 00_Bootstrap.unity
│
├── Shell/
│   └── AppShell.unity
│
├── Menu/
│   ├── MainMenu.unity
│   └── Profile.unity
│
├── Gameplay/
│   ├── WorldMap.unity
│   ├── Battle.unity
│   └── Tutorial.unity
│
└── Preview/
    ├── InventoryPreview.unity
    └── ShopPreview.unity
```

Rules:

* Scene authoring belongs in Assets.
* Scenes are not stored inside feature packages.
* Preview scenes are allowed.
* Bootstrap scene should be clearly identifiable.

Recommended naming:

```text
00_Bootstrap.unity
10_MainMenu.unity
20_WorldMap.unity
30_Battle.unity
```

---

# Assets/Settings

Contains project-level configuration assets.

Examples:

```text
Assets/Settings/
├── Input/
├── Rendering/
├── Localization/
├── StartupRoutes/
├── Themes/
└── Runtime/
```

Contains:

* Input System assets
* Render Pipeline assets
* Localization assets
* Runtime configuration ScriptableObjects
* Startup route assets

Should remain small and easy to discover.

---

# Assets/AddressableAssetsData

Contains Addressables configuration.

Examples:

```text
Assets/AddressableAssetsData/
├── AssetGroups/
├── Profiles/
├── Schemas/
└── BuildSettings/
```

Contains:

* Groups
* Profiles
* Schemas
* Catalog settings

Must not contain gameplay logic.

---

# Assets/Localization

Contains localization assets.

Examples:

```text
Assets/Localization/
├── Tables/
├── Fonts/
└── SmartStrings/
```

---

# Assets/StreamingAssets

Contains files that must be deployed without modification.

Examples:

```text
Assets/StreamingAssets/
├── Config/
├── Json/
└── StaticData/
```

Use only when Unity requires StreamingAssets behavior.

Prefer Addressables when possible.

---

# Assets/ThirdParty

Contains imported third-party assets that are not installed through UPM.

Examples:

```text
Assets/ThirdParty/
├── VendorA/
├── VendorB/
└── AssetStorePackage/
```

Purpose:

* Isolate vendor content.
* Avoid mixing external and first-party assets.

Never modify vendor assets directly.

Create wrappers instead.

---

# Packages Directory

The `Packages` directory contains first-party source code.

Rule:

```text
If developers write it,
it most likely belongs in Packages.
```

Contains:

* Runtime code
* Editor code
* Tests
* Architecture
* Features

---

# Packages Structure

```text
Packages/
├── com.company.game.bootstrap/
├── com.company.game.foundation/
├── com.company.game.feature.inventory/
├── com.company.game.feature.shop/
├── com.company.game.feature.profile/
└── ...
```

Every feature should be a package.

---

# Bootstrap Package

```text
Packages/com.company.game.bootstrap/
```

Contains:

* App startup
* Composition root
* Dependency registration
* Initial routing
* Global lifetime

This package owns application startup.

---

# Foundation Package

```text
Packages/com.company.game.foundation/
```

Contains:

* Shared abstractions
* Shared contracts
* Shared primitives
* Shared UI abstractions
* Shared infrastructure abstractions

Foundation must remain small.

It is not a dumping ground.

---

# Feature Packages

Recommended naming:

```text
com.company.game.feature.inventory
com.company.game.feature.shop
com.company.game.feature.profile
com.company.game.feature.battle
```

A feature package owns:

* UI
* Use Cases
* Domain
* Infrastructure
* Contracts
* Dependency registration

---

# Feature Package Layout

```text
Packages/com.company.game.feature.inventory/

├── package.json

├── Runtime/
│
├── Editor/
│
└── Tests/
```

---

# Runtime Structure

```text
Runtime/
├── Contracts/
├── Domain/
├── Application/
├── Infrastructure/
├── Presentation/
└── Composition/
```

Detailed rules are described in:

```text
docs/architecture/modular-monolith.md
```

---

# Editor Structure

```text
Editor/
├── Inspectors/
├── Validators/
├── Importers/
└── MenuItems/
```

Contains:

* Custom inspectors
* Validation tools
* Asset import tools
* Editor automation

Editor code must never leak into Runtime assemblies.

---

# Tests Structure

```text
Tests/
├── Editor/
└── Runtime/
```

Editor:

```text
Tests/Editor/
├── Domain/
├── Application/
└── EditorTools/
```

Runtime:

```text
Tests/Runtime/
├── Presentation/
├── Infrastructure/
└── Integration/
```

Rules:

* Prefer Editor tests.
* Use Runtime tests only when Unity lifecycle is required.

---

# Documentation Directory

```text
docs/
├── architecture/
├── unity/
├── conventions/
├── development/
└── examples/
```

Purpose:

* Architecture documentation.
* Team conventions.
* Agent instructions.
* Development guides.

Documentation is part of the codebase.

Update documentation when architecture changes.

---

# BuildScripts Directory

Contains build automation.

```text
BuildScripts/
├── Build.cs
├── AddressablesBuild.cs
├── BuildProfiles.cs
└── Versioning.cs
```

Responsibilities:

* CI builds.
* Local builds.
* Version generation.
* Addressables builds.

Build automation must not depend on manual editor actions.

---

# GitHub Directory

```text
.github/
└── workflows/
```

Contains:

* Validation pipelines.
* Test pipelines.
* Build pipelines.
* Release pipelines.

Examples:

```text
validate.yml
test.yml
build-client.yml
release.yml
```

---

# What Does NOT Belong In Assets

The following should not be placed inside Assets:

```text
UseCases
Domain Models
Repositories
Application Services
Feature Logic
Dependency Registration
```

These belong inside Packages.

---

# What Does NOT Belong In Packages

The following should not be placed inside Packages:

```text
Scenes
Models
Materials
Textures
Audio Clips
Addressables Settings
Render Pipeline Assets
```

These belong inside Assets.

---

# Naming Rules

Directories should use PascalCase.

Examples:

```text
Inventory
Battle
Profile
Settings
StartupRoutes
```

Package names should use:

```text
com.company.game.<area>
```

Examples:

```text
com.company.game.bootstrap
com.company.game.foundation
com.company.game.feature.inventory
```

---

# Decision Rule

When creating a new file ask:

```text
Is this Unity-authored content?
```

If yes:

```text
Assets/
```

If no:

```text
Packages/
```

Then ask:

```text
Which feature owns it?
```

Place the file inside the owning feature package.

Avoid creating global folders unless ownership is truly shared.

---

# Summary

The repository follows a simple rule:

```text
Assets = Content

Packages = Code

Features own their code

Bootstrap owns startup

Foundation owns shared abstractions
```

Every file should have a clear owner and a predictable location.
