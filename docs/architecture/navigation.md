# Navigation

## Purpose

This document defines navigation architecture.

The project uses centralized navigation.

Navigation is owned by:

```text
Router
```

UI elements do not navigate directly.

Screens do not create other screens.

Features do not instantiate UI manually.

---

# Navigation Goals

The navigation system should provide:

* Predictable behavior.
* Centralized routing.
* Explicit navigation contracts.
* Testable navigation logic.
* Consistent screen lifecycle.
* Clear separation between Screens and Scenes.

---

# Core Concepts

The navigation system consists of:

```text
Router

Route

ScreenHost

ScreenFactory

Navigation Payload

Modal Host
```

---

# Navigation Flow

```text
View
    ↓
ViewModel
    ↓
Router
    ↓
ScreenHost
    ↓
ScreenFactory
    ↓
Screen
```

Only Router may initiate navigation.

---

# Screen vs Scene

The most important rule:

```text
Screen ≠ Scene
```

---

# What Is A Screen

A Screen is a routed UI page.

Examples:

```text
Inventory

Shop

Profile

Settings

Friends

Guilds
```

Screens live inside UI Toolkit.

Screens are managed by ScreenHost.

---

# What Is A Scene

A Scene is a Unity runtime environment.

Examples:

```text
MainMenu

WorldMap

Battle

Tutorial
```

Scenes are managed by SceneManager.

---

# Decision Rule

Ask:

```text
Does this represent a game world?
```

If yes:

```text
Scene
```

Examples:

```text
Battle

WorldMap

Tutorial
```

If no:

```text
Screen
```

Examples:

```text
Inventory

Shop

Settings

Profile
```

---

# Router

Router owns navigation.

Responsibilities:

```text
Open Screen

Close Screen

Replace Screen

Open Scene

Open Modal

Navigate Back
```

Router should not know screen internals.

Router should not know view internals.

---

# ScreenHost

ScreenHost owns screen lifecycle.

Responsibilities:

```text
Create Screens

Destroy Screens

Maintain Stack

Activate Screens

Deactivate Screens
```

ScreenHost belongs to Bootstrap.

Exactly one ScreenHost should exist.

---

# Screen Factory

Screens are created through factories.

Example:

```text
InventoryScreenFactory

ShopScreenFactory

ProfileScreenFactory
```

Responsibilities:

```text
Create View

Create ViewModel

Configure Bindings

Configure Lifetime
```

ScreenHost uses factories.

Router never creates screens directly.

---

# Route

Navigation is performed through Routes.

Examples:

```text
Inventory

Shop

Profile

Settings
```

Routes identify destinations.

---

# RouteId

Every route has a unique identifier.

Example:

```text
Inventory

Profile

Shop

BattlePass
```

Prefer strongly typed route identifiers.

Avoid string literals throughout the codebase.

---

# Route Naming

Use feature-oriented naming.

Good:

```text
Inventory

Profile

Shop

BattlePass
```

Bad:

```text
OpenInventory

InventoryPage

InventoryWindow
```

Routes identify destinations.

Not actions.

---

# Navigation Payload

Routes may accept payloads.

Example:

```text
Profile
```

Payload:

```text
PlayerId
```

Example:

```text
HeroDetails
```

Payload:

```text
HeroId
```

Payloads should be immutable.

---

# Navigation Example

```text
Inventory
    ↓
Hero Details
```

Payload:

```text
HeroId
```

Router passes payload.

Screen receives payload.

---

# Back Stack

Screen navigation maintains a stack.

Example:

```text
MainMenu
    ↓
Inventory
    ↓
HeroDetails
```

Stack:

```text
MainMenu

Inventory

HeroDetails
```

Back:

```text
HeroDetails
    ↓
Inventory
```

---

# Replace Navigation

Some routes should replace current routes.

Example:

```text
Login
    ↓
MainMenu
```

After login:

```text
Back → disabled
```

Previous route is removed.

---

# Modal Navigation

Modal windows use a dedicated stack.

Examples:

```text
Confirmation Dialog

Purchase Dialog

Error Dialog

Rename Dialog
```

Modals are not screens.

---

# Modal Host

Modal Host owns modal lifecycle.

Responsibilities:

```text
Create Modal

Destroy Modal

Manage Modal Stack
```

Separate from ScreenHost.

---

# Overlay Navigation

Overlays are temporary UI.

Examples:

```text
Loading Overlay

Toast

Notification

Achievement Popup
```

Overlays are not screens.

Overlays are not modals.

---

# Startup Route

Application startup is route-driven.

Example:

```text
Bootstrap
    ↓
Startup Route
    ↓
Inventory
```

or

```text
Bootstrap
    ↓
Startup Route
    ↓
MainMenu
```

This enables fast debugging.

---

# Development Routes

Development may override startup routes.

Examples:

```text
Inventory

Shop

Battle
```

Useful for:

* UI development.
* QA.
* Designer workflows.

---

# Scene Navigation

Scene navigation is centralized.

Preferred:

```text
Router
    ↓
Scene Service
    ↓
SceneManager
```

Avoid:

```text
ViewModel
    ↓
SceneManager.LoadScene
```

Navigation should remain centralized.

---

# Additive Scenes

Additive scenes are allowed.

Examples:

```text
WorldMap
    ↓
Battle
```

or

```text
WorldMap
    ↓
Dungeon
```

Router remains responsible.

---

# Screen Lifetime

```text
Open Screen
    ↓
Create ViewModel
    ↓
Create View
    ↓
Bind
```

Close:

```text
Destroy View

Dispose ViewModel

Release Scope
```

Navigation owns lifecycle.

---

# Navigation Events

Navigation events may exist.

Examples:

```text
ScreenOpened

ScreenClosed

ModalOpened

ModalClosed
```

Use sparingly.

Avoid building navigation around events.

Router should remain primary.

---

# Deep Linking

Future support:

```text
Profile/Player123

HeroDetails/Hero42

Guild/Alpha
```

Routes should be designed with future deep-link support in mind.

---

# Testing

Navigation should be testable.

Recommended tests:

```text
Route Resolution

Back Stack

Payload Passing

Modal Flow

Startup Routes
```

Navigation logic should not require Unity runtime.

---

# Forbidden Patterns

Avoid:

```text
ScreenManager

UIManager

WindowManager

Global Popup Manager

Static Navigation
```

with overlapping responsibilities.

Use:

```text
Router

ScreenHost

ModalHost
```

instead.

---

# Review Checklist

Before adding navigation ask:

1. Is this a Screen or a Scene?
2. Does Router own navigation?
3. Does ScreenHost own lifecycle?
4. Is payload explicit?
5. Does back navigation work?
6. Is this a modal instead of a screen?
7. Can this be tested?

If any answer is no:

Reconsider the design.

---

# Summary

Navigation follows:

```text
Router
    ↓
ScreenHost
    ↓
ScreenFactory
    ↓
Screen
```

Use this rule:

```text
Screens display content.

Scenes host worlds.

Router controls navigation.

ScreenHost controls lifecycle.
```
