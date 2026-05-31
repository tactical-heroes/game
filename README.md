# Tactical Heroes

Tactical Heroes is a Unity 6 cross-platform turn-based strategy game with
multiplayer, inspired by classic Heroes of Might and Magic V-style gameplay.

The project targets Windows, Linux, macOS, Android, and iOS. It is built as one
Unity application composed from feature-first local packages.

## Status

The game is in active development. Architecture, package layout, UI rules, and
technology choices are documented in the `docs/` folder.

## Technology Stack

Core technologies:

* Unity 6
* C#
* UI Toolkit
* MVVM
* VContainer
* MessagePipe
* Addressables
* UniTask
* R3
* PrimeTween
* NuGetForUnity
* SQLite via gilzoide sqlite-net

See `docs/technology-stack.md` for the approved stack and detailed links.

## Requirements

Recommended editor version:

```text
Unity 6000.4.9f1
```

Repository setup expects:

* Git LFS for binary assets.
* Unity Package Manager to restore Unity packages.
* NuGetForUnity to restore NuGet packages from `Assets/packages.config`.

## Getting Started

1. Clone the repository.
2. Make sure Git LFS files are pulled.
3. Open the project folder in Unity 6.
4. Let Unity restore packages and import assets.
5. Open the bootstrap scene or the relevant development scene.

## Documentation

Start with:

| Topic             | Document                              |
| ----------------- | ------------------------------------- |
| Technology stack  | docs/technology-stack.md              |
| Dependency rules  | docs/architecture/dependency-rules.md |
| Project structure | docs/conventions/project-structure.md |
| UI Toolkit        | docs/unity/ui-toolkit.md              |
| Storage           | docs/unity/storage.md                 |

## License

This project is licensed under the Apache License 2.0. See `LICENSE`.
