import {
  StudentAppShop,
  StudentAppShopProduct,
  StudentAppShopVariant,
  PlaceStudentAppOrderLine,
} from "@/features/studentApp/types";

export type StudentAppCart = Readonly<Record<string, number>>;

export const maximumUnitsPerItem = 99;

export function cartLines(shop: StudentAppShop, cart: StudentAppCart): PlaceStudentAppOrderLine[] {
  const packLines = shop.packs
    .filter((pack) => (cart[pack.id] ?? 0) > 0)
    .map((pack) => ({ classPackId: pack.id, productVariantId: null, quantity: 1 }));
  const productLines = shop.products.flatMap((product) =>
    product.variants
      .filter((variant) => (cart[variant.id] ?? 0) > 0)
      .map((variant) => ({
        classPackId: null,
        productVariantId: variant.id,
        quantity: cart[variant.id],
      })),
  );
  return [...packLines, ...productLines];
}

export function cartTotal(shop: StudentAppShop, cart: StudentAppCart): number {
  const packsTotal = shop.packs
    .filter((pack) => (cart[pack.id] ?? 0) > 0)
    .reduce((total, pack) => total + pack.price, 0);
  const productsTotal = shop.products.reduce(
    (total, product) =>
      total +
      product.variants.reduce(
        (variantTotal, variant) => variantTotal + product.price * (cart[variant.id] ?? 0),
        0,
      ),
    0,
  );
  return packsTotal + productsTotal;
}

export function cartUnits(cart: StudentAppCart): number {
  return Object.values(cart).reduce((units, itemUnits) => units + itemUnits, 0);
}

export function withUnits(cart: StudentAppCart, itemId: string, units: number): StudentAppCart {
  const nextCart: Record<string, number> = { ...cart };
  const cappedUnits = Math.min(units, maximumUnitsPerItem);
  if (cappedUnits > 0) {
    nextCart[itemId] = cappedUnits;
  } else {
    delete nextCart[itemId];
  }
  return nextCart;
}

export interface StudentAppCartItem {
  itemId: string;
  name: string;
  units: number;
  total: number;
  isPack: boolean;
}

export function variantName(
  product: StudentAppShopProduct,
  variant: StudentAppShopVariant,
): string {
  return variant.name.length === 0 ? product.name : `${product.name} · ${variant.name}`;
}

export function cartItems(shop: StudentAppShop, cart: StudentAppCart): StudentAppCartItem[] {
  const packItems = shop.packs
    .filter((pack) => (cart[pack.id] ?? 0) > 0)
    .map((pack) => ({
      itemId: pack.id,
      name: pack.name,
      units: 1,
      total: pack.price,
      isPack: true,
    }));
  const productItems = shop.products.flatMap((product) =>
    product.variants
      .filter((variant) => (cart[variant.id] ?? 0) > 0)
      .map((variant) => ({
        itemId: variant.id,
        name: variantName(product, variant),
        units: cart[variant.id] ?? 0,
        total: product.price * (cart[variant.id] ?? 0),
        isPack: false,
      })),
  );
  return [...packItems, ...productItems];
}
