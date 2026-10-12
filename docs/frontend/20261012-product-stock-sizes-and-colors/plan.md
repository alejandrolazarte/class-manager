# Product stock, sizes and colors

Date: 2026-10-12

Status: built. Part 1 (stock in the form and size chips) in PR #167, part 2 (colors) in its own pull request.

Design (Claude artifact, private to the owner): [Stock de productos](https://claude.ai/artifact/1dDSgjMRaLAZ1fkKUApuxv). Boards 1–3 are PR 1, boards 4–7 are PR 2. The texts in the boards are proposals; the ones marked as new below need the owner's OK.

## Problem

- A product is created without stock. The stock is loaded afterwards in Editar producto, in a panel (`StockPanel`) that sits below **Guardar** and **Dejar de vender este producto**, so the screen has two forms and two save buttons, and the place where the stock is chosen ("Stock" chips) and the place where it is loaded are far apart.
- Sizes are typed as one text with commas ("S, M, L"). The instructor has to guess the format: "S M L" becomes one size called "S M L", and nothing shows how many sizes the product will have until it is saved.
- A product can only vary in one dimension. A t-shirt in three sizes and two colors can't be sold with its own stock per combination.

## Decisions

- **The stock lives in one place, in both screens.** Right below the "Stock" chips, a **Unidades** block. Creating: a −/+ counter per size, saved with **Guardar**. Editing: the current units per size (0 shown as "Agotado"), **Cargar stock** and **Ajustar stock**, and the last movements. Nothing moves between creating and editing.
- **Editing never changes units in the form.** Every change is a stock movement (restock or adjustment with a reason), so the edit screen only shows the units, and the two buttons open a `BottomSheet` with the movement form. **Guardar** stays the only button of the form.
- **Sizes are chips, not a comma list.** A field with an **Agregar** button (Enter and comma also add). With no sizes yet, two buttons load a whole set: "XS a XXL" and "35 a 45". While typing, up to six sizes that start with the typed text appear as suggestions. No size standard is enforced: sizes differ between Spain and Argentina, and between clothes and shoes, and the field also takes "Niño 8" or "1 litro". Validation stays as today: no repeated names (case-insensitive), at most 20, at most 30 characters each.
- **Colors are picked, never typed.** A fixed palette of 25 colors in the app; the instructor taps the ones the product comes in. There is no "create a color": a swatch that only renames a palette color adds nothing, and a missing color is added to the app's palette for every business.
- **Size × color are the variants.** Like Shopify and Tiendanube: sizes and colors are two lists, every combination is a variant with its own stock, and a combination the business doesn't have is left at 0. Two fixed dimensions, not generic options: simpler for the instructor and enough for what these businesses sell.

## PR 1: stock in the form and size chips

Boards 1, 2 and 3 of the design. No change to how variants are stored.

### Backend

- `CreateProductCommand` takes an initial stock per variant: `VariantChange` gets an optional `InitialStock` (only read on create). `CreateProductUseCase` creates one `StockMovement` of kind `Restock` per variant with units, in the same save as the product (the command already runs in one transaction), so a product is never left half created.
- Validation: `InitialStock` between 0 and `StockMovement.MaximumQuantity`; 0 creates no movement; any value above 0 on a product with `StockMode.Unlimited` is a validation error on `Variants`.
- `UpdateProductUseCase` ignores `InitialStock`: on edit the stock only changes through `POST /products/{id}/stock`, as today.
- `ProductResponse` already returns the stock per variant, so the created product comes back with its units.

### Frontend

- New primitive `src/ui/TagInput`: a text field with an **Agregar** button, chips with a remove button, optional "set" buttons shown while empty, and type-ahead suggestions. Enter and comma add, a repeated value is rejected with an inline error. It has its own tests and is reused for colors' chips in PR 2.
- `productSchema`: `variantNames` becomes a list of names instead of a comma text; `splitVariantNames` and its comma parsing go away. `toVariantChanges` keeps today's rule that adding sizes to a product without sizes keeps its stock on the first size (test `When_sizes_are_added_to_a_product_without_sizes`).
- `ProductFormScreen`:
  - "Talles o variantes" uses `TagInput` with the "XS a XXL" and "35 a 45" sets.
  - Below the "Stock" chips, the **Unidades** block, hidden when the stock mode is "Sin stock". Creating: one `NumberStepper` row per size (one row titled "Unidades" when there are no sizes), with the total. Editing: the units per size, "Agotado" in danger tone for 0, **Cargar stock**, **Ajustar stock** and "Últimos movimientos".
- `StockMovementSheet` (new, in `features/products/components`): the restock or adjustment form of today's `StockPanel` in a `BottomSheet`, with a `SegmentedControl` for Cargar / Ajustar, the size chips with their current units, the quantity and note fields of today's panel (the note is required as "Motivo" when adjusting, and the button stays disabled until it is filled). It keeps today's validation messages and the "Stock actualizado" toast, and closes after saving. The design's stepper and "L: 0 → 10" preview were left out to keep today's fields and texts; they can come later.
- `StockPanel` is deleted (it had no tests); the new behavior tests cover loading stock from the sheet and the disabled adjustment without a reason.

### Texts

Reused as they are: "Stock", "Unidades", "{count} unidades", "Agotado", "Cargar", "Ajustar", "Cargar stock", "Ajustar stock", "Unidades que llegaron", "Diferencia (por ejemplo -1)", "Nota", "Motivo", "Últimos movimientos", "Stock actualizado".

New (to confirm): "Las que tenés hoy de cada talle.", "Agregar", "Por ejemplo: XL o 38" (replaces "S, M, L" as the placeholder), "XS a XXL", "35 a 45", "Ya agregaste {name}", and the accessibility labels "Quitar {name}", "Restar a {name}" and "Sumar a {name}". The sizes validation drops "separados por comas": "Hasta 20 talles distintos, de hasta 30 caracteres cada uno". `products.stock.title` ("Stock", used only by the old panel) is removed.

### Tests

- Unit: creating a product with initial stock records one restock per size with units; 0 records none; initial stock on a product without stock is rejected; editing ignores it.
- Integration (`Endpoints/Products`): creating a product with sizes and initial stock returns the units per size; the movements list shows the restocks.
- Frontend: adding a size with Agregar, Enter and comma; a repeated size shows the error and is not added; a set button loads the whole set; removing a chip removes its units row; the units are sent on create; "Sin stock" hides the block; on edit, **Cargar stock** opens the sheet and saving records the movement; the adjustment button stays disabled until the reason is filled.

### Screenshots

The product form changes layout, so Nuevo producto and Editar producto are captured before and after (light and dark) and shown to the owner, following the rule in `CLAUDE.md`.

## PR 2: colors

Boards 4–7 of the design. Changes how variants are stored.

### Palette

25 colors, in this order: Negro, Gris oscuro, Gris, Gris claro, Blanco, Crema, Beige, Camel, Marrón, Bordó, Rojo, Coral, Rosa, Fucsia, Lila, Violeta, Azul marino, Azul, Celeste, Turquesa, Verde agua, Verde, Verde oliva, Amarillo, Naranja.

- The API stores a stable key per color (`black`, `navy`, `mint`…), never the Spanish name, so the palette can be translated. The list of keys lives in `Core` (`ProductColors`) and is what the API validates against.
- `ProductColors` also keeps the Spanish name of each key, used only for the order line name that is saved with the order ("Remera · M · Negro") and for the stock movement label; the screens take the names from `es.json` (`products.color.*`).
- The swatch values live in `app/src/theme/productColorSwatches.ts`, next to `brandSwatches.ts`: `src/theme` is where raw colors are allowed, so no lint rule changes. Swatches are product colors, not theme colors: they look the same in every theme and in dark mode, with a thin border so white and light colors stay visible.

### Backend

- `ProductVariant` gets `Size` (today's `Name`, at most 30 characters) and `Color` (a palette key or null). The migration `AddProductVariantColors` renames `Name` to `Size`, so every existing variant becomes a size without color and keeps its id, stock and order lines.
- `VariantChange` becomes `(Id, Size, Color, InitialStock)`. `Product` validates sizes (distinct, at most 20) and colors (palette keys, all variants or none, at most 10), and the variants are exactly the combinations; at most 200 (20 × 10). Removing a size or a color deactivates its variants, as removing a size does today.
- When colors are added to a product with sizes (or sizes to one with colors), the app reuses the existing variant of each size for the first color, so its stock and history stay.
- Today there is no unique index on variants (the domain checks repeated names). Keep it that way: `Product` rejects a repeated size or color, so a combination can't appear twice.
- `SaleNameOf` and `OrderLine.NameMaxLength` include the color: "Remera · M · Negro". Order lines keep the name they were saved with, so past orders don't change.
- `ProductResponse`, `StudentAppShopVariantResponse` and the stock movement response return `size` and `color` instead of `name`.
- Stock, movements, reservations, sales and refunds keep working by variant id: no change there.

### Frontend

- "Talles o variantes" becomes two fields, **Talles** (the `TagInput` of PR 1) and **Colores**: the chosen colors as chips with their swatch, and **Elegir colores**, which opens a `BottomSheet` with the palette in a five-column grid (tap to choose several, ✓ on the chosen ones, "Listo · N colores").
- Unidades with sizes and colors is a table: sizes as columns, colors as rows, one number field per cell (0 in danger tone). With only sizes or only colors it stays the list of PR 1. Editing shows the same table read-only, with "Agotado: L Blanco" below.
- `StockMovementSheet` adds the color chips; the size chips show the units of the chosen color.
- New primitives in `src/ui`: `ColorSwatch` (swatch with an optional selected ring and check) and `StockGrid` (the table, editable or read-only), each with its tests.
- Student app, `ProductDetailPage`: **Color** (swatches, name of the chosen one next to the label) and then **Talle** (chips). A size with no stock in the chosen color is shown struck through and disabled, and choosing a sold-out combination shows "Agotado por ahora." and disables the add button, as today with sizes.
- Everywhere a variant name is shown (counter sale, order rows, refund panel, cart, movements history) shows "M · Negro".

### Texts

New (to confirm): "Talles", "Colores", "Elegir colores", "Tocá los colores en que viene este producto.", "Listo · {count} colores", "Color", "Agotado: {items}", "Tocá cada casilla y escribí cuántas tenés de ese color y talle." and the 25 color names. "Talles o variantes" (`products.form.variants`) is removed.

### Tests

- Unit: variants are the combinations of sizes and colors; an unknown color key, a repeated color or more than 10 colors are rejected; removing a color deactivates its variants and keeps their movements; existing products keep their variants as sizes.
- Integration: create and edit a product with sizes and colors; the shop returns size and color per variant; an order line saves "Producto · Talle · Color". The existing test that business A can't read business B's variants still covers `ProductVariant`.
- Frontend: choosing colors in the sheet adds their chips; the table shows one row per color and one column per size; the student picks a color and a size, and a size without stock in that color is disabled.

### Screenshots

Nuevo producto, Editar producto, the stock and color sheets and the student product page are captured before and after (light and dark) and shown to the owner.

## Open questions

- The initial stock creates `Restock` movements with no note. If the owner wants to tell them apart in the history, add a note such as "Stock inicial" (a new text) or a movement kind.
- Whether a combination the business never sells (L Blanco) should be hidden from students instead of shown as sold out. The proposal shows it as sold out; hiding it needs a switch per cell.
