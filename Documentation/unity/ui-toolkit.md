# UI Toolkit And MVVM

## Purpose

UI Toolkit is the default UI technology for new screens. MVVM is the default
screen pattern. Views stay thin. ViewModels own presentation logic. Business
rules stay in Application or Domain. See `Documentation/architecture/navigation.md`.

## Ownership

UI Toolkit and ViewModels belong to Presentation. Presentation may contain
Screens, Views, ViewModels, screen state, screen factories, UXML, USS, and
reusable UI components. Presentation must not contain business rules or
infrastructure adapters.

## Flow

```text
View -> ViewModel -> UseCase -> Domain
ViewModel -> Router
ViewModel -> ScreenState -> View
```

The View binds and forwards user intent. The ViewModel calls UseCases, maps
results to presentation state, and requests navigation through Router.

## Screen Structure

Keep screen files together:

```text
Presentation/Screens/Inventory/
  InventoryScreen.cs
  InventoryView.uxml
  InventoryView.uss
  InventoryViewModel.cs
  InventoryScreenState.cs
  InventoryScreenFactory.cs
```

## Screen vs Scene

A Screen is a routed UI page. A View is the UI Toolkit implementation.
Screens are not Unity scenes. Use `Documentation/unity/scenes.md` when authored Unity
content, cameras, lighting, physics, or scene-local systems are required.

## View

The View may query UI Toolkit elements, bind visual state, forward input to the
ViewModel, and manage purely visual behavior.

The View must not call UseCases directly, contain business rules, decide
navigation flow, or load shared/runtime Addressables directly.

## ViewModel

The ViewModel may call UseCases, transform DTOs into presentation state, expose
commands, request navigation through Router, and own selection, filtering,
sorting, formatting, loading, and error state.

The ViewModel must not contain Domain business rules, depend on Infrastructure
implementations, instantiate Views or scenes, become a global event bus, or be
an application singleton.

## Screen State And Commands

Represent non-trivial screen state explicitly: `Idle`, `Loading`, `Ready`,
`Empty`, `Error`.

Views render state. ViewModels compute state. Errors should be represented in
`ScreenState`, not thrown into the View.

Commands belong to the ViewModel. They validate presentation state, call
UseCases, update screen state, request navigation when needed, and are disabled
while conflicting async work is running.

## Lifetime And DI

ViewModel lifetime matches screen lifetime. Create ViewModels through screen
factories or screen scope composition.

Screen-owned subscriptions, cancellation tokens, and asset handles must be
disposed when the screen closes.

## UXML And USS

UXML defines structure: static layout, named elements, and reusable templates.
USS defines visual style: layout, typography, spacing, colors, and state
classes. Do not put business decisions in UXML or encode feature logic through
fragile style class combinations.

## Responsive Layout

Screens must support Windows, Linux, macOS, Android, and iOS. Use flexible
containers, USS variables, min/max sizes, and size or platform modifiers
instead of one-resolution absolute layouts. Fixed pixel positions are allowed
only for intentionally fixed-format elements.

Handle safe areas, touch targets, keyboard/mouse, and gamepad focus. Verify
compact phone, tablet, and desktop aspect ratios before finishing a screen.

## Components And Design System

Reusable UI belongs under feature-local `Components` until at least two
features need it. Move primitives such as buttons, tabs, inputs, list views,
loading states, and empty states to a shared design system only when ownership
and API are stable.

## Naming

Use names from `Documentation/conventions/naming.md`.

Typical files: `InventoryScreen.cs`, `InventoryView.uxml`,
`InventoryView.uss`, `InventoryViewModel.cs`, `InventoryScreenState.cs`.

Element names should be stable and purpose-based: `items-list`,
`equip-button`, `empty-state`.

## Navigation And Events

Screens and modals are opened through Router and ScreenHost.
Views and ViewModels do not instantiate screens, modals, or scenes directly.

Local UI interactions stay local:

```text
Button -> View -> ViewModel command
```

Publish global events only for completed application facts.

## Preview And Testing

Preview scenes are allowed for fast UI iteration with fake services, but they
must not become production entry points.

Test ViewModels with pure tests and fakes for UseCases, routers, clocks, and
asset providers. Use runtime tests only for UI Toolkit lifecycle, bindings, or
ScreenHost integration that cannot be covered without Unity.

## Review Checklist

Before adding UI or a ViewModel, verify:

1. It belongs to Presentation.
2. It is a screen, not a scene, unless authored Unity content is needed.
3. View is thin and ViewModel owns presentation logic.
4. State is explicit and testable.
5. Navigation goes through Router and ScreenHost.
6. Layout works on target platforms, aspect ratios, safe areas, and inputs.
