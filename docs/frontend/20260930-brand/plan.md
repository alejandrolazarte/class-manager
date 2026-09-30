# Plan — Brand: logo, brand theme and welcome screen

Follow-up to the Claude Design exploration "Marca Propia" (Ajustes → Marca, the branded student app and its welcome screen). It delivers the "business theme" of [themes and business icon](../20260928-themes-and-business-icon/plan.md) (Part 4), the accent color and the theme lock from [branding and plans](../../branding-and-plans.md), and the custom logo that plan left out of scope.

## What each person sees

| Who | Where the brand shows |
|---|---|
| Owner (`Business.Manage`) | **Ajustes → Marca**: logo, name students see, main color, optional accent, live preview, "Usar siempre mi marca", "Ver pantalla de bienvenida". The business card at the top of Ajustes shows the logo. |
| Owner and coaches | Welcome screen when the app opens, the brand theme, and the logo + brand name at the top of **Alumnos** (the screen the team app opens on). |
| Families and students | Welcome screen when the app opens, the brand theme, and the logo + brand name at the top of **Inicio**. |

Before sign-in there is no business, so sign-in, welcome and password screens keep the app mark (`BrandMark`).

## Decisions

- **The brand belongs to the business (the tenant),** like the rest of its settings. Branches that want to share a brand configure it in each one; an organization-wide brand waits for [branches](../../backend/20260928-roles-and-permissions/plan.md) to need it.
- **Only the seed colors are stored** (`ThemeColor`, `AccentColor`, `#rrggbb` lowercase). The app derives every token (`deriveBrandTheme`) so each text pair passes WCAG AA in light and dark: it darkens (light) or lightens (dark) the color until white or near-black text reads on it. The screen says so ("Lo oscurecimos un poco…") when the color changed.
- **Accent tokens** (`accent`, `accent-soft`, `accent-soft-foreground`) are new. Built-in themes set them to their `primary*` values, so nothing changes for them. The accent is used for small highlights only: the eyebrow above screen titles and chips; never for buttons.
- **A color close to a status hue is allowed but warned** (`findBrandColorClash`): red, amber and green mean cancelled, pending and paid.
- **Default and lock:** a user who never chose a theme gets the brand theme ("Marca" in the picker); a stored choice wins. With **"Usar siempre mi marca"** the brand theme wins over any choice and the picker shows it is locked. Locking needs a main color. Removing the colors falls back to each user's choice or `aqua`.
- **Theme preference storage** now records whether the theme was chosen. Values saved by older versions with the default `aqua` count as "not chosen", because the old code saved it without the user picking it.
- **The logo is stored in the database** (`BrandLogos`, one row per business, `varbinary(max)`), not in blob storage: a few hundred KB per business fits the [hosting plan](../../hosting-plan.md). PNG, JPG or WebP up to **512 KB**, checked by signature (magic bytes), not by the file name or declared type. `Businesses.LogoUpdatedAt` is the logo version the app caches by, so `GET` of the brand never carries the image.
- **The app downloads the logo with the access token** and shows it as a data URI, because web `<img>` requests cannot carry the `Authorization` header.
- **Welcome screen:** logo, brand name and "¡Qué bueno verte!" on the brand color, for 1.6 s each time the app opens (after the brand loads; tap to skip). It uses neutral wording because the team token carries no name.
- **No paid gate yet.** The design marks the lock as PRO; pricing is still an [open decision](../../mvp-plan.md#open-decisions), so during the pilot everything is available.

## API

| Operation | Endpoint | Permission |
|---|---|---|
| Read brand (team) | `GET /api/business/brand` | `Business.View` |
| Read brand (family) | `GET /api/family/brand` | family |
| Read logo | `GET /api/business/brand/logo`, `GET /api/family/brand/logo` (`404` without logo) | same as above |
| Save name, colors, lock | `PUT /api/business/brand` `{ brandName, themeColor, accentColor, locksTheme }` | `Business.Manage` |
| Upload logo | `PUT /api/business/brand/logo` (multipart, field `file`) | `Business.Manage` |
| Remove logo | `DELETE /api/business/brand/logo` | `Business.Manage` |

The brand response is `{ displayName, brandName, themeColor, accentColor, locksTheme, logoUpdatedAt }`; `displayName` is the brand name or, when empty, the business name.

| # | Rule | Result |
|---|---|---|
| B1 | Colors are `null` or `#` + 6 hex digits | Validation `400` |
| B2 | Brand name empty (stored as `null`) or 2–120 characters | Validation `400` |
| B3 | Locking needs a main color | Validation `400` on `LocksTheme` |
| B4 | Logo is PNG, JPG or WebP by signature | `400` `brand.unsupported_logo` |
| B5 | Logo is at most 512 KB | `400` `brand.logo_too_large` |
| B6 | Only the current business changes and is read | Tenant from the token; `BrandLogos` is tenant-owned |

Migration `AddBusinessBrand` is additive: nullable columns on `Businesses`, `LocksTheme` defaulting to `false`, and the new `BrandLogos` table.

## Tests

- Unit (domain): malformed color, colors stored lowercase, blank brand name falls back to the business name, lock without a color, logo not an image, logo too large, PNG detected.
- Integration: owner saves the brand and reads it back; malformed color `400`; business A's brand and logo stay invisible to business B; logo upload/download and removal; non-image `400`; a family reads the school's brand and logo; a coach cannot save it (`403`).
- App: derived themes pass AA for light, dark, saturated and gray seeds; no accent follows the main color; too-light color is darkened; clash detection; brand theme is the default, doesn't override a choice, overrides it when locked, and falls back when removed; old stored `aqua` is "not chosen"; saving from Ajustes → Marca sends colors and lock; an oversized logo is not uploaded; the welcome shows the brand; the header shows the logo; the brand theme is applied.
- Screenshots: `e2e/screenshots/Brand_screens.capture.ts` uploads a logo through the UI, saves colors and captures Ajustes → Marca, the welcome screen and the branded team and family screens.
