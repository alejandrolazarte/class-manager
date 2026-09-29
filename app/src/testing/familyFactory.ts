import {
  FamilyHome,
  FamilyNextClass,
  FamilyOrder,
  FamilyShop,
  FamilyStudent,
} from "@/features/family/types";

export function buildAccessTokenWithClaims(claims: object): string {
  const payload = btoa(JSON.stringify(claims))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=+$/, "");
  return `header.${payload}.signature`;
}

export function buildFamilyAccessToken(): string {
  return buildAccessTokenWithClaims({ sub: "user-family", kind: "family" });
}

export function buildFamilyNextClass(overrides: Partial<FamilyNextClass> = {}): FamilyNextClass {
  return {
    name: "Natación inicial",
    date: "2026-09-29",
    startTime: "18:00",
    endTime: "18:45",
    instructorFullName: "Laura Gómez",
    location: null,
    isPrivateLesson: false,
    isCancelled: false,
    ...overrides,
  };
}

export function buildFamilyStudent(overrides: Partial<FamilyStudent> = {}): FamilyStudent {
  return {
    id: "0192f0c5-0000-7000-8000-000000000001",
    fullName: "Tomás Pérez",
    nextClasses: [buildFamilyNextClass()],
    ...overrides,
  };
}

export function buildFamilyHome(overrides: Partial<FamilyHome> = {}): FamilyHome {
  return {
    businessName: "DF Swimming Tenerife",
    currencyCode: "EUR",
    clientFullName: "Ana Pérez",
    students: [buildFamilyStudent()],
    billing: {
      kind: "BusinessFee",
      monthlyFee: { month: "2026-09", fee: 60, paid: 0, balance: 60, status: "Unpaid" },
      classes: null,
    },
    ...overrides,
  };
}

export function buildFamilyShop(overrides: Partial<FamilyShop> = {}): FamilyShop {
  return {
    currencyCode: "EUR",
    packs: [{ id: "pack-8", name: "8 clases", classCount: 8, price: 120, validityMonths: 2 }],
    products: [
      {
        id: "product-cap",
        name: "Gorro de natación",
        description: null,
        price: 12,
        variants: [
          { id: "variant-s", name: "S", availability: "Available" },
          { id: "variant-m", name: "M", availability: "SoldOut" },
        ],
      },
    ],
    ...overrides,
  };
}

export function buildFamilyOrder(overrides: Partial<FamilyOrder> = {}): FamilyOrder {
  return {
    id: "order-1",
    status: "Requested",
    awaitsPickup: false,
    total: 120,
    refundedAmount: 0,
    createdAt: "2026-09-29T10:00:00Z",
    paidOn: null,
    expiresAt: "2026-10-06T10:00:00Z",
    lines: [
      {
        id: "line-1",
        kind: "ClassPack",
        name: "8 clases",
        quantity: 1,
        total: 120,
        refundedQuantity: 0,
      },
    ],
    ...overrides,
  };
}
