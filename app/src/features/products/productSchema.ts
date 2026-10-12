import { z } from "zod";
import { parseAmount, toAmountText } from "@/features/fees/money";
import { amountSchema } from "@/features/fees/paymentSchema";
import { Product, SaveProductRequest, StockMode, VariantChange } from "@/features/products/types";
import { translate } from "@/i18n/translate";

export const productLimits = {
  nameMinimumLength: 2,
  nameMaximumLength: 60,
  descriptionMaximumLength: 500,
  variantNameMaximumLength: 30,
  maximumVariantCount: 20,
  maximumImageCount: 5,
  maximumInitialStock: 10_000,
} as const;

export const unnamedVariantStockKey = "";

const decimalRadix = 10;

function hasValidVariantNames(variantNames: string[]): boolean {
  const distinctNames = new Set(variantNames.map((variantName) => variantName.toLowerCase()));
  return (
    variantNames.length <= productLimits.maximumVariantCount &&
    distinctNames.size === variantNames.length &&
    variantNames.every(
      (variantName) =>
        variantName.trim().length > 0 &&
        variantName.length <= productLimits.variantNameMaximumLength,
    )
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
  variantNames: z
    .array(z.string())
    .refine(hasValidVariantNames, translate("products.validation.variantsInvalid")),
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
    variantNames: (product?.variants ?? [])
      .map((variant) => variant.name)
      .filter((variantName) => variantName.length > 0),
    initialStock: {},
  };
}

function toVariantChanges(variantNames: string[], product?: Product): VariantChange[] {
  const existingVariants = product?.variants ?? [];
  if (variantNames.length === 0) {
    const unnamedVariant = existingVariants.length === 1 ? existingVariants[0] : undefined;
    return [{ id: unnamedVariant?.id ?? null, name: "" }];
  }
  if (existingVariants.length === 1 && existingVariants[0].name.length === 0) {
    return variantNames.map((variantName, index) => ({
      id: index === 0 ? existingVariants[0].id : null,
      name: variantName,
    }));
  }
  return variantNames.map((variantName) => ({
    id:
      existingVariants.find((variant) => variant.name.toLowerCase() === variantName.toLowerCase())
        ?.id ?? null,
    name: variantName,
  }));
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
    initialStock: initialStockOf(initialStock[variant.name]),
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
            toVariantChanges(formValues.variantNames),
            formValues.stockMode === "Unlimited" ? {} : formValues.initialStock,
          )
        : toVariantChanges(formValues.variantNames, product),
  };
}
