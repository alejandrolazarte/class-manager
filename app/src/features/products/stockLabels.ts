import { Product, ProductVariant } from "@/features/products/types";
import { translate, translateCount } from "@/i18n/translate";

export function variantLabel(variant: ProductVariant): string {
  return variant.name.length === 0 ? translate("products.stock.single") : variant.name;
}

export function stockSummary(product: Product): string {
  if (product.stockMode === "Unlimited") {
    return translate("products.stockMode.Unlimited");
  }
  const totalStock = product.variants.reduce((total, variant) => total + (variant.stock ?? 0), 0);
  if (totalStock <= 0) {
    return translate(
      product.stockMode === "TrackedWithBackorder"
        ? "products.stock.onOrder"
        : "products.stock.soldOut",
    );
  }
  return translateCount("products.stock.units", totalStock);
}
