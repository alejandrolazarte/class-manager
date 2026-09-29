import { FamilyShop, PlaceFamilyOrderLine } from "@/features/family/types";

export type FamilyCart = Readonly<Record<string, number>>;

export const maximumUnitsPerItem = 99;

export function cartLines(shop: FamilyShop, cart: FamilyCart): PlaceFamilyOrderLine[] {
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

export function cartTotal(shop: FamilyShop, cart: FamilyCart): number {
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
