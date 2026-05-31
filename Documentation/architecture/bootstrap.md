# Bootstrap

## Purpose

Bootstrap creates the application. Features provide functionality. Scenes
provide runtime content. Screens provide UI.

Bootstrap may know which feature installers exist, but feature packages must
not depend on Bootstrap.

## Responsibilities

Bootstrap owns:

* Production startup path.
* Application lifetime scope.
* Root dependency registration through VContainer.
* Feature installer registration.
* Infrastructure initialization required before first route.
* Startup route resolution.
* Opening the first screen or scene.
* Application-wide runtime services.

Bootstrap must not contain:

* Business rules.
* Gameplay logic.
* Feature-specific UI behavior.
* Domain decisions.
* Scene-local orchestration.

## Startup Flow

```text
Unity Startup
-> Assets/Scenes/Bootstrap/00_Bootstrap.unity
-> AppBootstrapper
-> AppLifetimeScope
-> Dependency Registration
-> Feature Installation
-> Infrastructure Initialization
-> Startup Route Resolution
-> Initial Screen or Scene
```

`00_Bootstrap.unity` must be the first production scene in Build Settings.
It is infrastructure only: not gameplay, not main menu, not a feature scene.

## Lifetime Scopes

Use the smallest lifetime that matches ownership:

* Application scope: routers, messaging, global config, application services.
* Scene scope: scene-local services, factories, controllers, object pools.
* Screen scope: ViewModel, screen state, commands, screen-local subscriptions.

Concrete implementations are wired only in Bootstrap, feature installers, or
scope installers.

See:

* `Documentation/conventions/dependency-injection.md`

## UI And Scenes

Bootstrap owns the global `ScreenHost` or resolves the service that owns it.
UI Toolkit screens are not Unity scenes.

Preferred UI flow:

```text
Bootstrap -> Router -> ScreenHost -> Screen Factory -> Screen
```

Create Unity scenes for runtime environments such as Battle, World Map,
Tutorial, or a scene-backed Main Menu. Use screens for Inventory, Settings,
Profile, Shop, modal windows, and similar UI pages.

See:

* `Documentation/architecture/navigation.md`
* `Documentation/unity/scenes.md`
* `Documentation/unity/ui-toolkit.md`

## Development Routes

Development may support direct route startup for fast iteration:

```text
Bootstrap -> target screen or scene
```

Preview scenes may use fake services, but they must not replace Bootstrap in
production runtime.

## Review Checklist

Before adding code to Bootstrap, verify:

1. It is required to create or wire the application.
2. It is not feature business logic.
3. It cannot live in a feature installer, scene entry point, or screen scope.
4. It does not introduce a global singleton without approval.
5. It preserves the single production startup path.
