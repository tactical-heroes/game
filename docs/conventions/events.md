# Events

## Purpose

Events decouple observers from facts that already happened.
They must not replace direct dependencies, use cases, queries, or navigation.

## Core Rule

Use an event when:

* The sender should not know receivers.
* The operation already succeeded.
* Multiple independent subscribers may react.
* The fact is meaningful outside the immediate call stack.

Do not use an event for request/response, button clicks, required ordering, or
critical orchestration.

## Event Types

| Type           | Location          | Scope                     |
| -------------- | ----------------- | ------------------------- |
| Domain Event   | Feature Domain    | Internal business fact    |
| Contract Event | Feature Contracts | Cross-feature public fact |

Domain entities must not publish directly to MessagePipe. Application collects
or handles domain events and publishes contract events only when needed.

## Commands, Queries, Navigation

Use the right tool:

* Command or UseCase: something must be done.
* Query: data must be returned.
* Router: UI flow must change.
* Event: something already happened and observers may react.

Events do not replace Router calls.

## Naming

Name events as completed facts:

```text
InventoryItemEquippedEvent
ShopPurchaseCompletedEvent
BattleStartedEvent
PlayerLevelChangedEvent
```

Avoid names like `EquipItemEvent`, `OpenScreenEvent`, or
`ButtonClickedGlobalEvent`.

## Payload

Payloads should be small, immutable, and stable.

Allowed:

* IDs and value objects.
* Public DTOs.
* Timestamps when relevant.
* Minimal context needed by subscribers.

Forbidden:

* Views and ViewModels.
* MonoBehaviours or scene objects.
* Infrastructure clients.
* Addressables handles.
* Mutable domain aggregates.

## Publishing

Publish events after the operation succeeds.

Typical flow:

```text
UseCase -> Domain operation -> persistence -> publish fact
```

Do not publish success events before persistence or required state changes are
complete.

## Subscribing

Subscribers should be registered by the feature that owns the reaction.
Subscriber failures must be handled intentionally and must not silently corrupt
application state.

Do not depend on subscriber execution order. If order matters, orchestrate it
inside a UseCase.

## MessagePipe

MessagePipe is configured in application scope by default.
Screen-local interaction should use ViewModel methods, callbacks, or local
screen scope services.

Do not create unrelated message buses per feature without a clear scope reason.

## High-Frequency Data

Do not publish high-frequency gameplay data globally.
Use local streams, dedicated services, polling, or direct references within the
owning scene/system.

## Review Checklist

Before creating an event, verify:

1. It represents a completed fact.
2. The sender does not need a direct receiver.
3. It is not a command, query, or navigation request.
4. The payload is small and layer-safe.
5. The event lives in the correct layer.
6. Subscribers do not rely on hidden ordering.
