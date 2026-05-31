# Testing

## Purpose

This document defines the testing strategy used throughout the project.

The goals are:

* Fast feedback.
* Reliable refactoring.
* Architecture validation.
* Regression prevention.
* Confidence in long-term development.

Testing is part of the architecture.

Code should be designed to be testable.

---

# Testing Pyramid

The project follows a testing pyramid.

```text
           UI Tests
        Integration Tests
    Application / Feature Tests
          Domain Tests
```

Most tests should be located near the bottom.

Fewer tests should exist near the top.

---

# Test Categories

The project uses:

```text
Domain Tests

Application Tests

Feature Tests

Integration Tests

Runtime Tests

UI Tests
```

Each category has a different purpose.

---

# Test Locations

Every feature package contains:

```text
Tests/
├── Editor/
└── Runtime/
```

---

# Editor Tests

Location:

```text
Tests/Editor/
```

Purpose:

* Fast execution.
* No Unity runtime.
* Pure C# testing.

Editor tests should be preferred whenever possible.

---

# Runtime Tests

Location:

```text
Tests/Runtime/
```

Purpose:

* Unity lifecycle.
* Scene loading.
* UI Toolkit.
* Addressables.
* Runtime integration.

Runtime tests are slower.

Use only when necessary.

---

# Domain Tests

The majority of tests should be Domain Tests.

Purpose:

* Validate business rules.
* Validate invariants.
* Validate calculations.
* Validate state transitions.

Domain tests should:

```text
Run Fast

Require No Unity Runtime

Have No External Dependencies
```

---

# Domain Test Examples

Examples:

```text
Hero Level Progression

Inventory Rules

Currency Rules

Battle Resolution Rules

Quest Completion Rules
```

Domain tests should never require:

```text
MonoBehaviour

Scene

UIDocument

Addressables

Backend
```

---

# Application Tests

Application tests validate use cases.

Examples:

```text
EquipItemUseCase

PurchaseProductUseCase

CreateGuildUseCase
```

Application tests verify:

* Workflow orchestration.
* Repository interaction.
* Event publishing.
* Validation behavior.

---

# Application Test Dependencies

Application tests should use:

```text
Fake Repositories

Fake Services

Fake Gateways
```

Avoid:

```text
Real Backend

Real Save System

Real Analytics
```

Application tests should remain deterministic.

---

# Feature Tests

Feature tests validate a complete feature behavior.

Example:

```text
Inventory Feature

Shop Feature

BattlePass Feature
```

Purpose:

* Validate interactions between layers.
* Validate feature workflows.
* Validate public contracts.

Feature tests may use multiple components together.

---

# Integration Tests

Integration tests validate cooperation between modules.

Examples:

```text
Inventory → Achievements

Battle → Progression

Shop → Inventory
```

Purpose:

* Validate cross-feature communication.
* Validate event flows.
* Validate routing integration.
* Validate startup composition.

Integration tests should remain relatively small.

---

# Runtime Tests

Runtime tests validate Unity-specific behavior.

Examples:

```text
Scene Loading

UI Toolkit Lifecycle

Addressables Loading

Lifetime Scopes
```

Runtime tests may use:

```text
SceneManager

MonoBehaviour

VisualElement

UIDocument
```

---

# UI Tests

UI tests validate:

```text
Bindings

State Changes

Navigation

Modal Flow

Screen Lifecycle
```

UI tests should focus on behavior.

Not visuals.

---

# What To Test

Test:

```text
Business Rules

Use Cases

State Transitions

Validation

Navigation Logic

Event Publishing

Contracts
```

---

# What NOT To Test

Avoid testing:

```text
Simple Getters

Framework Code

Unity Internals

Third-Party Libraries
```

Focus on project behavior.

---

# ViewModel Tests

ViewModels should be tested.

Examples:

```text
Loading State

Error State

Empty State

Navigation Requests

Selection Logic
```

ViewModel tests should not require:

```text
UIDocument

VisualElement

Scene
```

ViewModels should remain plain C# whenever possible.

---

# Screen Tests

Screen tests validate:

```text
Binding

Lifecycle

Open

Close

Modal Flow
```

Screen tests belong to Runtime Tests.

---

# Navigation Tests

Navigation should be tested.

Examples:

```text
Open Screen

Back Navigation

Payload Passing

Modal Stack

Route Resolution
```

Navigation logic should not require manual testing.

---

# Event Tests

Validate:

```text
Correct Event Published

Correct Event Payload

Correct Subscriber Behavior
```

Avoid testing MessagePipe itself.

Test project behavior.

---

# Repository Tests

Infrastructure repositories should be tested.

Examples:

```text
Serialization

Mapping

Caching

Persistence
```

Use isolated environments.

Avoid production services.

---

# Scene Tests

Validate:

```text
Scene Loads

Scene Scope Builds

Required References Exist

Scene Entry Point Initializes
```

Scene tests belong to Runtime Tests.

---

# Preview Scene Testing

Preview scenes exist for:

```text
Designer Validation

UI Development

Visual Inspection
```

Preview scenes are not a replacement for automated tests.

---

# Fake Implementations

Prefer:

```text
FakeInventoryRepository

FakePlayerProgressStorage

FakeRouter
```

Over:

```text
Mocks Everywhere
```

Use fakes when behavior matters.

Use mocks only when interaction verification is required.

---

# Test Naming

Class:

```text
EquipItemUseCaseTests
```

Method:

```text
Should_Equip_Item_When_Requirements_Are_Met

Should_Return_Error_When_Item_Does_Not_Exist
```

Names should describe behavior.

---

# Test Independence

Every test should:

```text
Run Independently

Run In Any Order

Have No Shared State
```

Tests must not depend on previous test execution.

---

# Determinism

Tests should be deterministic.

Avoid:

```text
Random Values

Current Time

Network Calls

External APIs
```

Inject abstractions when needed.

Examples:

```text
IClock

IRandomProvider

IGuidProvider
```

---

# Coverage Philosophy

Do not optimize for coverage percentage.

Optimize for confidence.

Bad goal:

```text
95% Coverage
```

Good goal:

```text
Critical Behavior Protected
```

Coverage is a metric.

Not a goal.

---

# Architecture Validation

Tests should validate architecture assumptions.

Examples:

```text
Feature Boundaries

Dependency Direction

Composition Root Build
```

Architecture tests are encouraged.

---

# CI Requirements

Every Pull Request should execute:

```text
Editor Tests

Application Tests

Domain Tests
```

Recommended:

```text
Runtime Tests
```

for protected branches.

---

# Review Checklist

Before adding a test ask:

1. What behavior am I validating?
2. Can this be an Editor Test?
3. Does this require Unity runtime?
4. Can I replace dependencies with fakes?
5. Is the test deterministic?
6. Does the name describe behavior?

If any answer is no:

Improve the test before merging.

---

# Summary

Use this rule:

```text
Most tests should be Domain and Application tests.

Runtime tests should be used only when Unity lifecycle matters.

Test behavior.

Not implementation details.
```
