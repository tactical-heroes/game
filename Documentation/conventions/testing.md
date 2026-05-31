# Testing

## Purpose

Tests should protect business behavior, architecture boundaries, and Unity
integration points without making the project slow or brittle.

## Test Pyramid

Prefer more tests at the bottom:

```text
Many: Domain and Application unit tests
Some: ViewModel and feature integration tests
Few: Runtime, scene, Addressables, and UI flow tests
```

Domain and Application tests should usually run without Unity runtime.

## Locations

Feature package tests live near the feature:

```text
Tests/
  Editor/
  Runtime/
```

Use Editor tests for pure C# behavior. Use Runtime tests for Play Mode,
scenes, Addressables, MonoBehaviours, and lifecycle behavior.

## What To Test

Prioritize:

* Domain rules and invariants.
* UseCase orchestration.
* Error paths and validation.
* ViewModel state transitions.
* Event publication and subscription behavior.
* Repository adapter contracts with fakes or controlled test data.
* Composition build tests for non-trivial installers.

Avoid testing:

* Unity engine internals.
* Simple getters and setters.
* Visual layout pixel details unless explicitly required.
* Private implementation details.

## Domain Tests

Domain tests should be deterministic, fast, and pure C#.
They must not require scenes, Addressables, DI containers, PlayerPrefs, network,
or Unity runtime.

## Application Tests

Application tests use fake repositories, gateways, routers, clocks, and event
publishers.

Verify that UseCases:

* Call the correct ports.
* Enforce application policies.
* Publish events only after successful state changes.
* Return stable results or errors.

## ViewModel Tests

ViewModel tests verify presentation state:

* Loading, error, empty, and ready states.
* Command behavior.
* Selection, filtering, and formatting.
* Navigation requests through router fakes.

Do not require real Views for ViewModel tests.

## Integration And Runtime Tests

Use integration/runtime tests for:

* Container build validation.
* Scene load and unload lifecycle.
* ScreenHost behavior.
* Addressables load and release behavior.
* MessagePipe registration.

Keep these tests focused and fewer than pure unit tests.

## Test Doubles

Prefer small fakes for Application and Presentation tests.
Use mocks only when interaction verification is important.

Do not use real backend clients, real saves, or remote content in normal tests.

## Naming

Use behavior-oriented names:

```text
MethodOrAction_WhenCondition_ExpectedResult
```

Example:

```text
EquipItem_WhenSlotIsEmpty_EquipsItem
```

## Determinism

Tests should control:

* Time.
* Randomness.
* External data.
* Async completion.
* Scene and asset lifetime.

## CI Requirements

CI should run fast Editor tests by default.
Runtime tests may run in a separate job when they are slower or require Unity
Play Mode.

## Review Checklist

Before merging behavior changes, verify:

1. Business rules have pure tests.
2. UseCases are tested with fakes.
3. ViewModel state transitions are covered when UI behavior changed.
4. Unity lifecycle behavior has runtime coverage or a manual check.
5. Tests do not depend on hidden global state.
