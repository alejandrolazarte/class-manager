import { z } from "zod";
import { parseAmount, toAmountText } from "@/formatting/money";
import { amountSchema } from "@/features/fees/paymentSchema";
import { Product, SaveProductRequest, StockMode, VariantChange } from "@/features/products/types";
import { colorsOf, sizesOf } from "@/features/products/variantOptions";
import { translate } from "@/i18n/translate";

export const productLimits = {
  nameMinimumLength: 2,
  nameMaximumLength: 60,
  descriptionMaximumLength: 500,
  sizeMaximumLength: 30,
  maximumSizeCount: 20,
  maximumColorCount: 10,
  maximumImageCount: 5,
  maximumInitialStock: 10_000,
} as const;

const decimalRadix = 10;
const stockKeySeparator = "|";

interface VariantCombination {
  size: string;
  color: string | null;
}

export function stockKeyOf(size: string, color: string | null): string {
  return `${size}${stockKeySeparator}${color ?? ""}`;
}

export function combinationsOf(sizes: string[], colors: string[]): VariantCombination[] {
  const sizeOptions = sizes.length === 0 ? [""] : sizes;
  const colorOptions = colors.length === 0 ? [null] : colors;
  return sizeOptions.flatMap((size) => colorOptions.map((color) => ({ size, color })));
}

function hasValidSizes(sizes: string[]): boolean {
  const distinctSizes = new Set(sizes.map((size) => size.toLowerCase()));
  return (
    sizes.length <= productLimits.maximumSizeCount &&
    distinctSizes.size === sizes.length &&
    sizes.every((size) => size.trim().length > 0 && size.length <= productLimits.sizeMaximumLength)
  );
}

export const productSchema = z.object({
  name: z
    .string()
    .trim()
    .min(productLimits.nameMinimumLength, translate("products.validation.nameInvalid"))
    .max(productLimits.nameMaximumLength, translate("products.validation.nameInvalid")),
  description: z
    .string()
    .trim()
    .max(
      productLimits.descriptionMaximumLength,
      translate("products.validation.descriptionTooLong"),
    ),
  price: amountSchema,
  stockMode: z.enum(["Unlimited", "Tracked", "TrackedWithBackorder"]),
  isVisibleInApp: z.boolean(),
  sizes: z
    .array(z.string())
    .refine(hasValidSizes, translate("products.validation.variantsInvalid")),
  colors: z.array(z.string()).max(productLimits.maximumColorCount),
  initialStock: z.record(z.string(), z.string()),
});

export type ProductFormValues = z.input<typeof productSchema>;

export const productFieldNames = [
  "name",
  "description",
  "price",
  "stockMode",
] as const satisfies readonly (keyof ProductFormValues)[];

export function toProductFormValues(product?: Product): ProductFormValues {
  return {
    name: product?.name ?? "",
    description: product?.description ?? "",
    price: product ? toAmountText(product.price) : "",
    stockMode: product?.stockMode ?? ("Tracked" satisfies StockMode),
    isVisibleInApp: product?.isVisibleInApp ?? true,
    sizes: sizesOf(product?.variants ?? []),
    colors: colorsOf(product?.variants ?? []),
    initialStock: {},
  };
}

function isSameSize(first: string, second: string): boolean {
  return first.toLowerCase() === second.toLowerCase();
}

function toVariantChanges(sizes: string[], colors: string[], product?: Product): VariantChange[] {
  const existingVariants = product?.variants ?? [];
  const combinations = combinationsOf(sizes, colors);
  const keptIds = new Set<string>();
  const exactMatches = combinations.map((combination) => {
    const match = existingVariants.find(
      (variant) =>
        isSameSize(variant.size, combination.size) && variant.color === combination.color,
    );
    if (match !== undefined) {
      keptIds.add(match.id);
    }
    return match;
  });
  return combinations.map((combination, index) => {
    const looseMatch =
      exactMatches[index] ??
      existingVariants.find(
        (variant) =>
          !keptIds.has(variant.id) &&
          (variant.size.length === 0 || isSameSize(variant.size, combination.size)) &&
          (variant.color === null || variant.color === combination.color),
      );
    if (looseMatch !== undefined) {
      keptIds.add(looseMatch.id);
    }
    return { id: looseMatch?.id ?? null, size: combination.size, color: combination.color };
  });
}

function initialStockOf(initialStockText: string | undefined): number {
  const initialStock = Number.parseInt(initialStockText ?? "", decimalRadix);
  return Number.isNaN(initialStock) ? 0 : initialStock;
}

function withInitialStock(
  variants: VariantChange[],
  initialStock: Record<string, string>,
): VariantChange[] {
  return variants.map((variant) => ({
    ...variant,
    initialStock: initialStockOf(initialStock[stockKeyOf(variant.size, variant.color)]),
  }));
}

export function toSaveProductRequest(
  formValues: ProductFormValues,
  product?: Product,
): SaveProductRequest {
  const description = formValues.description.trim();
  return {
    name: formValues.name.trim(),
    description: description.length === 0 ? null : description,
    price: parseAmount(formValues.price) ?? 0,
    stockMode: formValues.stockMode,
    isVisibleInApp: formValues.isVisibleInApp,
    variants:
      product === undefined
        ? withInitialStock(
            toVariantChanges(formValues.sizes, formValues.colors),
            formValues.stockMode === "Unlimited" ? {} : formValues.initialStock,
          )
        : toVariantChanges(formValues.sizes, formValues.colors, product),
  };
}
