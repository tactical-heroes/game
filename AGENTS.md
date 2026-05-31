## Project Overview

This repository contains a Unity 6 cross-platform turn-based strategy game with
multiplayer, inspired by classic Heroes of Might and Magic V-style gameplay.
Target platforms: Windows, Linux, Android, iOS, and macOS.

The application is composed from feature-first local Unity packages.

Business logic, UI, infrastructure, and application orchestration are isolated
inside feature packages.

Approved technologies are listed in `Documentation/technology-stack.md`.

---

## How To Use Docs

Do not read every document by default. Read the documents relevant to the
change you are making.

Always apply:

| Topic             | Document                                       |
| ----------------- | ---------------------------------------------- |
| Technology stack  | Documentation/technology-stack.md              |
| Dependency rules  | Documentation/architecture/dependency-rules.md |
| Project structure | Documentation/conventions/project-structure.md |

Read when relevant:

| Task                                                 | Document                                          |
| ---------------------------------------------------- | ------------------------------------------------- |
| Startup or runtime entry flow                        | Documentation/architecture/bootstrap.md           |
| Routes, modals, back stack, ScreenHost               | Documentation/architecture/navigation.md          |
| Assembly definitions and compile boundaries          | Documentation/conventions/asmdef.md               |
| DI registration, lifetimes, installers               | Documentation/conventions/dependency-injection.md |
| Events or MessagePipe                                | Documentation/conventions/events.md               |
| Naming a new type, route, assembly, package, or test | Documentation/conventions/naming.md               |
| Tests or test layout                                 | Documentation/conventions/testing.md              |
| Addressables or asset loading                        | Documentation/unity/addressables.md               |
| Local storage, SQLite, or secrets                    | Documentation/unity/storage.md                    |
| Unity scenes, scene entry points, scene scope        | Documentation/unity/scenes.md                     |
| UI Toolkit screen, View, ViewModel, UXML, USS        | Documentation/unity/ui-toolkit.md                 |

---

## Where To Add New Code

| New code                | Read                                              |
| ----------------------- | ------------------------------------------------- |
| Screen or ViewModel     | Documentation/unity/ui-toolkit.md                 |
| UseCase                 | Documentation/architecture/dependency-rules.md    |
| Feature or package      | Documentation/conventions/project-structure.md    |
| Event                   | Documentation/conventions/events.md               |
| Scene                   | Documentation/unity/scenes.md                     |
| Storage                 | Documentation/unity/storage.md                    |
| Dependency registration | Documentation/conventions/dependency-injection.md |
| asmdef                  | Documentation/conventions/asmdef.md               |
| Test                    | Documentation/conventions/testing.md              |
