import { APIRequestContext, expect } from "@playwright/test";
import { randomUUID } from "node:crypto";
import { e2eEnvironment } from "../support/environment";
import { SmtpSink } from "../support/smtpSink";
import { DemoBusiness, demoPassword } from "./demoBusiness";

export const demoFamilyPassword = "demo-family-password";

const familyFullName = "Ana Pérez";
const invitationTokenPattern = /accept-family-invitation\?token=([^\s]+)/;
const restockUnits = 10;

interface Identified {
  id: string;
}

interface CreatedProduct extends Identified {
  variants: { id: string; name: string }[];
}

interface FamilyShop {
  packs: { id: string; name: string }[];
  products: { name: string; variants: { id: string; name: string }[] }[];
}

interface ProductSeed {
  name: string;
  description: string;
  price: number;
  stockMode: "Unlimited" | "Tracked" | "TrackedWithBackorder";
  sizes: string[];
  restockedSizes: string[];
}

const productSeeds: ProductSeed[] = [
  {
    name: "Remera del club",
    description: "Algodón peinado con el logo bordado en el pecho.",
    price: 18000,
    stockMode: "Tracked",
    sizes: ["8", "10", "12", "S", "M", "L"],
    restockedSizes: ["8", "10", "S", "M", "L"],
  },
  {
    name: "Gorra del club",
    description: "Visera curva y ajuste trasero.",
    price: 9000,
    stockMode: "Unlimited",
    sizes: [],
    restockedSizes: [],
  },
  {
    name: "Buzo con capucha",
    description: "Frisa liviana. Se fabrica a pedido.",
    price: 32000,
    stockMode: "TrackedWithBackorder",
    sizes: ["S", "M", "L", "XL"],
    restockedSizes: [],
  },
  {
    name: "Botella térmica",
    description: "Acero inoxidable, 500 ml. Mantiene el agua fría todo el día.",
    price: 12000,
    stockMode: "Unlimited",
    sizes: [],
    restockedSizes: [],
  },
  {
    name: "Toalla microfibra",
    description: "Secado rápido, ocupa poco en el bolso.",
    price: 14000,
    stockMode: "Tracked",
    sizes: [],
    restockedSizes: [],
  },
];

export interface DemoFamily {
  email: string;
}

export async function seedDemoFamily(
  request: APIRequestContext,
  demo: DemoBusiness,
  smtpSink: SmtpSink,
): Promise<DemoFamily> {
  async function send<TResponse>(
    method: string,
    path: string,
    accessToken: string | null,
    data?: unknown,
  ): Promise<TResponse> {
    const response = await request.fetch(`${e2eEnvironment.apiUrl}${path}`, {
      method,
      data,
      headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : {},
    });
    expect(response.ok(), `${method} ${path} answered ${response.status()}`).toBeTruthy();
    return response.status() === 204
      ? (undefined as TResponse)
      : ((await response.json()) as TResponse);
  }

  const { accessToken: ownerToken } = await send<{ accessToken: string }>(
    "POST",
    "/api/auth/sign-in",
    null,
    { email: demo.email, password: demoPassword },
  );

  for (const seed of productSeeds) {
    const product = await send<CreatedProduct>("POST", "/api/products", ownerToken, {
      name: seed.name,
      description: seed.description,
      price: seed.price,
      stockMode: seed.stockMode,
      isVisibleInApp: true,
      variants: seed.sizes.map((size) => ({ id: null, name: size })),
    });
    for (const variant of product.variants) {
      if (
        seed.stockMode === "Tracked" &&
        (seed.sizes.length === 0 || seed.restockedSizes.includes(variant.name))
      ) {
        await send("POST", `/api/products/${product.id}/stock`, ownerToken, {
          variantId: variant.id,
          kind: "Restock",
          quantity: restockUnits,
          note: null,
        });
      }
    }
  }

  const email = `familia-${randomUUID()}@demo.local`;
  await send("POST", `/api/clients/${demo.familyClientId}/app-invitation`, ownerToken, { email });
  const invitation = await smtpSink.waitForEmailTo(email);
  const invitationToken = invitationTokenPattern.exec(invitation.body)?.[1];
  expect(invitationToken, "the invitation email has the accept link").toBeTruthy();
  const { accessToken: familyToken } = await send<{ accessToken: string }>(
    "POST",
    "/api/auth/family-invitations/accept",
    null,
    {
      token: decodeURIComponent(invitationToken!),
      fullName: familyFullName,
      password: demoFamilyPassword,
    },
  );

  const shop = await send<FamilyShop>("GET", "/api/family/shop", familyToken);
  const variantId = (productName: string, size = "") =>
    shop.products
      .find((product) => product.name === productName)!
      .variants.find((variant) => variant.name === size)!.id;
  const packId = shop.packs.find((pack) => pack.name === "8 clases")!.id;
  const placeOrder = (
    lines: { classPackId: string | null; productVariantId: string | null; quantity: number }[],
    delivery: string | null,
    deliveryClassGroupId: string | null,
  ) =>
    send<Identified>("POST", "/api/family/orders", familyToken, {
      lines,
      delivery,
      deliveryClassGroupId,
    });

  const readyOrder = await placeOrder(
    [
      { classPackId: null, productVariantId: variantId("Remera del club", "8"), quantity: 1 },
      { classPackId: null, productVariantId: variantId("Gorra del club"), quantity: 1 },
    ],
    "InClass",
    demo.beginnersClassGroupId,
  );
  await send("PUT", `/api/orders/${readyOrder.id}/payment`, ownerToken, {
    method: "Cash",
    paidOn: demo.today,
    isReady: true,
  });
  const cancelledOrder = await placeOrder(
    [{ classPackId: null, productVariantId: variantId("Botella térmica"), quantity: 1 }],
    "Pickup",
    null,
  );
  await send("PUT", `/api/family/orders/${cancelledOrder.id}/cancellation`, familyToken, {});
  await placeOrder([{ classPackId: packId, productVariantId: null, quantity: 1 }], null, null);

  return { email };
}
