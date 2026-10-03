# Theming and design tokens

The app supports several visual themes, each in light and dark, without touching screens. Styles are organized in four layers:

1. **Palette** (`app/src/theme/palette.ts`) — raw colors. Only themes use it.
2. **Semantic tokens** (`app/src/theme/themeColorTokens.ts`) — colors named by intent: `background`, `surface`, `foreground`, `muted-foreground`, `primary`, `primary-foreground`, `danger`, `success-foreground`, `warning-soft`, `inverse`, `shadow`, ... Every theme defines every token (enforced by the `ThemeColors` type).
3. **Typography** (`app/src/theme/typography.ts`) — font roles (`text`, `label`, `strong`, `heavy`) mapped to font families (Nunito 600/700/800/900 today) and loaded in the root layout with `expo-font`.
4. **UI primitives** (`app/src/ui/`) — `AppText`, `Icon`, `Button`, `IconButton`, `Chip`, `SegmentedControl`, `TextField`, `SearchInput`, `NumberStepper`, `ToggleSwitch`, `Card`, `Avatar`, `StatusPill`, `ProgressBar`, `Banner`, `ListRow`, `EmptyState`, `SectionTitle`, `Screen` / `ScrollScreen`, `ScreenHeader`, `FloatingActionButton`, `DayStrip`, `BrandMark`. Screens compose primitives and semantic classes; they never pick raw colors, font families or sizes.

## Visual language

The default theme, `aqua`, comes from the Claude Design handoff "Brazada App" (fresh, aquatic, rounded):

- Light: pale water-blue background, white cards with a soft blue-tinted shadow, lagoon-blue primary. Dark: deep navy surfaces, cyan primary, no shadows.
- Nunito for all text: 900 for screen titles and amounts, 800 for names and buttons, 700 for labels, 600 for body copy.
- Material Icons (`@expo/vector-icons/MaterialIcons`) behind semantic names.
- Large radii: 16px inputs and buttons, 18–20px cards, 24px highlight cards, pills for chips and status.
- Screens have their own header (`ScreenHeader`): optional back / close button, a small primary "eyebrow", a 26px title (`display`) and an optional subtitle. Native stack headers are hidden; the `Stack.Screen` titles stay for the web document title.

The design's oklch colors were converted to hex. A few light-mode values were darkened slightly so every text pair reaches WCAG AA (4.5:1).

## How it works

- `themes.ts` defines each built-in theme as `{ light, dark }` maps of token → hex. Today: `aqua` (default, Agua), `violet` (Violeta), `ocean` (Océano), `rose` (Rosa), `plum` (Ciruela), `indigo` (Índigo) and `graphite` (Grafito). Every theme except `aqua` uses neutral grays and only changes the `primary*` tokens. Theme names describe colors, never an activity.
- `ThemeProvider` (root layout) merges the built-in themes with optional `customThemes`, resolves the color scheme (`system` by default, or a forced `light` / `dark`), turns the active colors into CSS variables (`--color-primary: 0 118 180`) with NativeWind's [`vars()`](https://www.nativewind.dev/docs/api/vars) and sets them on a root `View`, so Android, iOS and web inherit them. It also feeds the navigation theme, so the tab bar follows the theme.
- `tailwind.config.js` maps each token to `rgb(var(--color-<token>) / <alpha-value>)` (`bg-surface`, `text-muted-foreground`, `bg-primary/20`) and each font role to a family (`font-strong`, `font-heavy`).
- `accent*` tokens are for small highlights (the eyebrow above titles, chips), never for buttons. Built-in themes set them to their `primary*` values.
- `useTheme()` exposes `colors` (hex values for props that are not classes: `placeholderTextColor`, icon colors, shadows), `colorScheme`, `themeName`, `themeNames`, `previewColors(themeName)`, `setThemeName` and `setColorSchemePreference`.
- `useElevationStyle()` (`src/ui/elevation.ts`) builds card and floating shadows from the `shadow` / `primary` tokens and drops card shadows in dark mode.
- `PersistedThemePreferences` (root layout) restores and saves the chosen theme and color scheme (`expo-secure-store` on native, `localStorage` on web). **Ajustes → Apariencia / Colores** lets each user pick them for their own device, in a grid of three swatches per row.

## Typography

Every text size comes from one type scale, `typeScale` in `typography.ts`. Each step is a font size with its line height:

| Step | Size / line height | Variants |
|---|---|---|
| `hero` | 32 / 36 | `hero`, `amount` |
| `display` | 26 / 30 | `display` |
| `title` | 20 / 24 | `headline` |
| `large` | 17 / 22 | `heading`, `lead`, `button`, `input` |
| `body` | 15 / 21 | `body`, `bodyStrong` |
| `caption` | 13 / 17 | `link`, `eyebrow`, `label`, `caption` |
| `small` | 12 / 16 | `footnote`, `badge`, `overline` |
| `micro` | 11 / 14 | `counter` (numbers in small badges) |

Steps are at least 2px apart from `caption` up: two texts that differ by one pixel don't read as a hierarchy, they look like a mistake. `small` and `micro` only go in compact spots (badges, footnotes) where the next step does not fit.

`tailwind.config.js` replaces Tailwind's font sizes with these steps (`text-body`, `text-large`, ...), so `text-base`, `text-xl` and the rest don't exist. `When_tailwind_config_is_loaded` fails until both lists match.

`AppText` replaces `Text`. It takes:

- `variant` — font role, size step and tracking: `hero`, `amount`, `display`, `headline`, `heading`, `lead`, `button`, `input`, `body`, `bodyStrong`, `link`, `eyebrow`, `label`, `caption`, `footnote`, `badge`, `overline`, `counter`.
- `tone` — theme color: `default`, `muted`, `subtle`, `disabled`, `primary`, `primarySoft`, `onPrimary`, `danger`, `warning`, `success`, `onSuccess`, `inverse`, ...

Text inputs use the same variants through `textVariantClassNames` (`input` for `TextField`, `SearchInput` and `NumberStepper`, `amount` for `AmountField`), so typed text matches across forms. Styles that are not classes (the tab bar) read `typeScale` directly.

The same element always uses the same variant: row and card names `bodyStrong`, the line under them `caption`, the identity card at the top of Ajustes or an account `heading`, sheet titles and totals `headline`. Screens may change the weight (`font-heavy`) or the color, never the size: when a new size is really needed, add a variant to `AppText`.

To change fonts, update the families and assets in `typography.ts` and the `fontFamilies` list in `tailwind.config.js`; `When_tailwind_config_is_loaded` fails until both match.

## Icons

`Icon` wraps Material Icons behind semantic names (`today`, `classes`, `students`, `fees`, `settings`, `brand`, `present`, `absent`, `enroll`, `cash`, ...). Add a name to `iconGlyphs` to use a new icon; swapping the icon set only touches `Icon.tsx`. Icons without `accessibilityLabel` are decorative and hidden from screen readers.

## Rules enforced by lint

`app/eslint/themeRules.js` and `eslint.config.js` make the build fail when app code (outside `src/theme/`) bypasses the system:

- `theme/no-raw-colors` — palette classes (`bg-gray-50`, `text-red-600`, `bg-white`) and literal colors (`#7c3aed`, `rgb(...)`).
- `theme/no-raw-font-sizes` — size and line-height classes (`text-base`, `text-[17px]`, `text-large`, `leading-4`) and numeric `fontSize` / `lineHeight` styles outside `AppText.tsx`; use an `AppText` variant, or `typeScale` for styles that are not classes.
- `no-restricted-imports` — `Text` and `ActivityIndicator` from `react-native` and anything from `@expo/vector-icons` outside `src/ui/`; use `AppText`, `Spinner` and `Icon`.

## Adding a built-in theme

1. Add any missing raw colors to `palette.ts`.
2. Add an entry to `themes` in `themes.ts` with `light` and `dark` colors. TypeScript fails if a token is missing.
3. Add its display name as `themes.<name>` in `src/i18n/es.json` (the picker falls back to the theme id).
4. Run `pnpm test`: `When_any_theme_is_defined` checks WCAG AA contrast (4.5:1) for every text/background pair in `themeContrastPairs`, and that the primary color is not mistaken for a status color (below).

## Primary colors never look like a status

Red means an error or a destructive action (Cancelar, Eliminar), amber a warning, green paid or present. If the primary color, used on every button and selected item, shares one of those hues, the owner can't tell a normal action from a dangerous one, or "paid" from any other highlight.

`findStatusHueClashes` (`themeStatusHues.ts`) rejects a theme whose `primary` hue is less than 35° from its `danger`, `warning` or `success` hue, in light or dark. Near-gray primaries (saturation below 0.3, like Grafito) are exempt: they carry no hue to confuse. This is why there are no red, orange, yellow or green built-in themes; warm brand colors fit better as an accent (see [branding and plans](../branding-and-plans.md)).

## The brand theme

A business sets its main color and an optional accent in **Ajustes → Marca** ([brand plan](20260930-brand/plan.md)). `BrandProvider` (team and family layouts) reads them from the API, builds the theme with `deriveBrandTheme` (neutral grays plus `primary*` and `accent*` derived so every pair passes AA) and registers it with `setBrandTheme` under the name `business`:

- A user who never chose a theme gets it; a stored choice (`chosenThemeName`) wins.
- When the business locks its brand, `isThemeLocked` is true, the brand theme wins over any choice and the picker only explains it.
- Signing out or removing the colors unregisters it; the user's choice or `aqua` applies again.

`ThemePreviewScope` applies another set of colors (and the matching `useTheme().colors`) to a subtree; Ajustes → Marca uses it for the live preview and the welcome preview.

## Themes created by a business

A business can bring its own theme without a code change:

1. Receive the theme as JSON (for example from the API): `{ "light": { "<token>": "#rrggbb", ... }, "dark": { ... } }`.
2. Validate it with `parseThemeDefinition(json)`. It returns `{ isValid: true, theme }`, or `{ isValid: false, problems }` listing missing or malformed tokens (`light.primary: expected a #rrggbb color`) and text pairs below AA contrast (`light primary-foreground on primary`).
3. Pass valid themes to `ThemeProvider` as `customThemes={{ "studio-brand": theme }}`. They appear in `themeNames`, in the Ajustes color picker (named by their id unless a translation exists) and can be selected with `setThemeName`.

A theme editor would only need to produce that JSON; screens and primitives already read everything from tokens.

## Adding a token

1. Add it to `themeColorTokens` and give it a value in every theme.
2. Add it to the token list in `tailwind.config.js`. `When_tailwind_config_is_loaded` fails until both lists match (the config is plain CommonJS and cannot import TypeScript).
3. If text is drawn on it, add the pair to `themeContrastPairs`.

## Why NativeWind variables

NativeWind v4 is already the styling layer and is stable on the three platforms. NativeWind v5 (Tailwind v4) is a release candidate and replaces `vars()` with `VariableContextProvider`; only `ThemeProvider` changes when upgrading. Tamagui, Unistyles or Restyle have good theming but would mean rewriting every style.
