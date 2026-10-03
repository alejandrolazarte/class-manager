import { APIRequestContext, expect } from "@playwright/test";
import { randomUUID } from "node:crypto";
import { e2eEnvironment } from "../support/environment";
import { SmtpSink } from "../support/smtpSink";
import { DemoBusiness, demoPassword } from "./demoBusiness";

export const demoFamilyPassword = "demo-family-password";

const familyFullName = "Ana Pérez";
const invitationTokenPattern = /accept-family-invitation\?token=([^\s]+)/;
const restockUnits = 10;
const beginnersWeekdays = [1, 3, 5];
const missedWeeksAgo = 2;
const daysPerWeek = 7;
const millisecondsPerDay = 86_400_000;

function isoDateOf(date: Date): string {
  return date.toISOString().slice(0, 10);
}

function pastClassDates(firstDate: string, today: string): string[] {
  const todayWeekday = new Date(`${today}T12:00:00Z`).getUTCDay();
  const dates: string[] = [];
  for (
    let date = new Date(`${firstDate}T12:00:00Z`);
    isoDateOf(date) < today;
    date = new Date(date.getTime() + millisecondsPerDay)
  ) {
    if ([...beginnersWeekdays, todayWeekday].includes(date.getUTCDay())) {
      dates.push(isoDateOf(date));
    }
  }
  return dates;
}

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
  absenceDate: string;
  makeupDate: string;
  packClassDate: string;
}

interface MakeupSlot {
  classGroupId: string;
  date: string;
  spotsLeft: number;
}

export function weekEndOf(isoDate: string): string {
  const date = new Date(`${isoDate}T12:00:00Z`);
  const daysToSunday = (daysPerWeek - date.getUTCDay()) % daysPerWeek;
  return isoDateOf(new Date(date.getTime() + daysToSunday * millisecondsPerDay));
}

function nextClassDateAfter(today: string, skippedDate: string): string {
  const todayWeekday = new Date(`${today}T12:00:00Z`).getUTCDay();
  for (let daysAhead = 1; ; daysAhead++) {
    const date = new Date(
      new Date(`${today}T12:00:00Z`).getTime() + daysAhead * millisecondsPerDay,
    );
    if (
      [...beginnersWeekdays, todayWeekday].includes(date.getUTCDay()) &&
      isoDateOf(date) !== skippedDate
    ) {
      return isoDateOf(date);
    }
  }
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

  const family = await send<{ students: { id: string; fullName: string }[] }>(
    "GET",
    `/api/clients/${demo.familyClientId}`,
    ownerToken,
  );
  const missedDay = new Date(
    new Date(`${demo.today}T12:00:00Z`).getTime() -
      missedWeeksAgo * daysPerWeek * millisecondsPerDay,
  );
  for (const classDate of pastClassDates(`${demo.month}-01`, demo.today)) {
    for (const student of family.students) {
      const isMissedClass =
        student.fullName === "Tomás Pérez" && classDate === isoDateOf(missedDay);
      await send(
        "PUT",
        `/api/class-groups/${demo.beginnersClassGroupId}/sessions/${classDate}/attendance/${student.id}`,
        ownerToken,
        { status: isMissedClass ? "Absent" : "Present" },
      );
    }
  }

  const lastPastClass = pastClassDates(`${demo.month}-01`, demo.today).at(-1) ?? demo.today;
  const feedbackSeeds: [studentFullName: string, classDate: string, text: string][] = [
    [
      "Lucía Pérez",
      lastPastClass,
      "Hoy mantuvo la postura en todo el circuito. Próximo objetivo: coordinar brazos y respiración.",
    ],
    [
      "Tomás Pérez",
      demo.today,
      "Muy buen ritmo en la serie larga. La semana que viene probamos la salida desde el borde.",
    ],
  ];
  for (const [studentFullName, classDate, text] of feedbackSeeds) {
    const student = family.students.find((candidate) => candidate.fullName === studentFullName)!;
    await send(
      "PUT",
      `/api/class-groups/${demo.beginnersClassGroupId}/sessions/${classDate}/feedback/${student.id}`,
      ownerToken,
      { text },
    );
  }

  const announcements: [title: string, body: string][] = [
    [
      "Muestra de fin de año",
      "Sábado 13 de diciembre en la pileta grande. Anotá a tu familia en la recepción.",
    ],
    [
      "Lunes 12 de octubre cerrado",
      "Feriado. Las clases de ese día se pueden recuperar durante la semana.",
    ],
  ];
  for (const [title, body] of announcements) {
    await send("POST", "/api/announcements", ownerToken, { title, body });
  }
  const nextWeekClass = isoDateOf(
    new Date(new Date(`${demo.today}T12:00:00Z`).getTime() + daysPerWeek * millisecondsPerDay),
  );
  await send(
    "PUT",
    `/api/class-groups/${demo.beginnersClassGroupId}/sessions/${nextWeekClass}/cancellation`,
    ownerToken,
    { reason: "Feriado" },
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

  const absenceDate = nextClassDateAfter(demo.today, nextWeekClass);
  const tomas = family.students.find((student) => student.fullName === "Tomás Pérez")!;
  await send(
    "PUT",
    `/api/family/students/${tomas.id}/absences/${demo.beginnersClassGroupId}/${absenceDate}`,
    familyToken,
    {},
  );

  const { slots } = await send<{ slots: MakeupSlot[] }>(
    "GET",
    `/api/family/students/${tomas.id}/makeups`,
    familyToken,
  );
  const openSlots = slots.filter((slot) => slot.spotsLeft > 0);
  const makeupSlot =
    openSlots.find((slot) => slot.date > demo.today && slot.date <= weekEndOf(demo.today)) ??
    openSlots[0]!;
  await send(
    "PUT",
    `/api/family/students/${tomas.id}/makeups/${makeupSlot.classGroupId}/${makeupSlot.date}`,
    familyToken,
    {},
  );

  const aquagymPack = await send<{ id: string }>("POST", "/api/class-packs", ownerToken, {
    name: "Pack Aquagym",
    classCount: 4,
    price: 26000,
    validityMonths: 1,
    classGroupIds: [demo.aquagymClassGroupId],
  });
  await send("POST", `/api/clients/${demo.familyClientId}/class-pack-purchases`, ownerToken, {
    classPackId: aquagymPack.id,
    price: 26000,
    purchasedOn: demo.today,
    method: "Transfer",
    notes: null,
  });
  const lucia = family.students.find((student) => student.fullName === "Lucía Pérez")!;
  const packClasses = await send<{ slots: MakeupSlot[] }>(
    "GET",
    `/api/family/students/${lucia.id}/pack-classes`,
    familyToken,
  );
  const packSlot =
    packClasses.slots.find(
      (slot) => slot.date > demo.today && slot.date <= weekEndOf(demo.today),
    ) ?? packClasses.slots[0]!;
  await send(
    "PUT",
    `/api/family/students/${lucia.id}/pack-classes/${packSlot.classGroupId}/${packSlot.date}`,
    familyToken,
    {},
  );

  return { email, absenceDate, makeupDate: makeupSlot.date, packClassDate: packSlot.date };
}

export async function inviteOwnerAsFamily(
  request: APIRequestContext,
  demo: DemoBusiness,
  smtpSink: SmtpSink,
): Promise<void> {
  const signIn = await request.post(`${e2eEnvironment.apiUrl}/api/auth/sign-in`, {
    data: { email: demo.email, password: demoPassword },
  });
  expect(signIn.ok(), "the demo owner signs in").toBeTruthy();
  const { accessToken } = (await signIn.json()) as { accessToken: string };
  const invitation = await request.post(
    `${e2eEnvironment.apiUrl}/api/clients/${demo.familyClientId}/app-invitation`,
    { data: { email: demo.email }, headers: { Authorization: `Bearer ${accessToken}` } },
  );
  expect(invitation.ok(), "the owner is invited as a family").toBeTruthy();
  const email = await smtpSink.waitForEmailTo(demo.email);
  const token = invitationTokenPattern.exec(email.body)?.[1];
  expect(token, "the invitation email has the accept link").toBeTruthy();
  const accepted = await request.post(
    `${e2eEnvironment.apiUrl}/api/auth/family-invitations/accept`,
    {
      data: {
        token: decodeURIComponent(token!),
        fullName: familyFullName,
        password: demoPassword,
      },
    },
  );
  expect(accepted.ok(), "the owner accepts the family invitation").toBeTruthy();
}
