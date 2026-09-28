# Plan — More themes, a business theme and a business icon

Follow-up to the Claude Design exploration "Iconos y Rubro". It proposed a **Rubro** setting (swimming, yoga, pilates, ...) that changed the icon in the app and suggested a theme for each activity. The idea is kept, but the two parts are split: the theme is not tied to the activity.

## Decisions

- **Two separate settings, owned by different people:**

  | Setting | Owner | Stored | Who sees it |
  |---|---|---|---|
  | **Theme** (colors, light/dark) | Each user | On the device (already: `PersistedThemePreferences`) | Only that user |
  | **Business icon** | The business (`Business.Manage`) | On the server, in `Businesses` | Every member of the business |

  An owner, a coach or (some day) a student picks the colors they like; no theme belongs to an activity.
- **No "Rubro".** It reads as a classification ("you are a swimming school") and nothing else in the app uses it. The owner chooses an **icon**, from a catalog grouped visually but not locked to an activity. The name in the UI is **"Icono del negocio"**.
- **The app mark and the business icon are different things.** Sign-in, welcome, forgot and reset password run before the app knows the business, so they keep the fixed app mark (`BrandMark`, the `pool` glyph today; a new app icon is being designed separately). Screens after sign-in that represent the business (the business card in Ajustes, the class chip in `StudentClasses`) show the business icon.
- **The icon is stored as a catalog key** (`swimming`, `yoga`, ...), not as a Material Icons glyph name, so swapping the icon set only touches the app. The API validates the key against its catalog; the app shows the `general` icon for a key it doesn't know (an older app against a newer API).
- **Defaults:** new businesses get `general`. The migration sets existing businesses to `swimming`, which is what they show today (the only pilot is a swimming school), so nothing changes for them.
- **Theme names describe colors**, not activities: Rosa, Arena, Coral, Cancha, Ciruela, Índigo join Agua, Violeta and Océano.
- **A business can create its own theme from one color.** The owner picks a brand color; the app derives the rest (light and dark) so every text pair passes WCAG AA. It shows up for every member as an extra theme, **"Colores de <negocio>"**, and is the default for anyone who hasn't chosen a theme. Each user can still pick another one: the business theme is offered, not imposed.

## Out of scope

- A custom logo upload ("Subir logo propio"). It will reuse the business icon slot, stored in the database (a few KB per business, inside the Azure SQL free offer, see [hosting plan](../../hosting-plan.md)). It waits until something the students see (a payment receipt) needs it.
- The new app icon (launcher, splash, `BrandMark`).
- A full theme editor (choosing every token). One color covers the need; the `customThemes` support in [theming](../theming.md) already accepts a full definition if that is ever wanted.
- Asking "¿Qué enseñás?" at sign-up to preselect an icon and a theme. Easy to add later; the owner can change both at any time.
- An icon per organization (brand) instead of per business (branch). Today every organization has one business; revisit with [branches](../../backend/20260928-roles-and-permissions/plan.md).

## Part 1 — Themes (app only)

| Id | Name | Primary (light) |
|---|---|---|
| `aqua` | Agua | existing |
| `violet` | Violeta | existing |
| `ocean` | Océano | existing |
| `rose` | Rosa | rose / raspberry |
| `sand` | Arena | burnt orange |
| `coral` | Coral | coral red |
| `court` | Cancha | green |
| `plum` | Ciruela | plum purple |
| `indigo` | Índigo | indigo blue |

- New themes follow `violet` and `ocean`: neutral grays plus their own `primary*` tokens, in light and dark. Values come from the design handoff; missing raw colors go to `palette.ts`.
- Names as `themes.<id>` in `es.json`.
- The color picker in Ajustes → Apariencia lays the swatches out in a wrapping grid of three per row; nine don't fit in one row.
- [Theming](../theming.md) is updated: the list of built-in themes, and "each user picks" instead of "the owner picks".

### Tests (write first)

- `When_any_theme_is_defined/Then_foreground_colors_meet_contrast_minimum` already covers every theme in light and dark (WCAG AA 4.5:1); a new theme that fails it doesn't merge.
- The picker shows every built-in theme with its Spanish name.
- Choosing a new theme applies and persists it (extend the existing `When_theme_is_changed` coverage if needed).

## Part 2 — Business icon (API)

| Operation | Endpoint | Use case | Permission |
|---|---|---|---|
| Read | `GET /api/business` gains `iconKey` | `GetCurrentBusinessUseCase` | `Business.View` |
| Change | `PUT /api/business/icon` `{ "iconKey": "yoga" }` | `SetBusinessIconUseCase` | `Business.Manage` |

A separate endpoint (like `PUT /api/business/monthly-fee`) because the icon is saved as soon as it is tapped, without the rest of the settings form.

- **Catalog:** `BusinessIcons` in `src/Core/Domain/Businesses`: `swimming`, `yoga`, `pilates`, `functional`, `martialArts`, `racketSports`, `gymnastics`, `running`, `football`, `music`, `art`, `languages`, `general`. Keys are constants; `BusinessIcons.All` is the set the API accepts.
- **Domain:** `Business.IconKey` (max 32 characters) and `Business.SetIcon(iconKey)`, which rejects an unknown key with a validation error on `IconKey`. `Business.Create` sets `general`.
- **Data:** migration `AddBusinessIcon`: column `Businesses.IconKey nvarchar(32) not null default 'general'`, then `UPDATE Businesses SET IconKey = 'swimming'` for existing rows. Additive, safe while the previous API version runs during a deploy.

| # | Rule | Result |
|---|---|---|
| I1 | `iconKey` is one of `BusinessIcons.All` (case-sensitive) | Validation `400` |
| I2 | Only the current business changes | Tenant from the token |

### Tests (write first)

- Unit: `When_icon_key_is_unknown/Then_it_is_rejected`; `When_icon_is_set/Then_business_has_the_new_icon`; `When_business_is_created/Then_its_icon_is_general`.
- Integration: `When_owner_sets_the_icon/Then_get_business_returns_it`; `When_icon_key_is_unknown/Then_bad_request_is_returned`; `When_business_A_sets_its_icon/Then_business_B_keeps_its_own` (tenant isolation).
- `When_endpoints_are_mapped/Then_every_endpoint_requires_a_permission_or_is_anonymous` covers the new endpoint.

## Part 3 — Business icon (app)

- `businessIcons.ts` in `features/business`: key → `Icon` name and Spanish label (`businessIcons.<key>`: Natación, Yoga, Pilates, Funcional, Artes marciales, Tenis / pádel, Gimnasia, Running, Fútbol, Música, Arte, Idiomas / apoyo escolar, General).
- `Icon` gains the glyphs: `self-improvement`, `accessibility-new`, `fitness-center`, `sports-martial-arts`, `sports-tennis`, `sports-gymnastics`, `directions-run`, `sports-soccer`, `music-note`, `brush`, `school`, `event-available` (all checked in the MaterialIcons glyph map). The `brand` name becomes `appMark`, used only by `BrandMark`.
- `BusinessIcon` component (`features/business/components`): the current business's icon in the same rounded square as `BrandMark`; `general` for an unknown key.
- **Ajustes → Negocio** gains an "Icono del negocio" section: a grid of the catalog (icon + name), the current one selected; tapping saves (`PUT /api/business/icon`) and refreshes the business query. Only shown to members with `Business.Manage`.
- The business card at the top of Ajustes and the class chip in `StudentClasses` use `BusinessIcon` instead of the fixed `brand` glyph.

### Tests (write first)

- Tapping an icon in Ajustes → Negocio sends its key and marks it selected.
- The Ajustes business card shows the business's icon.
- An unknown key shows the `general` icon.

## Part 4 — Business theme

### API

| Operation | Endpoint | Use case | Permission |
|---|---|---|---|
| Read | `GET /api/business` gains `themeColor` (`#rrggbb` or `null`) | `GetCurrentBusinessUseCase` | `Business.View` |
| Set or remove | `PUT /api/business/theme-color` `{ "themeColor": "#c2185b" }` (`null` removes it) | `SetBusinessThemeColorUseCase` | `Business.Manage` |

- `Business.ThemeColor` (`nvarchar(7)`, nullable). `SetThemeColor` accepts `null` or `#rrggbb` and stores it lowercase.
- Only the seed color is stored, not the derived theme. Contrast is the app's job (it owns the tokens); the API only checks the format.
- Migration `AddBusinessThemeColor`, additive (nullable column).

| # | Rule | Result |
|---|---|---|
| T1 | `themeColor` is `null` or `#` + 6 hex digits | Validation `400` |

### App

- `deriveThemeFromColor(color)` in `src/theme`: neutral grays (like `violet` and `ocean`) plus the `primary*` tokens built from the color. Light: `primary` is the color darkened until `primary-foreground` (white) on it reaches 4.5:1; `primary-strong` darker, `primary-soft` a pale tint, `primary-soft-foreground` a dark shade. Dark: the mirror (a light `primary` with a dark foreground, a deep `primary-soft`). The result goes through `parseThemeDefinition`; if it still fails, the business theme is not offered and the app keeps the user's theme.
- `CurrentBusinessProvider` registers it with `ThemeProvider` as `customThemes={{ business: theme }}` when `themeColor` is set, named "Colores de <negocio>" in the picker.
- **Default:** a user with no stored theme preference gets `business` when it exists, otherwise `aqua`. A stored choice always wins. Removing the business color falls back to `aqua` for whoever had `business` selected.
- **Ajustes → Negocio → "Colores del negocio"** (only with `Business.Manage`): a row of suggested colors plus a `#rrggbb` field, a live preview (button, chip, card in light and dark) and **Guardar** / **Quitar**. The preview uses the same derivation, so what the owner sees is what members get.
- Before sign-in there is no business, so auth screens use the user's stored theme or `aqua`.

### Tests (write first)

- App: `When_theme_is_derived_from_any_color/Then_every_pair_meets_contrast_minimum` over a spread of colors (very light, very dark, saturated, gray); `When_business_has_a_theme_color/Then_it_is_offered_and_is_the_default`; `When_user_chose_another_theme/Then_business_theme_does_not_override_it`; `When_business_color_is_removed/Then_aqua_is_used`; saving from Ajustes → Negocio sends the color.
- API: unit `When_theme_color_is_malformed/Then_it_is_rejected`, `When_theme_color_is_null/Then_it_is_removed`; integration `When_owner_sets_the_theme_color/Then_get_business_returns_it` and `When_business_A_sets_its_theme_color/Then_business_B_keeps_its_own`.

## Delivery

Pull requests, in order; each one leaves the app working:

1. Themes (Part 1).
2. Business icon and theme color in the API (Parts 2 and 4, API).
3. Business icon in the app (Part 3).
4. Business theme in the app (Part 4, app).

Adding the custom logo later changes only `BusinessIcon` (show the image when the business has one) and adds an upload in Ajustes → Negocio.
