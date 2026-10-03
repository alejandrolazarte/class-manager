import {
  FamilyAttendance,
  FamilyHome,
  FamilyMakeups,
  FamilyMakeupSlot,
  FamilyNews,
  FamilyNewsItem,
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
    classGroupId: "class-group-1",
    absenceNotified: false,
    isMakeup: false,
    isPackBooking: false,
    ...overrides,
  };
}

export function buildFamilyMakeupSlot(overrides: Partial<FamilyMakeupSlot> = {}): FamilyMakeupSlot {
  return {
    classGroupId: "class-group-2",
    name: "Natación avanzada",
    date: "2026-09-29",
    startTime: "19:00",
    endTime: "19:45",
    instructorFullName: "Marcos Díaz",
    location: null,
    spotsLeft: 3,
    isBooked: false,
    ...overrides,
  };
}

export function buildFamilyMakeups(overrides: Partial<FamilyMakeups> = {}): FamilyMakeups {
  return {
    credits: [{ missedOn: "2026-09-22", reason: "Notice", expiresOn: "2026-10-22" }],
    slots: [buildFamilyMakeupSlot()],
    ...overrides,
  };
}

export function buildFamilyAttendance(overrides: Partial<FamilyAttendance> = {}): FamilyAttendance {
  return {
    streakWeeks: 0,
    streakSince: null,
    attendedClasses: 0,
    recentWeeks: [],
    bestStreakWeeks: 0,
    level: 1,
    medals: [],
    ...overrides,
  };
}

export function buildFamilyStudent(overrides: Partial<FamilyStudent> = {}): FamilyStudent {
  return {
    id: "0192f0c5-0000-7000-8000-000000000001",
    fullName: "Tomás Pérez",
    nextClasses: [buildFamilyNextClass()],
    attendance: buildFamilyAttendance(),
    latestFeedback: null,
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
    levels: [
      { name: "Inicial", requiredClasses: 0 },
      { name: "Base", requiredClasses: 10 },
    ],
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
    deliveryClasses: [
      {
        classGroupId: "class-group-1",
        classGroupName: "Natación inicial",
        studentFullName: "Tomás Pérez",
      },
    ],
    ...overrides,
  };
}

export function buildFamilyOrder(overrides: Partial<FamilyOrder> = {}): FamilyOrder {
  return {
    id: "order-1",
    number: 1043,
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
    delivery: "Pickup",
    deliveryClassGroupName: null,
    isReady: false,
    ...overrides,
  };
}

export function buildFamilyNewsItem(overrides: Partial<FamilyNewsItem> = {}): FamilyNewsItem {
  return {
    id: "news-1",
    kind: "Announcement",
    occurredAt: "2026-09-29T10:00:00Z",
    isUnread: true,
    title: "Lunes 12 cerrado",
    body: "Feriado. Las clases se recuperan durante la semana.",
    studentFullNames: [],
    className: null,
    instructorFullName: null,
    classDate: null,
    orderId: null,
    ...overrides,
  };
}

export function buildFamilyNews(overrides: Partial<FamilyNews> = {}): FamilyNews {
  return {
    items: [buildFamilyNewsItem()],
    unreadCount: 1,
    ...overrides,
  };
}
