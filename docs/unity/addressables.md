# Addressables

## Purpose

Addressables are used for Unity asset loading, not for business logic.
They must stay behind abstractions owned by Presentation, Infrastructure, or
composition code.

## Core Rule

Do not call `Addressables.Load*` directly from Domain, Application use cases,
or ViewModels.

Preferred flow:

```text
ViewModel -> UseCase or Presentation Service -> Asset Provider Port
Infrastructure Addressables Adapter -> Addressables
```

Domain and Application should use stable IDs, DTOs, or ports instead of Unity
asset handles.

## Configuration

Addressables configuration belongs to:

```text
Assets/AddressableAssetsData/
```

Do not mix Addressables settings with feature source code.

## Asset Location

Addressable assets may live under project asset folders or feature-owned asset
folders depending on ownership. Keep source code in packages and Unity asset
configuration in the Unity project.

Feature-specific assets should use feature-oriented addresses.

## Groups

Group assets by ownership and loading behavior, for example `Shared_Local`,
`Shared_Remote`, `Inventory_Local`, `Battle_Scenes`, or `UI_Screens`.

Avoid groups that only mirror temporary folder structure.

## Addresses And Labels

Use stable addresses:

```text
inventory/icons/sword_iron
battle/scenes/arena_01
ui/screens/inventory
```

Use labels for loading behavior or ownership: `preload`, `remote`,
`inventory`, `battle`, `ui`.

Do not expose volatile physical paths as public API.

## Asset References

`AssetReference` may be used in Presentation, Infrastructure, composition, and
Unity-authored assets.

Do not store `AssetReference` in Domain models or pass it through Contracts.

## Providers And Factories

All runtime loading should go through project abstractions:

```text
IAssetProvider
IInventoryIconProvider
ISceneLoader
IPrefabFactory
```

Infrastructure implements these abstractions using Addressables.
Factories should own prefab instantiation and release rules.

## Scenes

Addressable scenes should be loaded through `ISceneLoader` or an equivalent
navigation-owned abstraction.

ViewModels request navigation. They do not load scenes directly.

See `docs/architecture/navigation.md` and `docs/unity/scenes.md`.

## Ownership And Release

Every loaded asset needs an owner: application lifetime, scene lifetime, screen
lifetime, or factory-created object lifetime.

The owner that loads or creates an asset must release it or transfer ownership
explicitly.

Do not load assets without a release strategy.

## Caching And Preloading

Cache lifetime must be explicit. Preload only assets required for a known route,
scene, or screen.

Do not hide long-lived caches in ViewModels or static helpers.

## Remote Content

Remote content must be isolated from critical startup unless explicitly
required.

Startup should have a local fallback or a clear failure path.

## Error Handling And Cancellation

Asset providers should define behavior for missing assets, failed downloads,
and cancellation.

Screen-owned loads cancel when the screen closes. Scene-owned loads cancel when
the scene unloads.

UI should display fallback or error state instead of silently ignoring failures.

## Testing

Domain and Application tests should not require Addressables.
Use fakes for asset provider ports.

Runtime tests should cover critical Addressables groups, scene loads, release
paths, and missing-asset behavior.

## Review Checklist

Before adding Addressables usage, verify:

1. It is behind a project abstraction.
2. Domain, Application, and ViewModels do not call Addressables directly.
3. Address and group names are stable.
4. Load ownership and release are explicit.
5. Failure and cancellation behavior is defined.
