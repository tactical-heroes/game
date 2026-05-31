# Storage

## Purpose

This document defines approved local storage choices.

Storage implementation belongs to Infrastructure. Domain and Application should
depend on ports and stable models, not concrete storage libraries.

## Data Storage

Use SQLite for structured local persistent data.

Approved package:

```text
com.gilzoide.sqlite-net
```

Use SQLite for:

* Local saves with relational structure.
* Player progress snapshots.
* Cached read models.
* Offline data that needs queries or indexes.

Do not store secrets directly in SQLite.

## Secret Storage

Use platform secure storage for secrets:

| Platform        | Secure storage   | Implementation note                          |
| --------------- | ---------------- | -------------------------------------------- |
| iOS             | Apple Keychain   | Native platform adapter                      |
| Android         | Android Keystore | Native platform adapter                      |
| Windows desktop | DPAPI            | `System.Security.Cryptography.ProtectedData` |
| Linux desktop   | Secret Service   | Native platform adapter                      |

Secrets include tokens, refresh tokens, credentials, encryption keys, and other
values that should not be readable as plain local data.

For Windows DPAPI, use the NuGet package:

```text
System.Security.Cryptography.ProtectedData
```

The API is exposed through the `System.Security.Cryptography` namespace.
The package targets .NET Standard 2.0, so it can compile in Unity, but DPAPI is
Windows-only. Call it only from a Windows desktop infrastructure adapter behind
platform guards. It is not a cross-platform .NET Core secret storage API.
Non-Windows runtimes must use their own secure store adapter.

## Dependency Direction

Application owns storage ports:

```text
IPlayerProgressRepository
ISecretStore
ISaveDatabase
```

Infrastructure implements these ports using SQLite or platform secure storage.
Presentation and Domain must not depend on SQLite, Keychain, Keystore, DPAPI,
or Secret Service implementations.

## File Ownership

Database files belong to app-controlled persistent storage.

Do not put writable databases in Addressables or StreamingAssets. Seed
databases may be distributed as read-only content, then copied into persistent
storage before writes.

## Async And Lifetime

Storage operations should be async when they may touch disk or platform APIs.
Use UniTask for Unity-facing async flows.

Connections, transactions, and subscriptions must be disposed by the owning
scope. Do not keep hidden global database connections.

## Review Checklist

Before adding storage code, verify:

1. Application depends on a port, not a concrete storage implementation.
2. SQLite is used for structured data, not secrets.
3. Secrets use the platform secure store.
4. Database file ownership and migrations are explicit.
5. Failure, cancellation, and disposal behavior is defined.
