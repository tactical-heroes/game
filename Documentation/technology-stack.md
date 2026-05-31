# Technology Stack

## Purpose

This document lists approved project technologies and architectural mechanisms.
Use it as the source of truth before introducing alternatives.

Detailed usage rules live in focused architecture, conventions, and Unity docs.

## Approved Stack

| Area                  | Technology or mechanism                    | Purpose                                      | Details                                           |
| --------------------- | ------------------------------------------ | -------------------------------------------- | ------------------------------------------------- |
| Architecture          | Feature-first modular package              | Isolate business capabilities                | Documentation/conventions/project-structure.md    |
| Domain modeling       | DDD where justified                        | Keep complex business rules explicit         | Documentation/architecture/dependency-rules.md    |
| Dependency Injection  | VContainer                                 | DI, lifetimes, feature installers            | Documentation/conventions/dependency-injection.md |
| Assembly boundaries   | Unity asmdef                               | Compile boundaries and dependency control    | Documentation/conventions/asmdef.md               |
| NuGet packages        | NuGetForUnity                              | Manage NuGet dependencies in Unity           | Assets/packages.config                            |
| Content loading       | Addressables                               | Runtime asset and scene content loading      | Documentation/unity/addressables.md               |
| Local data storage    | SQLite via gilzoide sqlite-net             | Structured local persistent data             | Documentation/unity/storage.md                    |
| Secret storage        | Platform secure storage                    | Tokens, credentials, and secret values       | Documentation/unity/storage.md                    |
| Windows secret API    | System.Security.Cryptography.ProtectedData | Windows-only DPAPI adapter API               | Documentation/unity/storage.md                    |
| UI technology         | UI Toolkit                                 | Runtime UI screens and components            | Documentation/unity/ui-toolkit.md                 |
| UI architecture       | MVVM                                       | Screen presentation state and commands       | Documentation/unity/ui-toolkit.md                 |
| Runtime entry         | Bootstrap scene                            | Production startup and app lifetime          | Documentation/architecture/bootstrap.md           |
| Navigation            | Router and ScreenHost                      | Screens, modals, overlays, scene-backed flow | Documentation/architecture/navigation.md          |
| Messaging             | MessagePipe                                | Events and application messages              | Documentation/conventions/events.md               |
| CQRS-style messaging  | MessagePipe                                | Commands, queries, and handlers when needed  | Documentation/conventions/events.md               |
| Async                 | UniTask                                    | Unity-friendly async operations              | Documentation/conventions/dependency-injection.md |
| Reactive programming  | R3                                         | Reactive state and streams                   | Documentation/unity/ui-toolkit.md                 |
| Tweens and animations | PrimeTween                                 | UI and gameplay tween animations             | Documentation/unity/ui-toolkit.md                 |

## Rules

Do not introduce competing frameworks without approval.

Examples:

* Do not add another DI container while VContainer is the approved container.
* Do not add another event bus while MessagePipe is the approved messaging tool.
* Do not add another tween library while PrimeTween is the approved tween tool.
* Do not use legacy Unity UI for new screens without a project-specific reason.

If a technology needs detailed rules, add a focused document and link it from
this table instead of expanding this file.
