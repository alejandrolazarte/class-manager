# Laztty brand

Laztty is the product name users see. The repository, solution, namespaces, package id (`com.alejandrolazarte.classmanager`), URL scheme and storage keys keep the `class-manager` name: renaming them would sign every user out, break installed Android apps and change nothing users see.

The name is not registered as a trademark yet. Before registering it, search the name and the mark together in the trademark office of each country where Laztty is sold (INPI in Argentina; classes 9 and 42 usually cover an app and its online service).

## Laztty brand and business brand

Every business picks its own color, logo and name under **Marca**, and the app and its emails use them once someone is signed in to that business. Laztty is the platform: it appears where no business is known yet or where the platform itself speaks.

| Where | Shows |
|---|---|
| App icon, splash screen, favicon, installed web app | Laztty |
| Loading screen before welcome, welcome, sign-in, sign-up, password screens, invitations | Laztty mark (`BrandMark`) |
| Settings, when the business has no logo | Laztty mark |
| Settings footer (`settings.version`) | "Laztty · versión …" |
| Emails with no business (password reset, account emails) | Laztty name and the default layout |
| Emails from a business | The business name, logo and color |
| Default web push title | Laztty |

The app's UI theme (Calma, lagoon by default) stays independent from the brand color: the brand color marks the platform, the theme colors the screens.

## The mark

A capital L made of two rounded pieces, slanted forward 12°: a white vertical stem and a horizontal foot at 55 % opacity, on a violet rounded square.

- Source: [`app/assets/brand/laztty-mark.svg`](../app/assets/brand/laztty-mark.svg) (white mark, transparent), [`laztty-icon.svg`](../app/assets/brand/laztty-icon.svg) (mark on the violet tile), [`laztty-lockup.svg`](../app/assets/brand/laztty-lockup.svg) (tile and the name).
- Geometry, on a 100 × 100 grid before the slant: stem `x 24, y 14, 26 × 72`, foot `x 24, y 60, 60 × 26`, corner radius 13. The slant is a 12° skew around the center. The mark takes 58 % of the tile.
- In the app the mark is drawn with views by `BrandMark` (`app/src/ui/BrandMark.tsx`), so it needs no image or SVG library. The loading screen wraps it in `Breathing` (`app/src/ui/motion/Breathing.tsx`): it scales to 94 % and fades to 60 % every 800 ms, and stays still with reduced motion.

Rules:

- Keep the slant, the two pieces and the lighter foot. Don't outline it, rotate it, add shadows inside it or put it on a photo.
- Clear space around the tile: at least a quarter of its width.
- Smallest size: 16 px for the favicon; 24 px anywhere else.
- On a dark background the violet tile stays as it is. A single-color version (Android themed icons) uses the mark alone, with the foot at 55 % opacity.

## Color

| Token | Value | Use |
|---|---|---|
| Laztty violet | `#6D28D9` (`palette.violet700`, `appBrandColor`) | The tile, app icon backgrounds, adaptive icon background |
| Mark | `#FFFFFF` (`appBrandMarkColor`) | The L |
| Ink | `#10212F` | The name on light backgrounds |

Violet is a common color with no owner in this market. What sets Laztty apart is the name, the slanted L and the color together.

## Doodle pattern

A repeating pattern of line drawings, in the spirit of the WhatsApp chat wallpaper: people of every body type, age and look doing yoga, pilates, swimming and functional training, with sport objects (kettlebell, dumbbell, rolled mat, bottle, stopwatch, goggles and swim cap) and small dots, stars, hearts and waves filling the gaps. It is part of the Laztty brand, so it only appears where Laztty speaks.

| Where | How |
|---|---|
| Welcome, behind the mark in the hero | `DoodlePattern` (`app/src/ui/DoodlePattern.tsx`), tinted with the theme `primary` at 30 % |

- Files: `app/assets/images/doodle-pattern.png` (420 × 420, a 420 pt tile) with `@2x` and `@3x`. Black lines on a transparent background, so the app tints them with `tintColor`; the tile repeats with no visible seam.
- The figures were generated with Figma AI (one line drawing per person or object, same prompt style), their backgrounds removed, line weights evened out and laid out on a seamless tile. The 1024 × 1024 originals are kept with the design canvas, outside the repository.
- Keep it decorative: it is hidden from screen readers and never sits behind text that has to be read. Lists, forms and day-to-day work screens don't use it.

## Name and type

- Written **Laztty**, capital L only, in running text and in UI copy. The lockup writes it lowercase: `laztty`.
- Typeface: Figtree Bold for the lockup, the same family the app uses.

## App icons

Rendered from `laztty-mark.svg` by [`tools/BrandAssets/render-brand-assets.mjs`](../tools/BrandAssets/render-brand-assets.mjs) with Playwright's Chromium:

| File | Size | Content |
|---|---|---|
| `app/assets/images/icon.png` | 1024 | Full-bleed violet, mark at 58 % (iOS and the stores round the corners) |
| `app/assets/images/android-icon-background.png` | 512 | Solid violet |
| `app/assets/images/android-icon-foreground.png` | 512 | Mark at 40 %, inside the adaptive icon safe zone |
| `app/assets/images/android-icon-monochrome.png` | 432 | Mark at 40 %, for themed icons |
| `app/assets/images/splash-icon.png` | 512 | Violet tile with the mark, on the white splash (`imageWidth` 96) |
| `app/assets/images/favicon.png` | 48 | Violet tile with the mark |
| `app/public/icons/icon-192.png`, `icon-512.png` | 192, 512 | Violet tile with the mark (installed web app) |
| `app/public/icons/icon-maskable-512.png` | 512 | Full-bleed violet, mark at 46 % (maskable) |
| `app/public/icons/apple-touch-icon.png` | 180 | Full-bleed violet, mark at 58 % |
| `app/backoffice-web/favicon.png` | 48 | Slate tile with the mark (backoffice) |
| `app/backoffice-web/icons/icon-192.png`, `icon-512.png` | 192, 512 | Slate tile with the mark (installed backoffice) |
| `app/backoffice-web/icons/icon-maskable-512.png` | 512 | Full-bleed slate, mark at 46 % (maskable) |
| `app/backoffice-web/icons/apple-touch-icon.png` | 180 | Full-bleed slate, mark at 58 % |

The backoffice (**Laztty Admin**) uses the same mark on slate `#334155` instead of violet, so the two installed web apps look related but are told apart at a glance, on dark and light wallpapers. Its manifest is `app/backoffice-web/manifest.webmanifest`; `pnpm export:backoffice` puts it and these icons in `dist-backoffice` in place of the app's, and sets the page's `theme-color`, description and iOS title from it (`scripts/applyBackofficeWebIdentity.js`).

To change the mark, edit `laztty-mark.svg` and the geometry in `BrandMark.tsx` together, then render the icons again:

```powershell
cd e2e; pnpm install; cd ..
node tools/BrandAssets/render-brand-assets.mjs
```

If Playwright's own Chromium is not installed, point the script at another one with `CHROMIUM_PATH`.
