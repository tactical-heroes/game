# Scenes

## Purpose

Scenes are Unity-authored runtime environments. They are not the default unit
for UI pages.

Navigation rules live in `docs/architecture/navigation.md`.

## Core Rule

```text
Screen != Scene
```

Use screens for routed UI pages. Use scenes for authored runtime environments
with cameras, lighting, physics, placed objects, or scene-local systems.

## Location

Production scenes live in:

```text
Assets/Scenes/
```

Bootstrap scene:

```text
Assets/Scenes/Bootstrap/00_Bootstrap.unity
```

Preview scenes may live under a clearly named preview/dev folder and must not
replace production Bootstrap.

## Scene Types

Common scene types:

* Bootstrap: application startup only.
* Shell: optional long-lived app shell.
* Main Menu: only when it needs scene-authored content.
* Battle/World/Tutorial: gameplay or authored runtime environment.
* Preview: development-only isolated scenario.

Inventory, Settings, Profile, Shop, and dialogs should usually be screens.

## Scene Entry Point

A scene may have an entry point when it needs local initialization.

Entry point responsibilities:

* Connect serialized scene references.
* Create or attach scene-local scope.
* Initialize scene-local systems.
* Start scene-specific flow.

Entry points must not recreate application Bootstrap or application-wide
services.

## Scene Scope

Create a scene scope when the scene owns local dependencies:

* Scene services.
* Scene factories.
* Scene controllers.
* Object pools.
* Scene-local subscriptions.

Destroy the scene scope when the scene unloads.

## What Belongs In A Scene

Allowed:

* Cameras, lights, volumes, and environment objects.
* Scene roots and entry points.
* Serialized references to scene objects.
* Scene-specific MonoBehaviours.
* Presentation adapters for scene UI.

## What Does Not Belong In A Scene

Forbidden:

* Domain business rules.
* Application use case logic.
* Infrastructure adapter implementation details.
* Global service initialization.
* Feature package source code.

## Loading

Scene loading must be centralized behind Router and SceneLoader abstractions.

ViewModels request navigation; they do not call Unity scene loading APIs.

Additive scenes are allowed when the runtime environment is composed from
multiple authored parts or the UI shell must remain loaded.

## Naming

Use stable, ordered names when useful:

```text
00_Bootstrap
10_MainMenu
20_WorldMap
30_Battle
Preview_BattleArena
```

Do not encode temporary task names in scene names.

## Build Settings

`00_Bootstrap.unity` is the first production scene.
Add other production scenes intentionally.
Do not add preview scenes to production builds unless explicitly required.

## Startup From Any Scene

Development can support direct play from preview or target scenes, but
production flows must still go through Bootstrap.

## Testing

Runtime tests should cover critical scene load/unload, scope creation,
entry-point initialization, and disposal behavior.

## Review Checklist

Before creating a scene, verify:

1. A screen is not enough.
2. The scene needs authored Unity content.
3. Startup still goes through Bootstrap.
4. Loading goes through Router or SceneLoader.
5. Scene-local scope and disposal are clear.
