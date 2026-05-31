## Project Overview

This repository contains a Unity 6 cross-platform turn-based strategy game with
multiplayer, inspired by classic Heroes of Might and Magic V-style gameplay.
Target platforms: Windows, Linux, Android, iOS, and macOS.

The application is composed from feature-first local Unity packages.

Business logic, UI, infrastructure, and application orchestration are isolated
inside feature packages.

Approved technologies are listed in `docs/technology-stack.md`.

---

## How To Use Docs

Do not read every document by default. Read the documents relevant to the
change you are making.

Always apply:

| Topic             | Document                              |
| ----------------- | ------------------------------------- |
| Technology stack  | docs/technology-stack.md              |
| Dependency rules  | docs/architecture/dependency-rules.md |
| Project structure | docs/conventions/project-structure.md |

Read when relevant:

| Task                                                 | Document                                 |
| ---------------------------------------------------- | ---------------------------------------- |
| Startup or runtime entry flow                        | docs/architecture/bootstrap.md           |
| Routes, modals, back stack, ScreenHost               | docs/architecture/navigation.md          |
| Assembly definitions and compile boundaries          | docs/conventions/asmdef.md               |
| DI registration, lifetimes, installers               | docs/conventions/dependency-injection.md |
| Events or MessagePipe                                | docs/conventions/events.md               |
| Naming a new type, route, assembly, package, or test | docs/conventions/naming.md               |
| Tests or test layout                                 | docs/conventions/testing.md              |
| UI design, Figma source, visual states               | docs/design/figma.md                     |
| Addressables or asset loading                        | docs/unity/addressables.md               |
| Local storage, SQLite, or secrets                    | docs/unity/storage.md                    |
| Unity scenes, scene entry points, scene scope        | docs/unity/scenes.md                     |
| UI Toolkit screen, View, ViewModel, UXML, USS        | docs/unity/ui-toolkit.md                 |

---

## Where To Add New Code

| New code                | Read                                     |
| ----------------------- | ---------------------------------------- |
| Screen or ViewModel     | docs/unity/ui-toolkit.md                 |
| UseCase                 | docs/architecture/dependency-rules.md    |
| Feature or package      | docs/conventions/project-structure.md    |
| Event                   | docs/conventions/events.md               |
| Scene                   | docs/unity/scenes.md                     |
| UI design mapping       | docs/design/figma.md                     |
| Storage                 | docs/unity/storage.md                    |
| Dependency registration | docs/conventions/dependency-injection.md |
| asmdef                  | docs/conventions/asmdef.md               |
| Test                    | docs/conventions/testing.md              |
