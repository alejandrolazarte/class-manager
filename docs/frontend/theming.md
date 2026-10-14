# Theming and design tokens

The app supports several visual themes, each in light and dark, without touching screens. Styles are organized in four layers:

1. **Palette** (`app/src/theme/palette.ts`) — raw colors. Only themes use it.
2. **Semantic tokens** (`app/src/theme/themeColorTokens.ts`) — colors named by intent: `background`, `surface`, `foreground`, `muted-foreground`, `primary`, `primary-foreground`, `danger`, `success-foreground`, `warning-soft`, `inverse`, ... Every theme defines every token (enforced by the `ThemeColors` type).
3. **Typography** (`app/src/theme/typography.ts`) — font roles (`text`, `label`, `strong`, `heavy`) mapped to font families (Figtree 400/500/600/700) and loaded in the root layout with `expo-font`.
4. **UI primitives** (`app/src/ui/`) — `AppText`, `Icon`, `IconCircle`, `Button`, `IconButton`, `TonalButton`, `ActionTile`, `Chip`, `SegmentedControl`, `Tabs`, `TextField`, `SearchInput`, `NumberStepper`, `ToggleSwitch`, `Avatar`, `StatusPill`, `StatusText`, `ProgressBar`, `Banner`, `Highlight`, `TonalBlock`, `ListRow`, `HairlineRow`, `HairlineSection`, `HairlineColumns`, `TimelineRow`, `BulletRow`, `CountsRow`, `SummaryBlock`, `EmptyState`, `SectionTitle`, `Screen` / `ScrollScreen`, `ScreenHeader`, `FloatingActionButton`, `DayStrip`, `MonthCalendar`, `ChipGroup`, `BrandMark` (the Laztty mark, [Laztty brand](../brand.md)), `DoodlePattern` (the Laztty doodle pattern, [Laztty brand](../brand.md#doodle-pattern)), and the motion primitives in `app/src/ui/motion/` ([Motion](#motion)). Screens compose primitives and semantic classes; they never pick raw colors, font families or sizes, and never animate on their own.

## Visual language

The look is **Calma** ([redesign plan](20261011-calma-redesign/plan.md)). The default theme is `violet`, the violet of the Laztty logo, on neutral grays. The `aqua` theme keeps the colors of the Claude Design handoff "Brazada App":

- Default (`violet`): light gray background, white surfaces, violet primary; dark: near-black surfaces, light violet primary. `aqua`: pale water-blue background and lagoon-blue primary; dark: deep navy surfaces, cyan primary.
- Figtree for all text: 600 for screen titles, amounts, names and buttons, 500 for labels and the tab bar, 400 for body copy and typed text, 700 only for initials and the numbers in badges.
- No cards: list items are rows separated by 1px `border-subtle` hairlines, flush with the screen padding (`HairlineRow`, `ListRow` with `ListDivider`, `TimelineRow`), and groups get a small section title (`SectionTitle` with `isOverline`: 12 px, 600, sentence case). The parts of a form or a detail (the photos of a product, its stock, each color in Marca, each extra student) are a `HairlineSection`: a hairline above, an optional title and hint, and the content. Switches (`ToggleSwitch`) are flat rows. Only floating elements (the floating button, the cart bar, the large brand mark) keep a shadow.
- Agenda rows are a timeline (`TimelineRow`): start and end time, a dot on a vertical line (filled and pulsing when live, filled in the status color when done or pending, hollow when upcoming or cancelled), title, meta and the status as colored text.
- Summaries (Cuotas, Pedidos) are a `SummaryBlock`: a label, a large number with the total next to it, a thin progress bar (green when complete) and status lines in color.
- Highlights and banners are tonal: `Highlight` (the live or next class in Inicio, 14 px radius) and `Banner` (12 px radius, the tone's soft background and text, no border). Inset content (notes, order payment and refund, the invite forms, grouped packs, the counter sale summary, the current plan, an attached class file) sits in a `TonalBlock` with the `muted` tone. Attendance counts are a `CountsRow` built on `HairlineColumns`.
- Selection: chip groups slide a dark `inverse` thumb under borderless chips; `Tabs` are text with a 2px `foreground` underline; the compact `SegmentedControl` (Día / Mes) is text with a short sliding line.
- Outline icons (Ionicons `-outline` glyphs from `@expo/vector-icons/Ionicons`, thin stroke with round ends, as the design draws them) behind semantic names, 20–22px in rows, buttons and the tab bar. No filled glyphs.
- Radii: 10px inputs, 12px buttons and banners, 14px highlights and photos, 20px sheets, pills for chips, status and the floating button.
- Sizes: buttons 48px (`large`) and 44px (`medium`), inputs 46px (the amount input 52px), icon buttons 44px (40px outlined, 36px compact for steppers such as month and week arrows), floating button 48px.
- Tab bar: the active tab takes the primary color and a small dot under its icon; the badge stays red.
- Screens have their own header (`ScreenHeader`): optional back / close button, a small primary "eyebrow", a 24px title (`display`) and an optional subtitle. Native stack headers are hidden; the `Stack.Screen` titles stay for the web document title.

The design's oklch colors were converted to hex. A few light-mode values were darkened slightly so every text pair reaches WCAG AA (4.5:1).

## How it works

- `themes.ts` defines each built-in theme as `{ light, dark }` maps of token → hex. Today: `violet` (default, Violeta, the Laztty color), `aqua` (Agua), `ocean` (Océano), `rose` (Rosa), `plum` (Ciruela), `indigo` (Índigo) and `graphite` (Grafito). Every theme except `aqua` uses neutral grays and only changes the `primary*` tokens. Theme names describe colors, never an activity.
- `ThemeProvider` (root layout) merges the built-in themes with optional `customThemes`, resolves the color scheme (`system` by default, or a forced `light` / `dark`), turns the active colors into CSS variables (`--color-primary: 0 118 180`) with NativeWind's [`vars()`](https://www.nativewind.dev/docs/api/vars) and sets them on a root `View`, so Android, iOS and web inherit them. It also feeds the navigation theme, so the tab bar follows the theme.
- `tailwind.config.js` maps each token to `rgb(var(--color-<token>) / <alpha-value>)` (`bg-surface`, `text-muted-foreground`, `bg-primary/20`) and each font role to a family (`font-strong`, `font-heavy`).
- `accent*` tokens are for small highlights (the eyebrow above titles, chips), never for buttons. Built-in themes set them to their `primary*` values.
- `useTheme()` exposes `colors` (hex values for props that are not classes: `placeholderTextColor`, icon colors, shadows), `colorScheme`, `colorSchemePreference`, `themeName` (`business` when the business theme applies), `setBrandTheme` and `setColorSchemePreference`.
- `useFloatingShadowStyle()` (`src/ui/elevation.ts`) builds the shadow of floating elements from the `primary` token. Nothing else has a shadow.
- `PersistedThemePreferences` (root layout) restores and saves the color scheme (`expo-secure-store` on native, `localStorage` on web); a theme name stored by older versions is ignored. **Ajustes → Apariencia** only offers Automático / Claro / Oscuro: the colors are the business's ([branding and plans](../branding-and-plans.md)).

## Typography

Every text size comes from one type scale, `typeScale` in `typography.ts`. Each step is a font size with its line height:

| Step | Size / line height | Variants |
|---|---|---|
| `hero` | 28 / 34 | `hero`, `amount` |
| `display` | 24 / 30 | `display` |
| `title` | 20 / 24 | `headline` |
| `large` | 17 / 22 | `heading`, `lead` |
| `body` | 15 / 21 | `body`, `bodyStrong`, `button`, `input` |
| `caption` | 13 / 17 | `link`, `eyebrow`, `label`, `caption` |
| `small` | 12 / 16 | `footnote`, `badge`, `overline` |
| `micro` | 11 / 14 | `pill` (status pills), `counter` (numbers in small badges), `tab` (tab bar labels in the brand preview, day letters in `DayStrip` and `MonthCalendar`; the tab bar itself reads `typeScale`) |

Steps are at least 2px apart from `caption` up: two texts that differ by one pixel don't read as a hierarchy, they look like a mistake. `small` and `micro` only go in compact spots (badges, footnotes) where the next step does not fit.

`tailwind.config.js` replaces Tailwind's font sizes with these steps (`text-body`, `text-large`, ...), so `text-base`, `text-xl` and the rest don't exist. `When_tailwind_config_is_loaded` fails until both lists match.

`AppText` replaces `Text`. It takes:

- `variant` — font role, size step and tracking: `hero`, `amount`, `display`, `headline`, `heading`, `lead`, `button`, `input`, `body`, `bodyStrong`, `link`, `eyebrow`, `label`, `caption`, `footnote`, `badge`, `overline`, `pill`, `counter`, `tab`.
- `tone` — theme color: `default`, `muted`, `subtle`, `disabled`, `primary`, `primarySoft`, `onPrimary`, `danger`, `warning`, `success`, `onSuccess`, `inverse`, ...

Text inputs use the same variants through `textVariantClassNames` (`input` for `TextField`, `SearchInput` and `NumberStepper`, `headline` for `AmountField`), so typed text matches across forms. `amount` is the large number of a `SummaryBlock`. Styles that are not classes (the tab bar) read `typeScale` directly.

The same element always uses the same variant: row and section names `bodyStrong`, the line under them `caption`, the identity row at the top of Ajustes or an account `heading`, sheet titles and totals `headline`. Screens may change the color, never the size or the weight: when a new size is really needed, add a variant to `AppText`. `font-heavy` (700) is only for initials (`Avatar`) and badge numbers (`counter`).

To change fonts, update the families and assets in `typography.ts` and the `fontFamilies` list in `tailwind.config.js`; `When_tailwind_config_is_loaded` fails until both match.

## Motion

Animations are short and live only in `app/src/ui/motion/`, built on Reanimated. Screens ask for a primitive; they never call `withTiming` or `withSpring` themselves.

- `motion.ts` holds every duration (140 ms for a press up to 380 ms for a list row, 800 ms for progress bars, 1.8 s for the live pulse), the one easing curve (`cubic-bezier(.2,.8,.2,1)`) and the soft spring used for toasts and selected marks.
- `useReducedMotion` wraps Reanimated's hook. With the device's "Reduce motion" on, every primitive shows its content in its final state, without movement; tests mock it to prove that.
- Primitives and where they are used:

| Primitive | What it does | Used by |
|---|---|---|
| `PressableScale` | Scales to 0.97 while pressed | `Button`, `ListRow`, `HairlineRow`, `TimelineRow`, `Highlight`, `TonalButton`, `ActionTile`, `Chip`, `FloatingActionButton`, attendance marks |
| `AnimatedListItem` | Fades in and rises 8 px, 45 ms apart, only the first 8 rows | Inicio, Clases, Alumnos, Cuotas, Pedidos, settings lists, avisos, Mis pedidos |
| `SlidingIndicator` + `useOptionLayouts` | Moves the selected background or line between options | `SegmentedControl`, `Tabs`, `DayStrip` (Clases and the Inicio week), `ChipGroup` |
| `GrowingFill` | Grows a bar to its value | `ProgressBar`, `StepProgress` |
| `LivePulse` | A ring that pulses in a loop | The live class in Inicio: the dot of its `Highlight` and of its `TimelineRow` |
| `Entrance` | Fade, scale from 0.96, slide up, spring rise or pop when a view appears | `ConfirmDialog`, `BottomSheet`, `ToastProvider`, the tab bar dot, attendance marks and the row tint behind them, the forgot password confirmation icon |
| `Breathing` | Scales to 94 % and fades to 60 % in a loop, still with reduced motion | The Laztty mark on the loading screen while the session and the business load ([Laztty brand](../brand.md)) |
| `Collapsible` | Opens and closes a panel by height; closing content is hidden from screen readers at once | Asistencia panels, private lesson cancel, order payment and refund |
| `SkeletonList` | Placeholder rows that breathe while a list loads, announced as "Cargando…" | Every list screen while it loads |
| `SkeletonDetail` | A header and field blocks that breathe while a detail or form loads, announced as "Cargando…" (`SkeletonScreen` wraps it in a full screen) | Every detail and form screen while it loads |
| `SkeletonFrame` | The breathing animation and the progress bar role shared by the skeletons | `SkeletonList`, `SkeletonDetail` |

Every screen-level data load shows a skeleton: `SkeletonList` for lists, `SkeletonDetail` for details and forms. A spinner (`Spinner`, or `Button` with `isLoading`) only shows an action in progress, such as a button saving or a row switching, and inside small pickers. `LoadingScreen` is only for app bootstrap.

Stack screens slide in from the right on native (`useStackScreenOptions`, none with reduced motion). Nothing animates while the user reads: numbers in lists stay still, and layouts keep their heights.

`className` works on the animated components through `cssInterop` (`animatedComponents.ts`). Shared values are read and written with `.get()` and `.set()`, as the React Compiler lint requires. Jest resolves `react-native-worklets` with its own resolver (`jest.config.js`).

## Icons

`Icon` wraps Ionicons outline glyphs behind semantic names (`today`, `classes`, `students`, `fees`, `settings`, `brand`, `present`, `absent`, `enroll`, `cash`, ...). Add a name to `iconGlyphs` to use a new icon; swapping the icon set only touches `Icon.tsx`. Icons without `accessibilityLabel` are decorative and hidden from screen readers.

## Rules enforced by lint

`app/eslint/themeRules.js` and `eslint.config.js` make the build fail when app code (outside `src/theme/`) bypasses the system:

- `theme/no-raw-colors` — palette classes (`bg-gray-50`, `text-red-600`, `bg-white`) and literal colors (`#7c3aed`, `rgb(...)`).
- `theme/no-raw-font-sizes` — size and line-height classes (`text-base`, `text-[17px]`, `text-large`, `leading-4`) and numeric `fontSize` / `lineHeight` styles outside `AppText.tsx`; use an `AppText` variant, or `typeScale` for styles that are not classes.
- `no-restricted-imports` — `Text` and `ActivityIndicator` from `react-native` and anything from `@expo/vector-icons` outside `src/ui/`; use `AppText`, `Spinner` and `Icon`.

## Adding a built-in theme

1. Add any missing raw colors to `palette.ts`.
2. Add an entry to `themes` in `themes.ts` with `light` and `dark` colors. TypeScript fails if a token is missing.
3. Add its display name as `themes.<name>` in `src/i18n/es.json`. Ajustes → Marca offers it to owners as soon as it is in `themes`.
4. Run `pnpm test`: `When_any_theme_is_defined` checks WCAG AA contrast (4.5:1) for every text/background pair in `themeContrastPairs`, and that the primary color is not mistaken for a status color (below).

## Primary colors never look like a status

Red means an error or a destructive action (Cancelar, Eliminar), amber a warning, green paid or present. If the primary color, used on every button and selected item, shares one of those hues, the owner can't tell a normal action from a dangerous one, or "paid" from any other highlight.

`findStatusHueClashes` (`themeStatusHues.ts`) rejects a theme whose `primary` hue is less than 35° from its `danger`, `warning` or `success` hue, in light or dark. Near-gray primaries (saturation below 0.3, like Grafito) are exempt: they carry no hue to confuse. This is why there are no red, orange, yellow or green built-in themes; warm brand colors fit better as an accent (see [branding and plans](../branding-and-plans.md)).

## The brand theme

In **Ajustes → Marca** ([brand plan](20260930-brand/plan.md)) the owner picks one of the built-in themes or its own main color and optional accent. Both are stored the same way, as `themeColor` / `accentColor`: a built-in theme is saved as its light `primary` color with no accent (`builtInThemeColor`), so emails get the color too, and Agua clears the colors. `BrandProvider` (team and student layouts) reads them from the API and `toBrandTheme` turns them into a theme:

- A color equal to a built-in theme's seed with no accent (`findBuiltInThemeName`) uses that built-in theme as designed.
- Any other color builds the theme with `deriveBrandTheme` (neutral grays plus `primary*` and `accent*` derived so every pair passes AA).
- `setBrandTheme` applies it to everyone in the business under the name `business`. Signing out or removing the colors clears it and `violet` applies again.

`ThemePreviewScope` applies another set of colors (and the matching `useTheme().colors`) to a subtree; Ajustes → Marca uses it for the live preview and the welcome preview.

## Themes created by a business

A business can bring its own theme without a code change:

1. Receive the theme as JSON (for example from the API): `{ "light": { "<token>": "#rrggbb", ... }, "dark": { ... } }`.
2. Validate it with `parseThemeDefinition(json)`. It returns `{ isValid: true, theme }`, or `{ isValid: false, problems }` listing missing or malformed tokens (`light.primary: expected a #rrggbb color`) and text pairs below AA contrast (`light primary-foreground on primary`).
3. Pass valid themes to `ThemeProvider` as `customThemes={{ "studio-brand": theme }}` with `baseThemeName="studio-brand"`, or apply one at runtime with `setBrandTheme(theme)`.

A theme editor would only need to produce that JSON; screens and primitives already read everything from tokens.

## Adding a token

1. Add it to `themeColorTokens` and give it a value in every theme.
2. Add it to the token list in `tailwind.config.js`. `When_tailwind_config_is_loaded` fails until both lists match (the config is plain CommonJS and cannot import TypeScript).
3. If text is drawn on it, add the pair to `themeContrastPairs`.

## Why NativeWind variables

NativeWind v4 is already the styling layer and is stable on the three platforms. NativeWind v5 (Tailwind v4) is a release candidate and replaces `vars()` with `VariableContextProvider`; only `ThemeProvider` changes when upgrading. Tamagui, Unistyles or Restyle have good theming but would mean rewriting every style.
