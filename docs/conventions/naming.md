# Naming Conventions

## Purpose

This document defines naming conventions used throughout the project.

The goals are:

* Consistency.
* Discoverability.
* Predictability.
* Reduced cognitive load.

A developer should be able to determine an object's responsibility from its name.

---

# General Rules

Use:

```text
PascalCase
```

for:

```text
Classes

Interfaces

Enums

Properties

Methods

Namespaces
```

Examples:

```text
InventoryScreen

InventoryViewModel

EquipItemUseCase

InventoryItemEquippedEvent
```

---

# Variables

Use:

```text
camelCase
```

Examples:

```text
inventoryItems

selectedHero

currentProfile
```

---

# Interfaces

Prefix interfaces with:

```text
I
```

Examples:

```text
IInventoryRepository

IPlayerProgressStorage

IAppRouter

IScreenFactory
```

---

# Feature Names

Feature names should represent business capabilities.

Good:

```text
Inventory

Shop

Profile

Matchmaking

Battle

Guilds

Friends
```

Bad:

```text
Managers

Systems

Utils

CoreStuff
```

---

# Domain Entities

Use nouns.

Examples:

```text
Inventory

Item

Hero

Guild

Quest
```

Avoid:

```text
InventoryEntity

HeroObject

QuestModel
```

The namespace already provides context.

---

# Value Objects

Use domain language.

Examples:

```text
Currency

Price

HeroId

PlayerId

GuildId
```

Avoid:

```text
CurrencyValueObject

PriceVo
```

---

# Domain Services

Use:

```text
<ServiceName>DomainService
```

Examples:

```text
MatchmakingDomainService

BattleResolutionDomainService
```

Use only when behavior does not belong to an Entity or Value Object.

---

# Domain Events

Use:

```text
<EventName>DomainEvent
```

Examples:

```text
ItemEquippedDomainEvent

QuestCompletedDomainEvent

HeroLevelUpDomainEvent
```

Domain Events belong to Domain.

---

# Use Cases

Use:

```text
<Action><Entity>UseCase
```

Examples:

```text
EquipItemUseCase

PurchaseProductUseCase

CreateGuildUseCase

JoinMatchUseCase
```

UseCase names should describe business actions.

---

# Commands

Use:

```text
<Action><Entity>Command
```

Examples:

```text
EquipItemCommand

PurchaseProductCommand

RenameGuildCommand
```

Commands describe intentions.

---

# Queries

Use:

```text
<Get><Entity>Query
```

Examples:

```text
GetInventoryItemsQuery

GetGuildMembersQuery

GetPlayerProfileQuery
```

Queries return data.

---

# Request Models

Use:

```text
<UseCaseName>Request
```

Examples:

```text
EquipItemRequest

PurchaseProductRequest

CreateGuildRequest
```

---

# Response Models

Use:

```text
<UseCaseName>Response
```

Examples:

```text
EquipItemResponse

PurchaseProductResponse

CreateGuildResponse
```

---

# Repositories

Use:

```text
I<Entity>Repository
```

Examples:

```text
IInventoryRepository

IQuestRepository

IGuildRepository
```

Implementations:

```text
InventoryRemoteRepository

InventoryLocalRepository

InventoryCachedRepository
```

Avoid:

```text
InventoryManager

InventoryService
```

when repository semantics are intended.

---

# API Clients

Use:

```text
<Entity>ApiClient
```

Examples:

```text
InventoryApiClient

GuildApiClient

ProfileApiClient
```

---

# Gateways

Use:

```text
<Entity>Gateway
```

Examples:

```text
PaymentsGateway

AnalyticsGateway

NotificationsGateway
```

Use when integrating external systems.

---

# Screen Names

Use:

```text
<Feature>Screen
```

Examples:

```text
InventoryScreen

ProfileScreen

ShopScreen
```

---

# View Names

Use:

```text
<Feature>View
```

Examples:

```text
InventoryView

ProfileView

ShopView
```

---

# ViewModel Names

Use:

```text
<Feature>ViewModel
```

Examples:

```text
InventoryViewModel

ProfileViewModel

ShopViewModel
```

---

# Screen State Names

Use:

```text
<Feature>ScreenState
```

Examples:

```text
InventoryScreenState

ProfileScreenState

ShopScreenState
```

---

# Screen Factory Names

Use:

```text
<Feature>ScreenFactory
```

Examples:

```text
InventoryScreenFactory

ProfileScreenFactory

ShopScreenFactory
```

---

# UI Components

Use nouns.

Examples:

```text
InventoryItemCard

HeroPortrait

GuildMemberRow

QuestProgressBar
```

---

# UXML Files

Use:

```text
<ScreenName>.uxml
```

Examples:

```text
InventoryScreen.uxml

ProfileScreen.uxml
```

Components:

```text
InventoryItemCard.uxml

HeroPortrait.uxml
```

---

# USS Files

Use:

```text
<ScreenName>.uss
```

Examples:

```text
InventoryScreen.uss

ProfileScreen.uss
```

---

# Routes

Use destination names.

Good:

```text
Inventory

Profile

Shop

Guild
```

Bad:

```text
OpenInventory

NavigateToProfile

ShowShop
```

Routes identify destinations.

---

# Events

Events must use past tense.

Good:

```text
InventoryItemEquippedEvent

ProfileUpdatedEvent

PurchaseCompletedEvent

QuestCompletedEvent
```

Bad:

```text
EquipItemEvent

UpdateProfileEvent

CompletePurchaseEvent
```

Events describe facts.

---

# MessagePipe Handlers

Use:

```text
<EventName>Handler
```

Examples:

```text
InventoryItemEquippedHandler

PurchaseCompletedHandler
```

---

# Installers

Use:

```text
<Feature>ModuleInstaller
```

Examples:

```text
InventoryModuleInstaller

ProfileModuleInstaller

BattleModuleInstaller
```

---

# Entry Points

Use:

```text
<Scene>EntryPoint
```

Examples:

```text
BattleSceneEntryPoint

WorldMapSceneEntryPoint

MainMenuSceneEntryPoint
```

---

# Assemblies

Use:

```text
Company.Game.Feature.Inventory.Domain

Company.Game.Feature.Inventory.Application

Company.Game.Feature.Inventory.Infrastructure

Company.Game.Feature.Inventory.Presentation
```

Assembly names should match ownership.

---

# Namespaces

Use:

```csharp
namespace Company.Game.Features.Inventory.Domain
{
}
```

Structure:

```text
Company.Game.Features.<Feature>.<Layer>
```

Examples:

```text
Company.Game.Features.Inventory.Domain

Company.Game.Features.Inventory.Application

Company.Game.Features.Inventory.Infrastructure

Company.Game.Features.Inventory.Presentation
```

---

# ScriptableObjects

Use:

```text
<Feature><Purpose>Asset
```

Examples:

```text
InventoryConfigAsset

BattleBalanceAsset

StartupRouteAsset
```

---

# Addressables

Use feature-oriented paths.

Good:

```text
Inventory/Icons/Sword

Profile/Avatars/Knight

Battle/VFX/Explosion
```

Bad:

```text
Assets1

Icons2

TempFolder
```

---

# Test Names

Use:

```text
<ClassName>Tests
```

Examples:

```text
EquipItemUseCaseTests

InventoryViewModelTests

InventoryRepositoryTests
```

Method names:

```text
Should_Return_Error_When_Item_Not_Found

Should_Equip_Item_When_Requirements_Are_Met
```

---

# Forbidden Names

Avoid:

```text
Manager

Helper

Utils

Common

Misc

Stuff

Data

Object

EntityModel

Vo

DtoModel
```

These names usually hide unclear responsibilities.

---

# Review Checklist

Before naming something ask:

1. Does the name describe responsibility?
2. Does the name describe ownership?
3. Does the name use project terminology?
4. Would a new developer understand it?
5. Does the name follow existing conventions?

If any answer is no:

Rename before committing.

---

# Summary

Use this rule:

```text
Names should describe responsibility.

Names should describe ownership.

Names should use business language.

Avoid generic technical names.
```
