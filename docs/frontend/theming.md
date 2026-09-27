# Theming and design tokens

The app supports several visual themes, each in light and dark, without touching screens. Styles are organized in three layers:

1. **Palette** (`app/src/theme/palette.ts`) — raw colors. Only themes use it.
2. **Semantic tokens** (`app/src/theme/themeColorTokens.ts`) — colors named by intent: `background`, `surface`, `foreground`, `muted-foreground`, `primary`, `primary-foreground`, `danger`, `warning-soft`, `inverse`, ... Every theme defines every token (enforced by the `ThemeColors` type).
3. **UI primitives** (`app/src/ui/`) — `AppText`, `Icon`, `Spinner`, `Button`, `Chip`, `TextField`, ... Screens compose primitives and semantic classes; they never pick raw colors.

## How it works

- `themes.ts` defines each theme as `{ light, dark }` maps of token → hex. Today: `violet` (default) and `ocean`.
- `ThemeProvider` (root layout) resolves the color scheme (`system` by default, or a forced `light` / `dark`), turns the active colors into CSS variables (`--color-primary: 124 58 237`) with NativeWind's [`vars()`](https://www.nativewind.dev/docs/api/vars) and sets them on a root `View`, so Android, iOS and web inherit them. It also feeds the navigation theme, so headers and the tab bar follow the theme.
- `tailwind.config.js` maps each token to `rgb(var(--color-<token>) / <alpha-value>)`: `bg-surface`, `text-muted-foreground`, `border-border-strong`, and opacity modifiers such as `border-danger/40` work.
- `useTheme()` exposes `colors` (hex values for props that are not classes: `placeholderTextColor`, `ActivityIndicator color`, icon colors), `colorScheme`, `setThemeName` and `setColorSchemePreference`.

## Typography

`AppText` replaces `Text`. It takes:

- `variant` — size and weight preset: `display`, `headline`, `title`, `heading`, `lead`, `body`, `bodyStrong`, `link`, `label`, `caption`, `footnote`, `badge`.
- `tone` — theme color: `default`, `muted`, `subtle`, `disabled`, `primary`, `onPrimary`, `danger`, `warning`, `success`, `inverse`, ...

To change fonts, load them with the `expo-font` config plugin and add the family to the variants in `AppText.tsx`: every text in the app goes through it.

## Icons

`Icon` wraps Ionicons (`@expo/vector-icons`) behind semantic names (`today`, `classes`, `students`, `fees`, `settings`, `add`, `previous`, `next`, `paid`). Add a name to `iconGlyphs` to use a new icon; swapping the icon set only touches `Icon.tsx`. Icons without `accessibilityLabel` are decorative and hidden from screen readers.

## Rules enforced by lint

`app/eslint/themeRules.js` and `eslint.config.js` make the build fail when app code (outside `src/theme/`) bypasses the system:

- `theme/no-raw-colors` — palette classes (`bg-gray-50`, `text-red-600`, `bg-white`) and literal colors (`#7c3aed`, `rgb(...)`).
- `no-restricted-imports` — `Text` and `ActivityIndicator` from `react-native` and anything from `@expo/vector-icons` outside `src/ui/`; use `AppText`, `Spinner` and `Icon`.

## Adding a theme

1. Add any missing raw colors to `palette.ts`.
2. Add an entry to `themes` in `themes.ts` with `light` and `dark` colors. TypeScript fails if a token is missing.
3. Run `pnpm test`: `When_any_theme_is_defined` checks WCAG AA contrast (4.5:1) for every text/background pair in `themeContrastPairs`.

## Adding a token

1. Add it to `themeColorTokens` and give it a value in every theme.
2. Add it to the token list in `tailwind.config.js`. `When_tailwind_config_is_loaded` fails until both lists match (the config is plain CommonJS and cannot import TypeScript).
3. If text is drawn on it, add the pair to `themeContrastPairs`.

## Not done yet

- The chosen theme and color scheme are not persisted, and there is no settings screen to pick them; `useTheme()` already exposes the setters.
- A per-business brand color (from the API) would be one more theme built at runtime and passed to `ThemeProvider`.

## Why NativeWind variables

NativeWind v4 is already the styling layer and is stable on the three platforms. NativeWind v5 (Tailwind v4) is a release candidate and replaces `vars()` with `VariableContextProvider`; only `ThemeProvider` changes when upgrading. Tamagui, Unistyles or Restyle have good theming but would mean rewriting every style.
