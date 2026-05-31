# Assembly Definitions

## Purpose

Assembly Definitions make architecture boundaries visible and enforceable.
Use asmdefs to control compile dependencies, editor/runtime separation, and
feature isolation.

Dependency rules are defined in `Documentation/architecture/dependency-rules.md`.

## Core Principle

If assembly A uses assembly B, the reference must be explicit.
No dependency should be hidden by folder placement.

## Strategy

Small feature:

```text
Company.Game.Feature.Inventory
Company.Game.Feature.Inventory.Editor
Company.Game.Feature.Inventory.Tests.Editor
Company.Game.Feature.Inventory.Tests.Runtime
```

Large feature:

```text
Company.Game.Feature.Inventory.Contracts
Company.Game.Feature.Inventory.Domain
Company.Game.Feature.Inventory.Application
Company.Game.Feature.Inventory.Infrastructure
Company.Game.Feature.Inventory.Presentation
Company.Game.Feature.Inventory.Composition
```

Start small when the feature is small. Split by layer when boundaries,
compilation time, or team ownership justify it.

## Layer References

Allowed references for split features:

| Assembly       | May reference                                             |
| -------------- | --------------------------------------------------------- |
| Contracts      | Foundation abstractions only                              |
| Domain         | Foundation.Domain                                         |
| Application    | Domain, Contracts, Foundation.Application                 |
| Infrastructure | Application, Domain, Contracts, Foundation.Infrastructure |
| Presentation   | Application, Contracts, Foundation.Presentation           |
| Composition    | Same feature layers, VContainer, MessagePipe              |

Forbidden references:

* Domain -> Unity, Application, Infrastructure, Presentation.
* Application -> Infrastructure implementations or Presentation.
* Presentation -> Infrastructure implementations.
* Infrastructure -> Presentation.
* Feature A internals -> Feature B internals.
* Runtime assembly -> Editor assembly.

## Bootstrap And Foundation

Bootstrap usually has one runtime assembly:

```text
Company.Game.Bootstrap
```

Bootstrap references feature Composition assemblies and Foundation.
Feature assemblies must not reference Bootstrap.

Foundation may be split by responsibility when it grows:

```text
Company.Game.Foundation.Domain
Company.Game.Foundation.Application
Company.Game.Foundation.Infrastructure
Company.Game.Foundation.Presentation
```

Avoid a giant Foundation assembly that every feature depends on.

## Editor Assemblies

Editor-only code lives in editor assemblies and may reference runtime
assemblies.

Runtime code must never reference editor assemblies or `UnityEditor`.

## Test Assemblies

Use:

```text
Company.Game.Feature.Inventory.Tests.Editor
Company.Game.Feature.Inventory.Tests.Runtime
```

Editor tests are preferred for pure Domain and Application tests.
Runtime tests are for Unity lifecycle, scenes, Addressables, and Play Mode
behavior.

## Naming

Assembly names use PascalCase segments:

```text
Company.Game.Feature.Inventory.Application
```

Namespaces should align:

```csharp
namespace Company.Game.Features.Inventory.Application
{
}
```

See `Documentation/conventions/naming.md` for the full naming table.

## When To Split

Split assemblies when:

* A feature has real layer complexity.
* Architecture boundaries need compiler enforcement.
* Compile time becomes noticeable.
* Multiple people work in the feature.
* Editor code must be isolated.

Do not create many assemblies for a tiny screen or a simple static feature.

## Review Checklist

Before adding or changing an asmdef, verify:

1. References match the dependency rules.
2. Runtime code does not reference editor code.
3. Cross-feature references use Contracts only.
4. Domain compiles without Unity runtime dependencies.
5. Assembly and namespace names are aligned.
