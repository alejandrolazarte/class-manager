# Roslyn analyzers

Project conventions that break the build when they are not followed. They live in `analyzers/ClassManager.Analyzers` and are wired into every project through `Directory.Build.props`, so a new project gets them automatically.

Rules in `CLAUDE.md` depend on someone reading them; these are enforced by the compiler, for people and AI agents alike.

## Rules

| ID | Applies to | Rule |
|---|---|---|
| TEST001 | Files with a `[Fact]` or `[Theory]` | The file name starts with `Then_` |
| TEST002 | Same | The file is inside a folder that starts with `When_` |
| TEST003 | Same | The file contains exactly one test method |
| TEST004 | Same | The namespace ends with the `When_` folder name, so two `Then_x` classes in different folders never collide |
| TEST005 | Same | The test method is named `<ClassName>_Run` |
| ARCH001 | Assembly `Core` | No `using` of `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore`, `ClassManager.Infrastructure` or `ClassManager.Api` |
| ARCH002 | Any assembly except `*Infrastructure`, `Tenancy.AspNetCore` and `*Tests` | No call to `IgnoreQueryFilters()`: it bypasses tenant isolation |
| ARCH010 | Every assembly | No call to `IgnoreQueryFilters()` without filter names: it also ignores the soft delete filter. Use `IgnoreTenantFilter()` to read across tenants, or name the filters (see [tenancy.md](tenancy.md)) |
| ARCH003 | Assembly `Security` | No `using` of `ClassManager.Core`, `ClassManager.Infrastructure` or `ClassManager.Api`: the security library must stay reusable (see [security.md](security.md)) |
| ARCH004 | Assemblies `Tenancy` and `Tenancy.*` | No `using` of `ClassManager.Core`, `ClassManager.Infrastructure`, `ClassManager.Api` or `ClassManager.Security`: the tenancy library must stay reusable (see [tenancy.md](tenancy.md)) |
| ARCH005 | Assemblies `ImportExport` and `ImportExport.*` | No `using` of `ClassManager.Core`, `ClassManager.Infrastructure`, `ClassManager.Api`, `ClassManager.Security` or `ClassManager.Tenancy`: the import and export engine must stay free of business concepts (see [import-export.md](import-export.md)) |
| ARCH006 | Assemblies `Notifications` and `Notifications.*` | No `using` of `ClassManager.Core`, `ClassManager.Infrastructure`, `ClassManager.Api`, `ClassManager.Security`, `ClassManager.Tenancy` or `ClassManager.ImportExport`: emails and web push must stay free of business concepts (see [notifications.md](notifications.md)) |
| ARCH008 | Assemblies `Storage` and `Storage.*` | No `using` of `ClassManager.Core`, `ClassManager.Infrastructure`, `ClassManager.Api`, `ClassManager.Security`, `ClassManager.Tenancy`, `ClassManager.ImportExport`, `ClassManager.Notifications` or `ClassManager.Subscriptions`: file storage must stay free of business concepts, and the app decides paths and owners (see [storage.md](storage.md)) |
| ARCH009 | Assemblies `Records` and `Records.*` | No `using` of any other `ClassManager.*` assembly: the record conventions (`ICreatedOn`, `IDeletedOn`, `IExpiredOn`) stay free of business concepts so any library can use them (see [CLAUDE.md](../CLAUDE.md#records-that-change-over-time)) |
| ARCH007 | Assemblies `Subscriptions` and `Subscriptions.*` | No `using` of `ClassManager.Core`, `ClassManager.Infrastructure`, `ClassManager.Api`, `ClassManager.Security`, `ClassManager.Tenancy`, `ClassManager.ImportExport` or `ClassManager.Notifications`: plans and features must stay free of business concepts, and the app resolves who the subscriber is (see [subscriptions plan](backend/20261003-subscriptions/plan.md)) |

Test helpers (fixtures, factories, builders) have no test methods, so the TEST rules don't apply to them.

## Example

```text
tests/ClassManager.Api.I.Tests/Endpoints/Clients/
  When_posting_valid_client/
    Then_returns_201_with_location.cs
```

```csharp
namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_posting_valid_client;

public sealed class Then_returns_201_with_location(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_with_location_Run() { ... }
}
```

## When a rule is wrong for a case

Suppress it **locally, with a justification**, never globally:

```csharp
#pragma warning disable ARCH002 // Reason: <why this call is safe>
```

If the same suppression is needed twice, the rule probably needs to change: update the analyzer and its tests instead.

## Changing or adding a rule

1. Write the analyzer test first in `tests/ClassManager.Analyzers.U.Tests` (it uses `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing` with `{|RULE:span|}` markup).
2. Add the descriptor to `DiagnosticDescriptors.cs` and the rule to `AnalyzerReleases.Unshipped.md` (the build fails otherwise: RS2000).
3. Implement it and build the whole solution: existing code must comply, or fix it in the same PR.

## Notes

- The analyzer targets `netstandard2.0` so it loads in any compiler host (Visual Studio, Rider, `dotnet build`).
- The compiler doesn't report analyzer diagnostics while there are compile errors. ARCH001 fires once the forbidden reference actually compiles, for example after someone adds an ASP.NET or EF Core reference to `Core`.
