# Naming Conventions

## Purpose

Names should reveal responsibility, feature ownership, and layer.
Prefer consistent, boring names over clever abbreviations.

## General C# Rules

| Item                        | Convention       | Example                |
| ---                         | ---              | ---                    |
| Classes, records, structs   | PascalCase       | `InventoryScreen`      |
| Interfaces                  | `I` + PascalCase | `IInventoryRepository` |
| Methods, properties, events | PascalCase       | `EquipItemAsync`       |
| Parameters and locals       | camelCase        | `selectedHero`         |
| Private fields              | `_camelCase`     | `_repository`          |
| Constants                   | PascalCase       | `MaxPartySize`         |
| Async methods               | `Async` suffix   | `LoadInventoryAsync`   |

Avoid unexplained abbreviations and generic names such as `Manager`, `Helper`,
`Utils`, `Data`, or `System`.

## Feature Names

Feature names are business capabilities:

```text
Inventory
Shop
Profile
Battle
Matchmaking
Quests
Settings
```

Do not name features by technical buckets such as `Managers`, `Systems`, or
`CoreLogic`.

## Layer Patterns

| Concept                   | Pattern                      | Example                      |
| ------------------------- | ---------------------------- | ---------------------------- |
| Entity                    | Noun                         | `InventoryItem`              |
| Value Object              | Noun | NounValue             | `ItemId`, `GoldAmount`       |
| Domain Service            | Noun + `DomainService`       | `LootRollDomainService`      |
| Domain Event              | PastFact + `Event`           | `InventoryItemEquippedEvent` |
|                           |                              |                              |
| Use Case                  | Verb + Noun + `UseCase`      | `EquipItemUseCase`           |
|                           |                              |                              |
| Command                   | Verb + Noun + `Command`      | `EquipItemCommand`           |
| Query                     | Noun + `Query`               | `InventoryItemsQuery`        |
|                           |                              |                              |
| Request                   | Action + `Request`           | `PurchaseItemRequest`        |
| Response                  | Action + `Response`          | `PurchaseItemResponse`       |
|                           |                              |                              |
| Repository Interface      | `I` + Noun + `Repository`    | `IInventoryRepository`       |
| Repository Implementation | Source + Noun + `Repository` | `RemoteInventoryRepository`  |
|                           |                              |                              |
| Gateway Interface         | `I` + Noun + `Gateway`       | `IShopGateway`               |
|                           |                              |                              |
| Client                    | System + `Client`            | `PlayFabInventoryClient`     |

## UI Patterns

| Concept          | Pattern                        | Example                  |
| ---------------- | ------------------------------ | ------------------------ |
| Screen           | Feature/Page + `Screen`        | `InventoryScreen`        |
| View             | Feature/Page + `View`          | `InventoryView`          |
| ViewModel        | Feature/Page + `ViewModel`     | `InventoryViewModel`     |
| Screen State     | Feature/Page + `ScreenState`   | `InventoryScreenState`   |
| Screen Factory   | Feature/Page + `ScreenFactory` | `InventoryScreenFactory` |
| Screen Route     | Feature/Page + `Route`         | `InventoryRoute`         |
| Screen Presenter | Feature/Page + `Presenter`     | `InventoryPresenter`     |
| Screen Installer | Feature/Page + `Installer`     | `InventoryInstaller`     |
| UI Component     | Component + `View`             | `ItemSlotView`           |
| UXML             | Same as View                   | `InventoryView.uxml`     |
| USS              | Same as View                   | `InventoryView.uss`      |

## Navigation And Events

| Concept       | Pattern                     | Example                        |
| ------------- | --------------------------- | ------------------------------ |
| Route Id      | Feature.Destination         | `Inventory.ItemDetails`        |
| Route Payload | Destination + `Route`       | `ItemDetailsRoute`             |
|               |                             |                                |
| Event         | CompletedFact + `Event`     | `ShopPurchaseCompletedEvent`   |
| Handler       | Event + `Handler`           | `ShopPurchaseCompletedHandler` |
|               |                             |                                |
| Installer     | Feature + `ModuleInstaller` | `InventoryModuleInstaller`     |
|               |                             |                                |
| Entry Point   | Scene + `EntryPoint`        | `BattleSceneEntryPoint`        |

## Assemblies And Namespaces

Assembly names use PascalCase segments:

```text
Company.Game.Feature.Inventory.Application
```

Namespaces should align with assemblies:

```csharp
namespace Company.Game.Features.Inventory.Application;
```

Package names use lowercase reverse-DNS:

```text
com.company.game.feature.inventory
```

## Addressables

Use stable, feature-oriented addresses:

```text
inventory/icons/sword_iron
battle/scenes/battle_arena_01
ui/screens/inventory
```

Do not expose volatile physical folder paths as public addresses.

## Tests

Test names should describe behavior:

```text
EquipItem_WhenSlotIsEmpty_EquipsItem
PurchaseItem_WhenBalanceIsLow_ReturnsError
```

## Review Checklist

Before introducing a name, verify:

1. The owner feature is clear.
2. The layer or role is clear.
3. The name is stable enough for public contracts.
4. It avoids generic buckets and abbreviations.
