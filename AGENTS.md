## Project Overview

This repository contains a Unity 6 game built using:

* Modular Monolith Architecture
* Feature-first package structure
* UI Toolkit
* MVVM
* DDD where business complexity justifies it
* VContainer
* MessagePipe
* Addressables

The project is organized around feature modules implemented as local Unity packages.

Business logic, UI, infrastructure, and application orchestration are isolated inside feature packages.

---

## Mandatory Reading

Before making architectural changes read:

| Topic                 | Document                                   |
| --------------------- | ------------------------------------------ |
| Architecture overview | docs/architecture/architecture-overview.md |
| Bootstrap             | docs/architecture/bootstrap.md             |
| Composition root      | docs/architecture/composition-root.md      |
| Dependency rules      | docs/architecture/dependency-rules.md      |
| Modular monolith      | docs/architecture/modular-monolith.md      |
| Navigation            | docs/architecture/navigation.md            |

| Assembly definitions  | docs/conventions/asmdef.md                 |
| Dependency injection  | docs/conventions/dependency-injection.md   |
| Events and messaging  | docs/conventions/events.md                 |
| Folder structure      | docs/conventions/folder-structure.md       |
| Naming conventions    | docs/conventions/naming.md                 |
| Package structure     | docs/conventions/packages.md               |
| Testing               | docs/conventions/testing.md                |

| Addressables          | docs/unity/addressables.md                 |
| MVVM                  | docs/unity/mvvm.md                         |
| Scenes                | docs/unity/scenes.md                       |
| UI Toolkit            | docs/unity/ui-toolkit.md                   |

---

## Architecture Summary

Dependency direction:

Presentation → Application → Domain

Infrastructure → Application

Infrastructure → Domain

Bootstrap → all feature packages

---

## Forbidden Dependencies

Domain → UnityEngine

Domain → Infrastructure

Domain → Presentation

Application → Presentation

Application → Infrastructure implementations

Feature A internals → Feature B internals

---

## Mandatory Rules

1. UI Toolkit belongs to Presentation.
2. ViewModels belong to Presentation.
3. UseCases belong to Application.
4. Business rules belong to Domain.
5. Infrastructure implements Application ports.
6. Features communicate through Contracts.
7. New functionality should be implemented inside a feature package.
8. Shared functionality should only be moved to Foundation when used by multiple features.
9. Do not introduce global singleton services without approval.
10. Do not bypass Bootstrap when creating new runtime flows.

---

## Where To Add New Code

New screen:
docs/unity/ui-toolkit.md

New ViewModel:
docs/unity/mvvm.md

New UseCase:
docs/examples/usecase-example.md

New Feature:
docs/examples/feature-example.md

New Event:
docs/conventions/events.md

New Package:
docs/conventions/packages.md

New Scene:
docs/unity/scenes.md

New Dependency Registration:
docs/architecture/composition-root.md

---

## Examples

Feature package example:
docs/examples/feature-example.md

Screen example:
docs/examples/screen-example.md

UseCase example:
docs/examples/usecase-example.md

Event example:
docs/examples/event-example.md
