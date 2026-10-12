# Branding and plans

Date: 2026-09-28

Product decisions on what a business can brand, and which parts could be paid. Pricing itself is still an [open decision](mvp-plan.md#open-decisions); this only records where branding fits. The technical plans are [themes, business theme and business icon](frontend/20260928-themes-and-business-icon/plan.md) and [brand: logo, brand theme and welcome screen](frontend/20260930-brand/plan.md).

## What each user sees

Decided 2026-10-11, replacing "each user picks a theme, unless the business locks it":

- **The colors belong to the business, not to each user.** Students, families and the team see the theme the business chose; nobody else picks colors.
- A business **without** a theme (or whose plan doesn't include the brand) shows the default theme (`violet`, the Laztty color) to everyone.
- **Each person still picks light, dark or automatic** in Ajustes → Apariencia. That is comfort and accessibility, not branding, so it is never imposed. The sheet says who chooses the colors.
- Before sign-in and on the account chooser there is no business yet, so those screens use `violet`. After entering a business, its colors apply; a person in two businesses sees each one in its own colors, which also tells them apart.
- **Each branch has its own theme**, because the brand is stored per business (tenant) and every branch is a business. The brand owner, who enters every branch, sets it in each one.

Why: in branded apps for gyms and studios (Mindbody's branded app, for example) the business sets the colors and logo and members don't change them; the brand is what the business pays for. In Messenger the theme belongs to the conversation and everyone in it sees the same one. Per-user themes are typical of work tools used all day (Slack), and where a company imposes its theme (Microsoft 365, "Prevent users from overriding their theme") only an accessibility option stays personal. Letting students pick and later taking it away when the business pays felt like losing something; if they never pick, there is nothing to lose.

## Free and paid

| Feature | Plan | Why |
|---|---|---|
| Light, dark or automatic | Free, for every person | Comfort and accessibility |
| Business theme: one of the built-in themes or its own colors, for students and team | Paid ("Marca propia" or part of a Pro plan) | Studios and chains with an established brand value it, and they are the ones that pay the most |
| Business icon from the catalog | Free | Same |
| Custom logo instead of the catalog icon | Paid, later | Students now see it (welcome screen and Inicio in the student app) |

**When a business stops paying:** the colors stay stored but are no longer applied; everyone falls back to `violet`. Nothing breaks, and paying again brings the theme back.

**Status (2026-10-11):** logo, business theme (built-in or own colors, with accent) and welcome screen are built and gated by the `brand` feature of the plan ([brand plan](frontend/20260930-brand/plan.md)). The theme lock ("Usar siempre mi marca") was removed: with no personal theme choice there is nothing to lock.

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
