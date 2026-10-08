# Branding and plans

Date: 2026-09-28

Product decisions on what a business can brand, and which parts could be paid. Pricing itself is still an [open decision](mvp-plan.md#open-decisions); this only records where branding fits. The technical plans are [themes, business theme and business icon](frontend/20260928-themes-and-business-icon/plan.md) and [brand: logo, brand theme and welcome screen](frontend/20260930-brand/plan.md).

## What each user sees

- A user in a business **with** its own theme starts with the business theme.
- A user in a business **without** one starts with the default theme (`aqua`).
- Any user can pick another theme at any time, unless the business locks its theme (paid, below).

## Free and paid

| Feature | Plan | Why |
|---|---|---|
| Built-in themes, light/dark | Free | Basic personalization; costs nothing to keep and makes the app feel like one's own from day one |
| Business icon from the catalog | Free | Same |
| Business theme from its brand colors, default for the team | Paid ("Marca propia" or part of a Pro plan) | Studios and chains with an established brand value it, and they are the ones that pay the most |
| Lock the business theme for members | Paid | Studios with a brand want instructors (and, later, students) to see it; an independent instructor doesn't need it |
| Custom logo instead of the catalog icon | Paid, later | Students now see it (welcome screen and Inicio in the student app) |

**When a business stops paying:** the colors stay stored but are no longer applied; everyone falls back to `aqua` or the theme they chose. Nothing breaks, and paying again brings the theme back.

**Status (2026-09-30):** logo, brand theme with accent, lock and welcome screen are built and, during the pilot, available to every business without a paid gate ([brand plan](frontend/20260930-brand/plan.md)).

**Timing:** while only the owner and instructors use the app, a business theme is nice to have, not a need. It becomes a strong selling point once students see something branded (receipts, a student portal, emails). Until then it can be free during the pilot as a hook, announced as part of the paid plan later.

## Brands with two or more colors

The business theme takes **up to two colors**:

| Color | Used for | Tokens |
|---|---|---|
| **Main** (required) | Buttons, selected tab, links, headers, the business icon background | `primary*` (exists today) |
| **Accent** (optional) | Small highlights only, never buttons: the eyebrow above titles, the "Hoy" chip, badges | New `accent*` tokens |

- **Without an accent, `accent*` equals `primary*`.** Built-in themes do the same unless a design gives them one, so screens look as they do today.
- **The owner chooses which color is the main one.** Many brands have a color that doesn't work as the main UI color (yellow, light green, beige: white text on it is unreadable). The preview shows both options, and the derivation darkens or lightens each color until its text passes WCAG AA, so an unusable choice still becomes legible.
- **A third color or more is not used in the interface.** More than two colors in a working app makes states (paid, overdue, cancelled) harder to read, since those already use green, amber and red. Extra brand colors belong in the logo, which shows the full brand as it is.
- Status colors (success, warning, danger) never take brand colors: "paid" must look the same in every business.
- **A warm brand (red, orange, yellow, green) goes better as the accent** than as the main color: the main color is on every button, and in those hues buttons look like errors, warnings or "paid". Built-in themes follow the same rule.

The accent is a token family (`accent`, `accent-soft`, `accent-soft-foreground`, see [theming](frontend/theming.md)) stored as `Business.AccentColor`.
