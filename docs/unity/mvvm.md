# MVVM

## Purpose

This document defines how MVVM is implemented in the project.

The project uses:

```text
UI Toolkit + MVVM
```

MVVM is the standard Presentation pattern.

All new screens should follow this approach.

---

# Goals

MVVM is used to achieve:

* Separation of concerns.
* Testable UI logic.
* Explicit state management.
* Reduced View complexity.
* Consistent screen implementation.

---

# High-Level Flow

```text
User
    ↓
View
    ↓
ViewModel
    ↓
UseCase
    ↓
Application
    ↓
Domain
```

Data flows back through:

```text
Domain
    ↓
Application
    ↓
ViewModel
    ↓
View
    ↓
User
```

---

# MVVM Components

Every screen consists of:

```text
View

ViewModel

ScreenState
```

Optional:

```text
Commands

Navigation Models

Item ViewModels
```

---

# View

The View is the UI Toolkit implementation.

Contains:

```text
UXML

USS

VisualElement bindings

UI callbacks
```

Responsibilities:

* Render state.
* Receive user input.
* Bind ViewModel.

View must remain thin.

---

# View Responsibilities

Allowed:

```text
Button Clicks

Binding

Visual Updates

Animations

Focus Management
```

Forbidden:

```text
Business Rules

Persistence

Networking

Repository Access

Application Logic
```

Views should not make decisions.

Views display decisions.

---

# ViewModel

ViewModel owns presentation logic.

Responsibilities:

```text
Presentation State

UI Actions

UseCase Calls

Navigation Requests

State Transformation
```

ViewModel is the brain of the screen.

---

# ViewModel Responsibilities

Allowed:

```text
Call UseCases

Manage Loading State

Manage Error State

Transform DTOs

Build Presentation Models
```

Forbidden:

```text
Business Rules

Repository Access

Direct Infrastructure Calls

Scene Management

Addressables Access
```

ViewModels communicate with Application.

Not Infrastructure.

---

# ScreenState

Every non-trivial screen should expose explicit state.

Example:

```text
Loading

Loaded

Empty

Error
```

State should be easy to inspect.

State should be easy to test.

---

# Why ScreenState Exists

Bad:

```text
IsLoading

HasItems

HasError

ShowRetry

ShowList

ShowEmpty
```

Multiple booleans create invalid combinations.

Good:

```text
Loading

Loaded

Empty

Error
```

One state.

One meaning.

---

# Example State

```text
InventoryScreenState
```

Possible values:

```text
Loading

Loaded

Empty

Error
```

The UI reacts to state.

The UI should not derive state itself.

---

# Commands

Commands represent user intentions.

Examples:

```text
EquipItem

PurchaseItem

DeleteDeck

RetryLoading
```

Commands belong to ViewModel.

Commands trigger UseCases.

---

# Use Case Integration

Preferred:

```text
View
    ↓
ViewModel
    ↓
UseCase
```

Example:

```text
Equip Button
    ↓
InventoryViewModel
    ↓
EquipItemUseCase
```

The View never calls UseCases directly.

---

# Use Case Ownership

ViewModels may depend on:

```text
UseCases

Queries

Commands

Application Services
```

ViewModels should not depend on:

```text
Repositories

API Clients

Save Systems

Analytics SDKs
```

These belong to Infrastructure.

---

# DTO Transformation

Application returns DTOs.

ViewModel transforms them into presentation models.

Example:

```text
InventoryItemDto
    ↓
InventoryItemViewModel
```

Purpose:

* Formatting.
* Localization.
* UI-specific state.

The View should not transform DTOs.

---

# Item ViewModels

Complex lists may use child ViewModels.

Example:

```text
InventoryViewModel
    ↓
InventoryItemViewModel
```

Responsibilities:

```text
Formatted Name

Formatted Description

Icon State

Selection State
```

Child ViewModels belong to Presentation.

---

# Navigation

Navigation should happen through Router.

Preferred:

```text
ViewModel
    ↓
Router
    ↓
ScreenHost
```

Avoid:

```text
ViewModel
    ↓
Instantiate Screen
```

Navigation must be centralized.

---

# Navigation Requests

Good:

```text
OpenProfile

OpenInventory

OpenShop
```

ViewModel requests navigation.

Router executes navigation.

---

# Async Flow

Most screens load data asynchronously.

Preferred:

```text
Open Screen
    ↓
Loading
    ↓
Load Data
    ↓
Loaded
```

State transitions should be explicit.

---

# Loading Flow Example

```text
Screen Open
    ↓
Loading State
    ↓
Load Inventory
    ↓
Loaded State
```

Failure:

```text
Screen Open
    ↓
Loading State
    ↓
Load Inventory
    ↓
Error State
```

---

# Error Handling

Errors should be represented by state.

Bad:

```text
Throw Exception
```

inside View.

Preferred:

```text
Error State
```

View reacts to state.

---

# Empty State

Empty data is not an error.

Preferred:

```text
Loading

Loaded

Empty

Error
```

Explicit Empty state improves UX.

---

# ViewModel Lifetime

ViewModel lifetime matches screen lifetime.

```text
Screen Open
    ↓
ViewModel Created
    ↓
Screen Close
    ↓
ViewModel Destroyed
```

ViewModels should not be global singletons.

---

# Dependency Injection

ViewModels should be created through factories.

Preferred:

```text
ScreenFactory
    ↓
ViewModel
```

Avoid:

```text
new InventoryViewModel()
```

inside arbitrary code.

---

# Events

ViewModels may subscribe to application events.

Examples:

```text
InventoryChangedEvent

ProfileUpdatedEvent
```

Purpose:

```text
Refresh UI

Refresh State

Update Lists
```

---

# Event Rules

Allowed:

```text
Application Events
```

Forbidden:

```text
UI Events as Global Events
```

Example:

Bad:

```text
EquipButtonClickedEvent
```

Good:

```text
InventoryItemEquippedEvent
```

Events should represent facts.

---

# MessagePipe Usage

ViewModels may subscribe to:

```text
Application Events
```

ViewModels should not become event buses.

Use events sparingly.

Prefer direct UseCase calls.

---

# Formatting

Formatting belongs to ViewModel.

Examples:

```text
Currency

Date

Time

Localized Text
```

Good:

```text
12 500 Gold
```

prepared by ViewModel.

Bad:

Formatting inside View.

---

# Localization

ViewModel may expose localized strings.

View should not construct localization logic.

Localization should remain centralized.

---

# Selection State

Selection belongs to ViewModel.

Examples:

```text
Selected Item

Selected Tab

Selected Hero
```

View displays selection.

ViewModel owns selection.

---

# Search and Filtering

Filtering belongs to ViewModel.

Example:

```text
Inventory Search

Inventory Filter

Inventory Sort
```

ViewModel transforms source data into visible state.

---

# Testing

ViewModels should be easy to test.

Recommended tests:

```text
Loading Tests

Error Tests

Navigation Tests

Filtering Tests

Selection Tests
```

ViewModel tests should not require Unity runtime.

---

# Example Structure

```text
InventoryScreen/
├── InventoryScreen.uxml
├── InventoryScreen.uss
├── InventoryView.cs
├── InventoryViewModel.cs
├── InventoryScreenState.cs
├── InventoryItemViewModel.cs
└── InventoryScreenFactory.cs
```

Everything required by the screen remains together.

---

# Review Checklist

Before creating a ViewModel ask:

1. Does it contain presentation logic?
2. Does it avoid business rules?
3. Does it depend only on Application abstractions?
4. Does it expose explicit state?
5. Can it be tested without Unity?
6. Does it use Router instead of direct navigation?
7. Does it avoid Infrastructure dependencies?

If any answer is no:

Refactor before implementation.

---

# Summary

The project uses MVVM.

Responsibilities:

```text
View
    ↓
Displays State

ViewModel
    ↓
Manages Presentation Logic

UseCase
    ↓
Executes Application Logic

Domain
    ↓
Executes Business Rules
```

Use this rule:

```text
Views render.

ViewModels coordinate.

UseCases execute.

Domain decides.
```
