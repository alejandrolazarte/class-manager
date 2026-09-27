# Theming and design tokens

The app supports several visual themes, each in light and dark, without touching screens. Styles are organized in four layers:

1. **Palette** (`app/src/theme/palette.ts`) — raw colors. Only themes use it.
2. **Semantic tokens** (`app/src/theme/themeColorTokens.ts`) — colors named by intent: `background`, `surface`, `foreground`, `muted-foreground`, `primary`, `primary-foreground`, `danger`, `success-foreground`, `warning-soft`, `inverse`, `shadow`, ... Every theme defines every token (enforced by the `ThemeColors` type).
3. **Typography** (`app/src/theme/typography.ts`) — font roles (`text`, `label`, `strong`, `heavy`) mapped to font families (Nunito 600/700/800/900 today) and loaded in the root layout with `expo-font`.
4. **UI primitives** (`app/src/ui/`) — `AppText`, `Icon`, `Button`, `IconButton`, `Chip`, `SegmentedControl`, `TextField`, `SearchInput`, `NumberStepper`, `ToggleSwitch`, `Card`, `Avatar`, `StatusPill`, `ProgressBar`, `Banner`, `ListRow`, `EmptyState`, `SectionTitle`, `Screen` / `ScrollScreen`, `ScreenHeader`, `FloatingActionButton`, `BrandMark`. Screens compose primitives and semantic classes; they never pick raw colors, font families or sizes.

## Visual language

The default theme, `aqua`, comes from the Claude Design handoff "Brazada App" (fresh, aquatic, rounded):

- Light: pale water-blue background, white cards with a soft blue-tinted shadow, lagoon-blue primary. Dark: deep navy surfaces, cyan primary, no shadows.
- Nunito for all text: 900 for screen titles and amounts, 800 for names and buttons, 700 for labels, 600 for body copy.
- Material Icons (`@expo/vector-icons/MaterialIcons`) behind semantic names.
- Large radii: 16px inputs and buttons, 18–20px cards, 24px highlight cards, pills for chips and status.
- Screens have their own header (`ScreenHeader`): optional back / close button, a small primary "eyebrow", a 26px title and an optional subtitle. Native stack headers are hidden; the `Stack.Screen` titles stay for the web document title.

The design's oklch colors were converted to hex. A few light-mode values were darkened slightly so every text pair reaches WCAG AA (4.5:1).

## How it works

- `themes.ts` defines each built-in theme as `{ light, dark }` maps of token → hex. Today: `aqua` (default), `violet` and `ocean`.
- `ThemeProvider` (root layout) merges the built-in themes with optional `customThemes`, resolves the color scheme (`system` by default, or a forced `light` / `dark`), turns the active colors into CSS variables (`--color-primary: 0 118 180`) with NativeWind's [`vars()`](https://www.nativewind.dev/docs/api/vars) and sets them on a root `View`, so Android, iOS and web inherit them. It also feeds the navigation theme, so the tab bar follows the theme.
- `tailwind.config.js` maps each token to `rgb(var(--color-<token>) / <alpha-value>)` (`bg-surface`, `text-muted-foreground`, `bg-primary/20`) and each font role to a family (`font-strong`, `font-heavy`).
- `useTheme()` exposes `colors` (hex values for props that are not classes: `placeholderTextColor`, icon colors, shadows), `colorScheme`, `themeName`, `themeNames`, `previewColors(themeName)`, `setThemeName` and `setColorSchemePreference`.
- `useElevationStyle()` (`src/ui/elevation.ts`) builds card and floating shadows from the `shadow` / `primary` tokens and drops card shadows in dark mode.
- `PersistedThemePreferences` (root layout) restores and saves the chosen theme and color scheme (`expo-secure-store` on native, `localStorage` on web). **Ajustes → Apariencia / Colores** lets the owner pick them.

## Typography

`AppText` replaces `Text`. It takes:

- `variant` — size, weight and tracking preset: `hero`, `display`, `headline`, `amount`, `title`, `heading`, `lead`, `body`, `bodyStrong`, `button`, `link`, `eyebrow`, `label`, `caption`, `footnote`, `badge`, `overline`.
- `tone` — theme color: `default`, `muted`, `subtle`, `disabled`, `primary`, `primarySoft`, `onPrimary`, `danger`, `warning`, `success`, `onSuccess`, `inverse`, ...

To change fonts, update the families and assets in `typography.ts` and the `fontFamilies` list in `tailwind.config.js`; `When_tailwind_config_is_loaded` fails until both match.

## Icons

`Icon` wraps Material Icons behind semantic names (`today`, `classes`, `students`, `fees`, `settings`, `brand`, `present`, `absent`, `enroll`, `cash`, ...). Add a name to `iconGlyphs` to use a new icon; swapping the icon set only touches `Icon.tsx`. Icons without `accessibilityLabel` are decorative and hidden from screen readers.

## Rules enforced by lint

`app/eslint/themeRules.js` and `eslint.config.js` make the build fail when app code (outside `src/theme/`) bypasses the system:

- `theme/no-raw-colors` — palette classes (`bg-gray-50`, `text-red-600`, `bg-white`) and literal colors (`#7c3aed`, `rgb(...)`).
- `no-restricted-imports` — `Text` and `ActivityIndicator` from `react-native` and anything from `@expo/vector-icons` outside `src/ui/`; use `AppText`, `Spinner` and `Icon`.

## Adding a built-in theme

1. Add any missing raw colors to `palette.ts`.
2. Add an entry to `themes` in `themes.ts` with `light` and `dark` colors. TypeScript fails if a token is missing.
3. Add its display name as `themes.<name>` in `src/i18n/es.json` (the picker falls back to the theme id).
4. Run `pnpm test`: `When_any_theme_is_defined` checks WCAG AA contrast (4.5:1) for every text/background pair in `themeContrastPairs`.

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
