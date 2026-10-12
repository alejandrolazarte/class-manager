import { translate, TranslationKey } from "@/i18n/translate";
import { ProductColorKey, productColorSwatches } from "@/theme/productColorSwatches";

export const productColorKeys = Object.keys(productColorSwatches) as ProductColorKey[];

const labelSeparator = " · ";

function isProductColorKey(color: string): color is ProductColorKey {
  return color in productColorSwatches;
}

export function productColorName(color: string): string {
  return isProductColorKey(color) ? translate(`products.color.${color}` as TranslationKey) : color;
}

export function productColorSwatch(color: string): string | null {
  return isProductColorKey(color) ? productColorSwatches[color] : null;
}

export function variantDisplayName(size: string, color: string | null): string {
  return [size, color === null ? "" : productColorName(color)]
    .filter((part) => part.length > 0)
    .join(labelSeparator);
}
