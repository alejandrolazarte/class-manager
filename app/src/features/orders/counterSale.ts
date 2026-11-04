import { ClassPack, SoldOnceClassPack } from "@/features/classPacks/types";
import { CounterSaleLine } from "@/features/orders/types";
import { variantDisplayName } from "@/features/products/productColors";
import { Product, ProductVariant } from "@/features/products/types";
import { groupDeliveryClasses } from "@/features/studentApp/deliveryClassGroups";
import { DeliveryClass } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";

export const maximumUnitsPerLine = 99;

export type CounterSaleItemKind = "plan" | "pack" | "product";

export interface CounterSaleItem {
  key: string;
  kind: CounterSaleItemKind;
  name: string;
  price: number;
  detail: string;
  imageUrl: string | null;
  maximumUnits: number;
  size: string | null;
  color: string | null;
  isSoldOut: boolean;
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
  const displayName = variantDisplayName(variant.size, variant.color);
  return product.variants.length > 1 && displayName.length > 0
    ? `${product.name} · ${displayName}`
    : product.name;
}

function planDetail(plan: ClassPack): string {
  return plan.classCount === null
    ? translate("student.shop.libreClasses")
    : translateCount("student.shop.classesPerMonth", plan.classCount);
}

export function counterSaleItems(
  classPacks: readonly SoldOnceClassPack[],
  products: readonly Product[],
  plans: readonly ClassPack[] = [],
): CounterSaleItem[] {
  return [
    ...plans.map((plan) => ({
      key: plan.id,
      kind: "plan" as const,
      name: plan.name,
      price: plan.price,
      detail: planDetail(plan),
      imageUrl: plan.images[0]?.url ?? null,
      maximumUnits: 1,
      size: null,
      color: null,
      isSoldOut: false,
    })),
    ...classPacks.map((classPack) => ({
      key: classPack.id,
      kind: "pack" as const,
      name: classPack.name,
      price: classPack.price,
      detail: translateCount("student.shop.classCount", classPack.classCount),
      imageUrl: classPack.images[0]?.url ?? null,
      maximumUnits: 1,
      size: null,
      color: null,
      isSoldOut: false,
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
        size: variant.size.length > 0 ? variant.size : null,
        color: variant.color,
        isSoldOut: maximumUnitsOf(product, variant) === 0,
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

function withoutKinds(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
  kinds: readonly CounterSaleItemKind[],
): UnitsByItem {
  const removedKeys = new Set(
    items.filter((item) => kinds.includes(item.kind)).map((item) => item.key),
  );
  return Object.fromEntries(Object.entries(unitsByItem).filter(([key]) => !removedKeys.has(key)));
}

export function withoutStudentItems(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): UnitsByItem {
  return withoutKinds(items, unitsByItem, ["plan", "pack"]);
}

export function withoutPlans(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): UnitsByItem {
  return withoutKinds(items, unitsByItem, ["plan"]);
}

export function withChangedUnits(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
  changedItem: CounterSaleItem,
  units: number,
): UnitsByItem {
  const nextUnits = Math.max(0, units);
  if (changedItem.kind === "plan" && nextUnits > 0) {
    return { [changedItem.key]: 1 };
  }
  const otherItems = nextUnits > 0 ? withoutPlans(items, unitsByItem) : unitsByItem;
  return { ...otherItems, [changedItem.key]: nextUnits };
}

export function chosenPlan(
  items: readonly CounterSaleItem[],
  unitsByItem: UnitsByItem,
): CounterSaleItem | undefined {
  return chosenItems(items, unitsByItem).find((item) => item.kind === "plan");
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
