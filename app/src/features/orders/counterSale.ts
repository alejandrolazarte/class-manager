import { ClassPack } from "@/features/classPacks/types";
import { CounterSaleLine } from "@/features/orders/types";
import { Product, ProductVariant } from "@/features/products/types";
import { groupDeliveryClasses } from "@/features/studentApp/deliveryClassGroups";
import { DeliveryClass } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";

export const maximumUnitsPerLine = 99;

export type CounterSaleItemKind = "pack" | "product";

export interface CounterSaleItem {
  key: string;
  kind: CounterSaleItemKind;
  name: string;
  price: number;
  detail: string;
  imageUrl: string | null;
  maximumUnits: number;
}

export type UnitsByItem = Readonly<Record<string, number>>;

export function maximumUnitsOf(product: Product, variant: ProductVariant): number {
  if (product.stockMode !== "Tracked") {
    return maximumUnitsPerLine;
  }
  return Math.max(0, Math.min(maximumUnitsPerLine, variant.stock ?? 0));
}

function variantStockDetail(product: Product, variant: ProductVariant): string | null {
  if (product.stockMode === "Unlimited") {
    return null;
  }
  const stock = variant.stock ?? 0;
  if (stock > 0) {
    return translateCount("orders.counterSale.stockLeft", stock);
  }
  return translate(
    product.stockMode === "TrackedWithBackorder"
      ? "products.stock.onOrder"
      : "products.stock.soldOut",
  );
}

function productItemName(product: Product, variant: ProductVariant): string {
  return product.variants.length > 1 && variant.name.length > 0
    ? `${product.name} · ${variant.name}`
    : product.name;
}

export function counterSaleItems(
  classPacks: readonly ClassPack[],
  products: readonly Product[],
): CounterSaleItem[] {
  return [
    ...classPacks.map((classPack) => ({
      key: classPack.id,
      kind: "pack" as const,
      name: classPack.name,
      price: classPack.price,
      detail: translateCount("student.shop.classCount", classPack.classCount),
      imageUrl: classPack.images[0]?.url ?? null,
      maximumUnits: 1,
    })),
    ...products.flatMap((product) =>
      product.variants.map((variant) => ({
        key: variant.id,
        kind: "product" as const,
        name: productItemName(product, variant),
        price: product.price,
        detail: variantStockDetail(product, variant) ?? "",
        imageUrl: product.images[0]?.url ?? null,
        maximumUnits: maximumUnitsOf(product, variant),
      })),
    ),
  ];
}

export function unitsOf(unitsByItem: UnitsByItem, item: CounterSaleItem): number {
  return unitsByItem[item.key] ?? 0;
}

export function chosenItems(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): CounterSaleItem[] {
  return items.filter((item) => unitsOf(unitsByItem, item) > 0);
}

export function counterSaleUnitCount(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): number {
  return items.reduce((count, item) => count + unitsOf(unitsByItem, item), 0);
}

export function counterSaleTotal(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): number {
  return items.reduce((total, item) => total + item.price * unitsOf(unitsByItem, item), 0);
}

export function counterSaleLines(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): CounterSaleLine[] {
  return chosenItems(items, unitsByItem).map((item) =>
    item.kind === "pack"
      ? { classPackId: item.key, productVariantId: null, quantity: null, unitPrice: null }
      : {
          classPackId: null,
          productVariantId: item.key,
          quantity: unitsOf(unitsByItem, item),
          unitPrice: null,
        },
  );
}

export function withoutPacks(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): UnitsByItem {
  const packKeys = new Set(items.filter((item) => item.kind === "pack").map((item) => item.key));
  return Object.fromEntries(Object.entries(unitsByItem).filter(([key]) => !packKeys.has(key)));
}

export interface DeliveryClassOption {
  classGroupId: string;
  hint: string;
}

export function deliveryClassOptions(
  deliveryClasses: readonly DeliveryClass[],
): DeliveryClassOption[] {
  return groupDeliveryClasses(deliveryClasses).map((group) => ({
    classGroupId: group.classGroupId,
    hint: `${group.classGroupName} · ${group.studentNames.join(", ")}`,
  }));
}
