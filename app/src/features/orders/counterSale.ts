import { ClassPack } from "@/features/classPacks/types";
import { CounterSaleLine } from "@/features/orders/types";
import { Product, ProductVariant } from "@/features/products/types";

export const maximumUnitsPerLine = 99;

const decimalRadix = 10;

export function quantityOf(quantityText: string | undefined): number {
  const quantity = Number.parseInt(quantityText ?? "", decimalRadix);
  return Number.isNaN(quantity) || quantity < 0 ? 0 : quantity;
}

export function maximumUnitsOf(product: Product, variant: ProductVariant): number {
  return product.stockMode === "Tracked"
    ? Math.max(0, Math.min(maximumUnitsPerLine, variant.stock ?? 0))
    : maximumUnitsPerLine;
}

export function counterSaleLines(
  classPackIds: readonly string[],
  quantitiesByVariant: Readonly<Record<string, string>>,
): CounterSaleLine[] {
  return [
    ...classPackIds.map((classPackId) => ({
      classPackId,
      productVariantId: null,
      quantity: null,
      unitPrice: null,
    })),
    ...Object.entries(quantitiesByVariant)
      .filter(([, quantityText]) => quantityOf(quantityText) > 0)
      .map(([productVariantId, quantityText]) => ({
        classPackId: null,
        productVariantId,
        quantity: quantityOf(quantityText),
        unitPrice: null,
      })),
  ];
}

export function counterSaleTotal(
  classPacks: readonly ClassPack[],
  products: readonly Product[],
  classPackIds: readonly string[],
  quantitiesByVariant: Readonly<Record<string, string>>,
): number {
  const packsTotal = classPacks
    .filter((classPack) => classPackIds.includes(classPack.id))
    .reduce((total, classPack) => total + classPack.price, 0);
  const productsTotal = products.reduce(
    (total, product) =>
      total +
      product.variants.reduce(
        (variantTotal, variant) =>
          variantTotal + product.price * quantityOf(quantitiesByVariant[variant.id]),
        0,
      ),
    0,
  );
  return packsTotal + productsTotal;
}
