# Scenes

## Purpose

This document defines how Unity scenes are used in the project.

Scenes are used for runtime environments, not for every UI screen.

The project separates:

```text
Screen
```

from:

```text
Scene
```

This distinction is mandatory.

---

# Core Rule

```text
Screen ≠ Scene
```

A Screen is a UI page.

A Scene is a Unity runtime environment.

---

# What Is A Screen

A Screen is a routed UI page managed by UI Toolkit.

Examples:

```text
Inventory
Shop
Profile
Settings
Friends
Guilds
BattlePass
```

Screens are managed by:

```text
Router
ScreenHost
ScreenFactory
```

Screens should not be separate Unity scenes by default.

---

# What Is A Scene

A Scene is a Unity-authored runtime environment.

Examples:

```text
Bootstrap
MainMenu
WorldMap
Battle
Tutorial
Preview
```

Scenes are managed by:

```text
SceneLoader
SceneManager
Router
```

Scenes contain Unity-authored objects and runtime environments.

---

# Scene Location

All scenes are stored in:

```text
Assets/Scenes/
```

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
│   └── MainMenu.unity
│
├── Gameplay/
│   ├── WorldMap.unity
│   ├── Battle.unity
│   └── Tutorial.unity
│
└── Preview/
    ├── InventoryPreview.unity
    ├── ShopPreview.unity
    └── BattlePreview.unity
```

Scenes should not be stored inside feature packages.

---

# Bootstrap Scene

The project has one bootstrap scene:

```text
Assets/Scenes/Bootstrap/00_Bootstrap.unity
```

This scene is the first scene in Build Settings.

Responsibilities:

* Create application lifetime.
* Create Composition Root.
* Configure routing.
* Initialize global services.
* Open startup route.

Bootstrap scene is not a gameplay scene.

Bootstrap scene is not a menu scene.

---

# Shell Scene

Optional.

A shell scene may contain long-lived UI and app-level objects.

Example:

```text
Assets/Scenes/Shell/AppShell.unity
```

May contain:

```text
ScreenHost
ModalHost
ToastHost
LoadingOverlayHost
UICamera
EventSystem
```

If these objects already live in Bootstrap scene, Shell scene may be unnecessary.

---

# Menu Scene

Menu scene is allowed when menu requires a scene-level environment.

Example:

```text
Assets/Scenes/Menu/MainMenu.unity
```

Use a scene for Main Menu when it contains:

* Animated 3D background.
* Camera setup.
* Lighting.
* Scene objects.
* Environment effects.

If Main Menu is pure UI, it can be a Screen instead.

---

# Gameplay Scenes

Gameplay scenes represent game worlds.

Examples:

```text
WorldMap.unity
Battle.unity
Tutorial.unity
```

Use scenes for:

* Cameras.
* Lighting.
* Terrain.
* Units.
* Battlefields.
* World objects.
* Scene-specific systems.

---

# Preview Scenes

Preview scenes are used for development and testing.

Examples:

```text
InventoryPreview.unity
ShopPreview.unity
BattlePreview.unity
```

Purpose:

* UI preview.
* Visual validation.
* Designer workflow.
* Runtime testing.
* Fake data scenarios.

Preview scenes may use fake services.

Preview scenes must not become production entry points.

---

# Scene Entry Points

Complex scenes may have local entry points.

Examples:

```text
BattleSceneEntryPoint
WorldMapSceneEntryPoint
MainMenuSceneEntryPoint
```

Scene Entry Point responsibilities:

* Initialize scene-local systems.
* Connect serialized scene references.
* Create scene-local scope.
* Start scene-specific flow.

Scene Entry Points must not recreate application bootstrap.

---

# Scene Scope

Complex scenes may have their own DI scope.

Examples:

```text
Battle Scope
WorldMap Scope
Tutorial Scope
```

Scene scope lifetime:

```text
Scene Load
    ↓
Scene Unload
```

Scene scope contains only scene-local dependencies.

---

# What Belongs In A Scene

Allowed:

```text
Cameras
Lights
Environment
Scene Roots
Scene Entry Points
Serialized References
Scene-specific MonoBehaviours
Gameplay Objects
```

Scenes are allowed to contain authored Unity content.

---

# What Does Not Belong In A Scene

Forbidden:

```text
Business Rules
UseCase Logic
Domain Logic
Repository Logic
Backend Client Logic
Global App Initialization
```

Scenes should not become architecture containers.

---

# Scene Loading

Scene loading must be centralized.

Preferred:

```text
Router
    ↓
SceneLoader
    ↓
Unity SceneManager
```

Avoid:

```text
ViewModel
    ↓
SceneManager.LoadScene
```

or:

```text
Random MonoBehaviour
    ↓
SceneManager.LoadScene
```

Scene navigation belongs to Router/SceneLoader.

---

# Additive Scenes

Additive scenes are allowed.

Use additive scenes when:

* A scene is composed from multiple authored parts.
* Battle loads an additional environment.
* UI shell should remain loaded.
* World map loads local areas.
* Gameplay needs streamed content.

Example:

```text
Bootstrap
    +
AppShell
    +
WorldMap
```

or:

```text
Bootstrap
    +
Battle
    +
BattleEnvironment_Forest
```

---

# Scene Naming

Use PascalCase.

Examples:

```text
00_Bootstrap.unity
MainMenu.unity
WorldMap.unity
Battle.unity
Tutorial.unity
InventoryPreview.unity
```

Numeric prefixes are allowed only for startup/order clarity.

Recommended:

```text
00_Bootstrap.unity
10_MainMenu.unity
20_WorldMap.unity
30_Battle.unity
```

---

# Scene Build Settings

Production scenes should be registered intentionally.

Required:

```text
00_Bootstrap.unity
```

Other scenes may be loaded by Addressables or direct scene references depending on project configuration.

Do not add preview scenes to production build unless explicitly required.

---

# Screens Inside Scenes

Scenes may host screens.

Example:

```text
Battle Scene
    ↓
Battle HUD Screen
    ↓
Pause Menu Screen
```

The scene provides gameplay environment.

The screen provides UI.

---

# Scene To Screen Relationship

Good:

```text
Battle Scene
    contains battle world

Battle HUD Screen
    displays battle UI
```

Bad:

```text
Inventory Scene
    exists only to show Inventory UI
```

Inventory should usually be a Screen.

---

# Startup From Any Scene

Development must support starting from any screen or scenario.

Preferred mechanisms:

```text
StartupRoute
PreviewScene
Editor Play Mode Start Scene
```

Do not bypass Bootstrap for production flows.

---

# Testing Scenes

Scene tests should verify:

```text
Scene loads correctly
Scene scope builds correctly
Scene entry point initializes
Required references are assigned
Scene unload disposes local scope
```

Use Runtime tests for scene lifecycle.

---

# Review Checklist

Before creating a scene ask:

1. Is this a runtime environment?
2. Does it need cameras, lighting, world objects, or authored content?
3. Is this more than a UI page?
4. Should this be a Screen instead?
5. Does this scene need a local EntryPoint?
6. Does this scene need a local Scope?
7. Will this scene be loaded through Router/SceneLoader?

If most answers are no:

Create a Screen, not a Scene.

---

# Summary

Use this rule:

```text
Scenes host worlds.

Screens host UI.

Router controls navigation.

SceneLoader controls scene loading.

ScreenHost controls screen lifecycle.
```

Do not create a Unity scene for every UI page.
