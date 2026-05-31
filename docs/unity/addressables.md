# Addressables

## Purpose

This document defines how Addressables are used in the project.

Addressables are used for:

* Asset loading.
* Content delivery.
* Memory control.
* Async resource management.
* Optional remote content.

Addressables are infrastructure.

They should not leak into Domain or Application logic.

---

# Core Rule

```text
Addressables belong to Infrastructure.
```

Domain, Application, and ViewModels should not directly load Addressables.

Use an abstraction.

Preferred:

```text
ViewModel
    ↓
UseCase / Presentation Service
    ↓
IAssetProvider / IIconProvider
    ↓
Addressables Implementation
```

Forbidden:

```text
ViewModel
    ↓
Addressables.LoadAssetAsync
```

---

# Where Addressables Config Lives

Addressables configuration belongs to:

```text
Assets/AddressableAssetsData/
```

This includes:

```text
Groups

Profiles

Schemas

Build Settings

Catalog Settings
```

Do not mix Addressables configuration with feature source code.

---

# Where Addressable Assets Live

Addressable assets usually live in:

```text
Assets/Art/
Assets/Audio/
Assets/Localization/
Assets/Scenes/
```

Examples:

```text
Assets/Art/UI/Icons/Inventory/
Assets/Art/Characters/Heroes/
Assets/Audio/SFX/
Assets/Scenes/Gameplay/
```

Feature code references assets through abstractions, not direct paths.

---

# Addressables Groups

Groups should be organized by ownership and loading behavior.

Recommended group types:

```text
Core

UI

Feature

Scenes

RemoteContent
```

Example:

```text
Addressables Groups/
├── Core
├── UI_Common
├── Feature_Inventory
├── Feature_Shop
├── Feature_Battle
├── Scenes_Gameplay
└── RemoteContent
```

---

# Group Naming

Use PascalCase or clear snake-like names consistently.

Recommended:

```text
Core

UI_Common

Feature_Inventory

Feature_Shop

Feature_Battle

Scenes_Gameplay
```

Avoid:

```text
Default Local Group

New Group

Group1

Temp
```

---

# Asset Address Naming

Addresses should be stable and feature-oriented.

Good:

```text
Inventory/Icons/Sword

Inventory/Icons/Shield

Profile/Avatars/Knight

Battle/VFX/Explosion

Scenes/Battle/Forest
```

Bad:

```text
Assets/Art/UI/icon_01.png

Sword2

NewAsset

Temp/Icon

guid_12345
```

Do not expose physical folder structure as the public address if it is likely to change.

---

# Labels

Use labels for loading asset sets.

Examples:

```text
InventoryIcons

ProfileAvatars

BattleVfx

Preload

Remote
```

Labels should describe loading behavior or ownership.

Avoid generic labels:

```text
Assets

Stuff

Temp

Misc
```

---

# Asset References

Use `AssetReference` when an asset needs to be configured in Inspector.

Use typed references where possible.

Examples:

```text
AssetReferenceSprite

AssetReferenceGameObject

AssetReferenceT<T>
```

Do not pass `AssetReference` into Domain.

Do not store `AssetReference` inside pure business models.

---

# Asset Provider

All Addressables access should go through project abstractions.

Recommended shared interface:

```csharp
public interface IAssetProvider
{
    UniTask<TAsset> LoadAsync<TAsset>(
        string address,
        CancellationToken cancellationToken)
        where TAsset : UnityEngine.Object;

    void Release<TAsset>(TAsset asset)
        where TAsset : UnityEngine.Object;
}
```

Implementation belongs to Infrastructure:

```text
AddressablesAssetProvider
```

---

# Feature-Specific Providers

Feature-specific asset needs should use feature-specific ports.

Example:

```text
Application/Ports/IInventoryIconProvider.cs

Infrastructure/Addressables/AddressablesInventoryIconProvider.cs
```

This avoids leaking generic asset loading into ViewModels.

Good:

```text
InventoryViewModel
    ↓
IInventoryIconProvider
```

Acceptable:

```text
InventoryViewModel
    ↓
IAssetProvider
```

for simple UI cases.

Best for large features:

```text
InventoryViewModel
    ↓
Inventory Presentation Service
    ↓
IInventoryIconProvider
```

---

# Loading From ViewModels

ViewModels should not directly use Addressables.

Allowed:

```text
ViewModel
    ↓
Presentation Asset Service
```

or:

```text
ViewModel
    ↓
UseCase
    ↓
Application Port
```

Forbidden:

```csharp
Addressables.LoadAssetAsync<Sprite>("Inventory/Icons/Sword");
```

inside ViewModel.

---

# Loading From Views

Views should avoid direct Addressables loading.

Allowed exceptions:

* Pure visual-only component.
* No business meaning.
* No testability impact.
* Asset lifetime is clearly local.

Even then, prefer a UI asset service or factory.

---

# Loading From Domain

Forbidden.

Domain must not know:

```text
Addressables

Sprites

Textures

GameObjects

AssetReference

Resource paths
```

Domain should use stable identifiers only.

Example:

```text
ItemDefinition.IconId
```

not:

```text
ItemDefinition.IconSprite
```

---

# Loading From Application

Application should not directly call Addressables.

Application may define ports.

Example:

```text
IInventoryIconProvider

IStaticDataProvider

IHeroDefinitionProvider
```

Infrastructure implements them using Addressables.

---

# Scene Loading

Addressable scenes should be loaded through a scene loading abstraction.

Preferred:

```text
Router
    ↓
ISceneLoader
    ↓
AddressablesSceneLoader
```

Forbidden:

```text
ViewModel
    ↓
Addressables.LoadSceneAsync
```

Scene navigation belongs to Router/SceneLoader.

---

# Prefab Loading

Prefab loading should be done through factories.

Preferred:

```text
UnitFactory
    ↓
IAssetProvider
```

or:

```text
HeroViewFactory
    ↓
IAssetProvider
```

Avoid arbitrary prefab loading from random MonoBehaviours.

---

# Memory Management

Every loaded asset must have clear ownership.

Owner examples:

```text
Screen

Scene

Factory

Asset Cache

Feature Service
```

The owner is responsible for release.

Do not load assets without a release strategy.

---

# Release Rules

If a service loads an asset, the same service should release it.

Good:

```text
InventoryIconProvider.Load
InventoryIconProvider.Release
```

Bad:

```text
ViewModel loads asset
View releases asset
```

Ownership becomes unclear.

---

# Caching

Caching is allowed in Infrastructure.

Examples:

```text
Icon Cache

Definition Cache

Prefab Cache
```

Cache lifetime must be explicit:

```text
Application Scope

Scene Scope

Screen Scope
```

Avoid permanent caches unless assets are truly global.

---

# Preloading

Preloading is allowed for:

```text
Core UI

Common Fonts

Common Icons

Critical Gameplay Assets

Scene Dependencies
```

Preloading should be explicit.

Avoid hidden preloading from random components.

---

# Remote Content

Remote content must be isolated.

Use dedicated groups:

```text
RemoteContent

Remote_BattlePass

Remote_Events
```

Do not mix critical local startup assets with optional remote content.

Startup should not depend on remote content unless explicitly required.

---

# Static Data

Static data may be loaded through Addressables.

Examples:

```text
HeroDefinitions

ItemDefinitions

BattleBalance

QuestDefinitions
```

Access should go through Application ports.

Example:

```text
IHeroDefinitionProvider
```

Implementation:

```text
AddressablesHeroDefinitionProvider
```

Domain should receive parsed domain objects or value objects, not Addressables handles.

---

# Error Handling

Asset loading can fail.

Every asset-loading abstraction should define failure behavior.

Possible strategies:

```text
Return Result<T>

Throw controlled exception

Fallback asset

Placeholder asset
```

UI should display fallback state.

Do not silently ignore missing assets.

---

# Cancellation

Async asset loading should support cancellation.

Examples:

```text
Screen closed

Scene unloaded

Route changed

Application shutdown
```

If loading belongs to a screen, cancellation belongs to screen lifetime.

---

# Addressable Keys

Avoid magic strings scattered across code.

Good:

```text
InventoryAssetKeys.SwordIcon
```

or:

```text
InventoryIconId.Sword
```

Bad:

```csharp
"Inventory/Icons/Sword"
```

inside arbitrary classes.

Centralize keys per feature.

---

# Testing

Application and Domain tests should not require Addressables.

Use fake providers.

Examples:

```text
FakeAssetProvider

FakeInventoryIconProvider

FakeHeroDefinitionProvider
```

Runtime tests may validate actual Addressables loading.

---

# Runtime Tests

Runtime Addressables tests should verify:

```text
Critical assets load

Scene assets load

Feature groups are valid

Missing assets fail predictably

Release logic works
```

Keep these tests limited.

They are slower than pure tests.

---

# Forbidden Patterns

Avoid:

```text
Addressables.LoadAssetAsync in ViewModel

Addressables.LoadAssetAsync in Domain

Addressables.LoadAssetAsync in UseCase

Magic address strings everywhere

Assets loaded without release ownership

Remote content required for basic startup

Feature code depending on physical asset paths
```

---

# Review Checklist

Before adding Addressables usage ask:

1. Which layer owns this load?
2. Who releases the asset?
3. Is there an abstraction?
4. Is the key stable?
5. Is loading cancellable?
6. Is failure handled?
7. Can tests use a fake provider?
8. Is this asset grouped correctly?

If any answer is unclear:

Do not add direct Addressables usage.

---

# Summary

Use this rule:

```text
Addressables load assets.

Infrastructure owns Addressables.

Application defines ports.

Presentation consumes prepared assets.

Domain knows only stable identifiers.
```

Addressables are powerful.

Keep them behind boundaries.
