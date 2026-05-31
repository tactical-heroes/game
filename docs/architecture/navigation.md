# Navigation

## Purpose

Navigation defines how the application opens screens, modals, overlays, and
scene-backed flows.

UI Toolkit screen implementation lives in `docs/unity/ui-toolkit.md`.
Scene rules live in `docs/unity/scenes.md`.

## Core Concepts

* Route: stable identifier plus optional payload.
* Router: application service that accepts navigation requests.
* ScreenHost: presentation service that owns screen stack and modal host.
* ScreenFactory: feature-owned factory that creates a screen and its scope.
* SceneLoader: abstraction for Unity scene loading when a route needs a scene.

## Screen vs Scene

Use a screen for UI pages:

* Inventory
* Settings
* Profile
* Shop
* Pause Menu
* Modal dialogs

Use a Unity scene for authored runtime environments:

* Bootstrap
* Battle
* World Map
* Tutorial
* Scene-backed Main Menu

Rule: if it is mostly UI, make it a screen. If it needs authored Unity objects,
lighting, cameras, physics, or scene-local systems, make it a scene.

## Navigation Flow

```text
View -> ViewModel -> Router -> ScreenHost -> ScreenFactory -> Screen
```

Views do not instantiate screens. ViewModels request navigation through a
router abstraction. Router does not know screen internals.

Scene-backed flow:

```text
ViewModel -> Router -> SceneLoader -> Scene Entry Point -> ScreenHost
```

## Routes

Route names should be stable and feature-oriented:

```text
Inventory.Main
Inventory.ItemDetails
Shop.Main
Battle.Loadout
Battle.Scene
Settings.Audio
```

Payloads should be small and serializable when possible:

```csharp
public readonly record struct InventoryItemDetailsRoute(ItemId ItemId);
```

Do not pass ViewModels, Views, scene objects, or infrastructure handles as
payload.

## Back Stack

The Router owns navigation semantics:

* Push opens a new screen.
* Replace swaps the current screen.
* Back closes the current screen when allowed.
* Modal opens inside the modal host.
* Overlay opens for app-level transient UI.

Screens may expose `CanClose` or a similar guard when unsaved state exists.

## Transitions

Screen, modal, overlay, and scene-backed transitions should be smooth.

Use Router, ScreenHost, and SceneLoader as the ownership boundary for
transitions. Views may animate visual entry and exit state, but they must not
decide navigation flow.

Use PrimeTween for UI and gameplay tweens. Transitions must preserve
cancellation, loading, and error behavior.

## Startup Routes

Bootstrap resolves the startup route and asks the Router to open it.
Development routes may target a screen or scene directly, but still go through
Bootstrap.

## Screen Lifetime

Screen lifetime starts when the route is opened and ends when the screen is
closed. Its scope owns:

* ViewModel.
* Screen state.
* Screen-local subscriptions.
* Screen-owned asset handles.

## Events

Do not use global events for normal navigation. Use Router calls.
Publishing a navigation-completed event is acceptable only when other systems
need to observe a completed fact.

## Testing

Test routing decisions without Unity scene loading where possible.
Use integration or runtime tests for SceneLoader and ScreenHost behavior.

## Forbidden Patterns

Avoid:

* View creating screens directly.
* ViewModel instantiating Unity scenes.
* Route payload containing UI or infrastructure objects.
* One Unity scene per UI page.
* Global event bus used as a router.

## Review Checklist

Before adding a route, verify:

1. It is named by feature and destination.
2. Its payload is minimal and stable.
3. It uses a screen unless a scene is actually required.
4. Screen creation is delegated to a feature factory.
5. Bootstrap remains the production entry point.
6. Transitions are smooth and owned by navigation or screen code.
