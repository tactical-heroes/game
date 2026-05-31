# UI Toolkit

## Purpose

This document defines how UI is implemented in the project.

The project uses:

```text
Unity UI Toolkit
```

UI Toolkit is the standard UI framework for the project.

Legacy UI systems should not be used for new development.

---

# UI Architecture

UI belongs to:

```text
Presentation
```

UI Toolkit is considered a Presentation technology.

UI must not contain:

* Business rules.
* Persistence logic.
* Networking logic.
* Domain logic.

UI is responsible only for presenting state and collecting user input.

---

# UI Hierarchy

The application UI follows:

```text
ScreenHost
    ↓
Screen
    ↓
View
    ↓
UI Toolkit Elements
```

---

# Core Concepts

The project uses the following concepts:

```text
Screen

View

ViewModel

ScreenState

ScreenFactory

ScreenHost

Router
```

Each has a specific responsibility.

---

# Screen

A Screen represents a complete user-facing page.

Examples:

```text
Inventory

Shop

Profile

Settings

BattlePass

Matchmaking
```

A Screen is not a Unity Scene.

A Screen is a routed UI page.

---

# View

A View is a UI Toolkit implementation of a Screen.

Contains:

```text
UXML

USS

VisualElement bindings

UI callbacks
```

Responsibilities:

* Display data.
* Collect user input.
* Bind ViewModel state.

Views should remain thin.

---

# ViewModel

ViewModels belong to Presentation.

Responsibilities:

* Expose UI state.
* Handle UI actions.
* Call Use Cases.
* Transform application data into presentation data.

ViewModels do not contain business rules.

Detailed rules:

```text
docs/unity/mvvm.md
```

---

# Screen State

Every complex screen should expose explicit state.

Example:

```text
Loading

Empty

Loaded

Error
```

Avoid implicit UI state.

State should be easy to inspect and test.

---

# Screen Factory

Screens should be created through factories.

Example:

```text
InventoryScreenFactory

ShopScreenFactory

ProfileScreenFactory
```

Responsibilities:

* Create View.
* Create ViewModel.
* Configure bindings.
* Configure lifecycle.

Factories own screen construction.

---

# Screen Host

The application contains a single Screen Host.

Responsibilities:

* Open screens.
* Close screens.
* Maintain navigation stack.
* Manage modals.
* Manage overlays.

Screen Host belongs to Bootstrap.

---

# Router

The Router owns navigation.

Responsibilities:

```text
Open Screen

Close Screen

Back

Forward

Pass Navigation Payload
```

Router should not know screen internals.

Router communicates with Screen Host.

---

# Screen Structure

Recommended structure:

```text
Presentation/
└── UiToolkit/
    └── Screens/
        └── Inventory/
            ├── InventoryScreen.uxml
            ├── InventoryScreen.uss
            ├── InventoryView.cs
            ├── InventoryViewModel.cs
            ├── InventoryScreenState.cs
            └── InventoryScreenFactory.cs
```

All screen files should remain together.

---

# Why Co-Locate Files

Good:

```text
InventoryScreen/
├── InventoryScreen.uxml
├── InventoryScreen.uss
├── InventoryView.cs
├── InventoryViewModel.cs
```

Bad:

```text
UXML/
USS/
Views/
ViewModels/
```

spread across the entire project.

Screen ownership is more important than file type.

---

# Component Structure

Reusable UI belongs in Components.

Example:

```text
Presentation/
└── UiToolkit/
    └── Components/
        └── InventoryItemCard/
            ├── InventoryItemCard.uxml
            ├── InventoryItemCard.uss
            ├── InventoryItemCardView.cs
            └── InventoryItemCardViewModel.cs
```

Components should remain self-contained.

---

# Design System

Shared UI belongs to the Design System.

Example:

```text
Foundation/
└── Presentation/
    └── UiToolkit/
        ├── Components/
        ├── Themes/
        └── Styles/
```

Contains:

```text
Buttons

Inputs

Dropdowns

Modal Windows

Loading Indicators

Typography

Theme Definitions
```

Feature-specific UI should not be moved into the Design System.

---

# UXML Rules

UXML defines structure.

Responsibilities:

```text
Hierarchy

Slots

Containers

Controls
```

UXML should not contain business logic.

Prefer readable names.

Example:

```xml
<ui:Button name="equip-button" />
```

Good.

Avoid:

```xml
<ui:Button name="button1" />
```

---

# USS Rules

USS defines appearance.

Responsibilities:

```text
Layout

Spacing

Typography

Colors

Animations
```

USS should not contain feature-specific logic.

Prefer design tokens and shared styles where appropriate.

---

# Naming Rules

Screen:

```text
InventoryScreen
```

Files:

```text
InventoryScreen.uxml

InventoryScreen.uss

InventoryView.cs

InventoryViewModel.cs

InventoryScreenFactory.cs

InventoryScreenState.cs
```

Component:

```text
InventoryItemCard
```

Files:

```text
InventoryItemCard.uxml

InventoryItemCard.uss

InventoryItemCardView.cs
```

Use PascalCase.

---

# Navigation

Preferred:

```text
Router
    ↓
ScreenHost
    ↓
ScreenFactory
    ↓
Screen
```

Avoid direct screen creation.

Bad:

```text
new InventoryScreen()
```

inside arbitrary code.

Navigation should be centralized.

---

# Modal Windows

Modals should be routed through a Modal Host.

Examples:

```text
Confirmation Dialog

Purchase Dialog

Error Dialog
```

Do not instantiate modals directly.

Use factories and routing.

---

# Loading Screens

Loading overlays should be centralized.

Preferred:

```text
LoadingOverlayHost
```

Avoid custom loading implementations for every screen.

---

# UI State

Every screen should explicitly represent state.

Preferred:

```text
Loading

Loaded

Empty

Error
```

Avoid:

```text
Ten unrelated booleans
```

that implicitly describe state.

---

# Async Operations

UI should never block.

Use:

```text
UniTask
```

or project-approved async abstractions.

ViewModels should manage loading state.

Views should only react to state changes.

---

# Event Handling

Preferred:

```text
Button Click
    ↓
View
    ↓
ViewModel
    ↓
UseCase
```

Avoid:

```text
Button Click
    ↓
Global Event Bus
```

UI actions should not become global events.

---

# Scene Usage

Do not create a scene for every screen.

Good screen candidates:

```text
Inventory

Profile

Shop

Settings
```

Good scene candidates:

```text
Main Menu

World Map

Battle

Tutorial
```

UI Screens and Unity Scenes are different concepts.

---

# Preview Scenes

Complex screens may have Preview Scenes.

Examples:

```text
InventoryPreview

ShopPreview

ProfilePreview
```

Purpose:

* UI development.
* Designer workflow.
* Visual testing.

Preview scenes may use fake services.

---

# Testing

UI should be testable.

Recommended:

```text
ViewModel Tests

Screen Tests

Navigation Tests

Factory Tests
```

Business rules should not require UI tests.

Business rules belong in Domain tests.

---

# Review Checklist

Before creating a new screen ask:

1. Is this a Screen or a Scene?
2. Does it belong to a Feature?
3. Does it have a ViewModel?
4. Does it expose explicit state?
5. Can it be opened through Router?
6. Is screen creation delegated to a Factory?
7. Are UXML and USS co-located?

If any answer is no:

Reconsider the design.

---

# Summary

The UI architecture follows:

```text
Router
    ↓
ScreenHost
    ↓
ScreenFactory
    ↓
Screen
    ↓
View
    ↓
ViewModel
    ↓
UseCases
```

Use this rule:

```text
Screens own UI.

ViewModels own presentation state.

UseCases own behavior.

Domain owns business rules.
```
