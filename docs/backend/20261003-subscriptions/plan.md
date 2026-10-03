# Backend plan — Subscriptions, plans and features

Status: steps 1–3 partly built (library, persistence, sign-up on `Free`, import/export and custom roles gated, branch limit); see [Steps](#steps). Closes the "Pricing model" [open decision](../../mvp-plan.md#open-decisions) for the MVP; the prices and the exact split of features per plan are placeholders for the owner to decide.

## Goal

- Every brand has **one subscription**, and that subscription has a **plan**: `Free`, `Lite`, `Pro` or `Enterprise`.
- What a plan gives is a set of **features**. Some features can also be bought on their own as **add-ons**.
- The API refuses a feature the subscription doesn't have, the same way it refuses a missing permission, and the app shows it as locked.
- No online billing yet: the platform operator sets plans and prices by hand. Charging cards comes later and plugs into the same model.

## Decisions

### The subscription belongs to the organization, not to the branch

- `Organization` is the brand ("empresa máster") and `Business` is the branch and the tenant ([authorization](../../authorization.md)). The subscription hangs from the organization, so **every branch of a brand has the same plan**.
- This matches how a brand is run: the brand owner pays and decides, a branch never sees a pricing screen, and creating a branch can't start a second bill.
- Limits that depend on the number of branches (for example "up to 3 branches") are features of the brand's plan, so they are checked in one place.
- A franchise where each branch pays its own bill is not a brand here: each franchisee signs up as their own organization. If that changes, `Subscription` gets an optional `BusinessId` later; nothing in this plan blocks it.
- A solo instructor is an organization with one branch, so nothing special is needed for them.

### Four plans

| Plan | Who it is for | Price shown |
|---|---|---|
| `Free` | Every new sign-up, forever, with limits | 0 |
| `Lite` | One instructor or a small studio | Placeholder |
| `Pro` | A studio with a team, a shop and its own brand | Placeholder |
| `Enterprise` | Brands with several branches; agreed by hand | "Consultar" (no list price) |

Four is enough: fewer and `Lite` and `Pro` merge into one plan that is either too cheap for studios or too expensive for instructors; more and the plan screen needs a comparison table nobody reads. Plan codes are stable identifiers (`free`, `lite`, `pro`, `enterprise`); the display name is UI copy.

### The plan's price is a list price; the subscription holds the real one

- `Plan.ListPrice` is what the app shows before subscribing ("desde …").
- `Subscription.Price` is what this brand actually pays, copied from the list price when the subscription is created and editable afterwards. Changing a list price never changes what existing customers pay.
- That is how the pilot instructor gets **`Enterprise` at 0**: a normal subscription with `Price = 0` and a `Note` ("Pilot, DF Swimming"). No special case in code, and later it becomes a discount or an end date (`EndsOn`) without a migration.
- Every price has a currency (`ARS`, `EUR`, `USD`): the pilot is in Spain and the target market is Argentina.

### Features, and an add-on is a feature with a price

Add-ons are not a separate concept: an add-on is a **feature that can also be bought on its own**.

- `Feature` is a catalog entry (`import-export`, `brand`, `shop`, …).
- `PlanFeature` says which features a plan includes, and, for counted features, the limit (`Limit = 3` branches; `null` is unlimited).
- A feature with `IsAddOn = true` has an `AddOnListPrice`, and a brand on a plan that doesn't include it can still get it through `SubscriptionFeature`. A feature that is not an add-on has no price: the only way to get it is a plan that includes it.
- `SubscriptionFeature` is **what the brand has on top of its plan**: a bought add-on with its agreed price, or a feature granted by hand at 0 (for example, to let a customer try `shop` for a month, with `EndsOn`). It can also raise a limit (`Limit = 5` branches).
- **Effective features = the plan's features ∪ the subscription's active features**, the larger limit winning. Included features are not copied into each subscription, so improving a plan improves it for everyone on it. Grandfathering an old plan, if it's ever needed, is a new plan code.

### Features are checked like permissions, and both must pass

- **Features live in code, plans live in data** — the same split as permissions and roles. The catalog is a constants class (`Features`) because endpoints and use cases reference the codes; which plan includes what, and the prices, are rows, so they change without a deploy.
- A request needs **the permission (who you are in the branch) and the feature (what the brand pays for)**. The brand owner on `Free` can't import; a coach on `Enterprise` still can't see money.
- Endpoints declare it next to the permission: `.RequirePermission(Permissions.ImportExport.Run).RequireFeature(Features.ImportExport)`.
- Limits are checked in the use case (`creating a branch when the brand already has as many as its limit`) through `IFeatureAccess`, and fail with `403 feature.limit_reached`.
- **Gates block actions, not reads.** Creating, editing, importing and exporting need the feature; listing what already exists doesn't, so a brand that drops to a smaller plan still sees its data. Deleting a custom role stays allowed too, so a downgraded brand can clean up.
- A missing feature returns `403` with the code `feature.not_in_plan` (and `feature.limit_reached` for limits) and the feature code in `feature`, never the permission's code, so the app can offer an upgrade instead of acting as if the button didn't exist.
- Effective features are read once per request, like permissions, so a plan change applies on the next request.

### Downgrades never delete data

- When a subscription ends or drops to a smaller plan, the brand falls back to `Free` (or to the new plan). Nothing is deleted: brand colors stay stored and stop applying ([branding and plans](../../branding-and-plans.md)), extra branches stay readable, imports stop.
- A limit blocks **creating** past it, never existing data: a brand with 4 branches that drops to a 1-branch plan keeps all 4 and can't create a fifth.

## Model

```mermaid
erDiagram
    Organization ||--|| Subscription : "has one active"
    Plan ||--o{ Subscription : "is on"
    Plan ||--o{ PlanFeature : includes
    Feature ||--o{ PlanFeature : "included in"
    Subscription ||--o{ SubscriptionFeature : "adds on top"
    Feature ||--o{ SubscriptionFeature : "added as"
    Organization ||--o{ Business : "branches (tenants)"

    Plan {
        string Code PK "free, lite, pro, enterprise"
        int DisplayOrder
        decimal ListPrice "nullable: Enterprise has none"
        string Currency
        string BillingPeriod "Monthly"
        bool IsDefault "true only for free"
        bool IsActive
    }
    Feature {
        string Code PK "from the Features class"
        bool IsAddOn
        decimal AddOnListPrice "required when IsAddOn"
        string Currency
        bool IsCounted "has a limit: branches, students"
    }
    PlanFeature {
        string PlanCode PK
        string FeatureCode PK
        int Limit "nullable: unlimited"
    }
    Subscription {
        Guid Id PK
        Guid SubscriberId "the organization; one open-ended per organization"
        string PlanCode
        decimal Price "the real price"
        string Currency
        date StartsOn
        date EndsOn "nullable: open-ended; set when it ends"
        string Note "why this price"
        DateTimeOffset CreatedAt
    }
    SubscriptionFeature {
        Guid Id PK
        Guid SubscriptionId
        string FeatureCode
        decimal Price "0 when granted"
        string Currency
        int Limit "nullable"
        date StartsOn
        date EndsOn "nullable"
    }
```

- A plan change closes the current subscription (`EndsOn`) and opens a new one, so the history of who paid what stays. A filtered unique index keeps one active subscription per organization.
- None of these tables are tenant-owned: like `Organizations`, they sit above the branches and are never filtered by `TenantId`.
- `Feature` rows mirror the `Features` class; a test fails when a code exists in one and not the other.

## How a request is checked

```mermaid
sequenceDiagram
    participant App
    participant Api as Api (endpoint)
    participant Perm as PermissionAuthorizationHandler
    participant Feat as RequireFeature (endpoint filter)
    participant Sub as IFeatureAccess
    participant Db as SQL Server

    App->>Api: POST /api/import-export/students (tenant_id = branch)
    Api->>Perm: permission importExport.run? (authorization, runs first)
    Perm-->>Api: ok (ICurrentMember)
    Api->>Feat: feature import-export?
    Feat->>Sub: effective features of the branch's organization
    Sub->>Db: active Subscription + PlanFeatures + SubscriptionFeatures (once per request)
    Db-->>Sub: rows
    Sub-->>Feat: { import-export, brand, branches: 3, ... }
    alt included
        Feat-->>Api: ok
        Api-->>App: 200
    else not included
        Feat-->>App: 403 feature.not_in_plan, feature = import-export
    end
```

`RequireFeature` is an endpoint filter, not an authorization policy: authorization already has the app's `permission:` policy provider, and a filter can answer with the problem body (`code`, `feature`) the app needs to show an upgrade. It also adds `RequiredFeatureMetadata`, and a test checks every required code is in `Features.All`.

## Feature catalog and split (first version, placeholders)

The daily core — students, families, class groups, enrollments, attendance, monthly fees — is never a feature: every plan has it, otherwise `Free` is useless.

| Feature | Counted | `Free` | `Lite` | `Pro` | `Enterprise` | Add-on |
|---|---|---|---|---|---|---|
| `students` (students of the branch) | yes | 30 | 150 | unlimited | unlimited | no |
| `branches` | yes | 1 | 1 | 3 | unlimited | yes, per extra branch |
| `team` (members of the branch besides its owners, pending invitations included) | yes | 0 | 2 | 10 | unlimited | no |
| `class-packs` and private lessons | | | ✓ | ✓ | ✓ | no |
| `import-export` | | | ✓ | ✓ | ✓ | yes |
| `custom-roles` | | | | ✓ | ✓ | no |
| `family-app` (family accounts, avisos) | | | | ✓ | ✓ | yes |
| `shop` (products and orders) | | | | ✓ | ✓ | yes |
| `brand` (brand theme, logo, theme lock) | | | | ✓ | ✓ | yes |
| Future paid features (online payments, reports, …) | | | | | | yes, priced |

## Library or part of Core?

**A separate library, like `Security`**: `src/Subscriptions` and `src/Subscriptions.AspNetCore`, with a new analyzer rule **ARCH007** (no `using` of `Core`, `Infrastructure`, `Api`, `Security`, `Tenancy`, `ImportExport` or `Notifications`).

Why:

- The mechanism has no class-manager concept in it. A subscription belongs to an opaque **subscriber id**; the library doesn't know it's an organization. Plans, features, add-ons, effective features, limits and `RequireFeature` are the same in any SaaS built from the template.
- The repository already works this way (`Security`, `Tenancy`, `ImportExport`, `Notifications`) and has the analyzers to keep the boundary. The boundary is cheap now and expensive to add once subscription code is spread over `Core`.
- It is the next candidate for the template ([README](../../../README.md)): build it here, then port it to `app-template` once it has run in the pilot.

What stays in the app:

| Library (`Subscriptions`, no dependencies) | Library (`Subscriptions.AspNetCore`) | App |
|---|---|---|
| `Plan`, `Feature`, `PlanFeature`, `Subscription`, `SubscriptionFeature`; `EffectiveFeatures` (union, larger limit wins, dates); `IFeatureAccess`, `ISubscriberResolver`; `FeatureErrorCodes` | `ApplySubscriptionsModel()` (tables in schema `billing`) and `ISubscriptionsDbContext`; `FeatureAccess` (once per request, falls back to the default plan); `RequireFeature(...)` endpoint filter; `AddSubscriptions<TDbContext, TSubscriberResolver>()` | `Features` and `PlanCodes` in `Core`; plan rows and prices (`SubscriptionCatalogSeed`); `OrganizationSubscriberResolver` (tenant → organization); the subscription at sign-up; limit checks in use cases; `GET /api/me` and the app screens |

- **The app's `AppDbContext` holds the tables**, applying the library's model like it applies the tenant query filters, instead of a `SubscriptionsDbContext` of its own. One migration history, the foreign key from `Subscriptions.SubscriberId` to `Organizations` (added by the app, since the library doesn't know organizations), and sign-up writes the subscription in the same transaction. `Security` has its own context because of Identity; this library has no such reason.
- **`Core` references `Subscriptions`**, the same as it references `Tenancy` and `Notifications`: the project has no dependencies, and use cases need `IFeatureAccess` for limits. `Core` never references `Subscriptions.AspNetCore`.

## API

| Operation | Endpoint | Who |
|---|---|---|
| The brand's plan, price, features and limits with usage | `GET /api/organization/subscription` | `subscription.view` (brand only) |
| Effective feature codes and limits, for the app to lock things | added to `GET /api/me` (`features`) | any member |
| Public plans with list prices, for the sign-up and plan screens | `GET /api/plans` | anonymous |

No endpoint changes a subscription in the MVP: there is no platform administrator role yet. The operator runs `scripts/set-subscription.cs <connection> <organizationId> <plan> <price> <currency> [note]` (and `add-feature`), the same style as `scripts/seed-demo-business.cs`. A platform admin screen comes with online billing.

## In the app

- `useFeature(code)` next to `useCan(...)`, fed by `GET /api/me`.
- **Permissions hide, features lock.** A button the member's role doesn't allow is hidden; a feature the plan doesn't include is shown with a lock and "Disponible en Pro", which only the brand owner can act on.
- Ajustes → Plan (brand owner only): current plan, price, what it includes, usage of counted features (`Alumnos 24 / 30`), and "Escribinos para cambiar de plan" until self-service exists.
- Sign-up shows nothing about plans: everyone starts on `Free`.

## Migration

- Create the `billing` schema, the five tables, the `Feature` rows and the four plans with their features.
- Every **existing** organization gets `Enterprise` at 0 with the note "Pilot": today that is DF Swimming, and the pilot keeps everything it already uses (brand, shop, family app, three branches).
- New sign-ups get the default plan (`Free`) from then on. The setting `Subscriptions:SignUpPlan` overrides it with another plan code: the e2e tests use `enterprise` so their flows don't depend on plan limits, and a launch promotion could use it too. Leave it unset in production unless that is the intent.
- `scripts/seed-demo-business.cs` gives the demo brand `Enterprise` at 0.

## Steps

1. ✅ `src/Subscriptions` + ARCH007 + unit tests of effective features (union, larger limit, expired add-on ignored).
2. ✅ `Subscriptions.AspNetCore`: model, migration with the seed, `RequireFeature`, `FeatureAccess`; sign-up creates the `Free` subscription; existing organizations get `Enterprise` at 0; a branch of brand A never reads brand B's subscription.
3. Gate the features in the table:
   - ✅ `import-export` (import and export), `custom-roles` (create and edit), `branches` limit. Tests: a brand without the feature gets `403 feature.not_in_plan`, an add-on unlocks it, the limit blocks the next branch and an add-on raises it.
   - Next: `students` and `team` limits (adding students in every entry point: client form, student form, import; inviting members), and `class-packs`, `family-app`, `shop`, `brand`, together with step 4 so the app shows locks instead of errors.
4. `features` in `GET /api/me`, `GET /api/organization/subscription`, `GET /api/plans`; app: `useFeature`, locks and Ajustes → Plan.
5. `scripts/set-subscription.cs` and a runbook in `docs/`.

## Later, not in the MVP

- Online billing (Mercado Pago / Stripe subscriptions): webhooks set `EndsOn`, and a `Status` column (`PastDue`, grace period) arrives with them; until then `EndsOn` alone says whether a subscription is active.
- Trials (`Pro` for 14 days at sign-up): a subscription with `EndsOn` that falls back to `Free`.
- Yearly billing (`BillingPeriod = Yearly`), coupons, and a platform admin screen.
