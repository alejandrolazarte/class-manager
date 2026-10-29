import {
  StudentAppAttendance,
  StudentAppClassHistory,
  StudentAppClassPacks,
  StudentAppPackClass,
  StudentAppPackPurchase,
  StudentAppPastClass,
  StudentAppHome,
  StudentAppMakeups,
  StudentAppMakeupSlot,
  StudentAppNews,
  StudentAppNewsItem,
  StudentAppNextClass,
  StudentAppOrder,
  StudentAppShop,
  AccountStudent,
} from "@/features/studentApp/types";

export function buildAccessTokenWithClaims(claims: object): string {
  const payload = btoa(JSON.stringify(claims))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=+$/, "");
  return `header.${payload}.signature`;
}

export function buildStudentAccessToken(): string {
  return buildAccessTokenWithClaims({ sub: "user-student", kind: "student" });
}

export function buildStudentAppNextClass(
  overrides: Partial<StudentAppNextClass> = {},
): StudentAppNextClass {
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

export function buildStudentAppPastClass(
  overrides: Partial<StudentAppPastClass> = {},
): StudentAppPastClass {
  return {
    name: "Natación inicial",
    date: "2026-09-22",
    startTime: "18:00",
    endTime: "18:45",
    instructorFullName: "Laura Gómez",
    location: null,
    isPrivateLesson: false,
    classGroupId: "class-group-1",
    status: "Attended",
    isMakeup: false,
    isPackBooking: false,
    paidWithPack: null,
    isUnpaid: false,
    ...overrides,
  };
}

export function buildStudentAppClassHistory(
  overrides: Partial<StudentAppClassHistory> = {},
): StudentAppClassHistory {
  return {
    month: "2026-09",
    classes: [buildStudentAppPastClass()],
    ...overrides,
  };
}

export function buildStudentAppPackClass(
  overrides: Partial<StudentAppPackClass> = {},
): StudentAppPackClass {
  return {
    date: "2026-09-22",
    className: "Natación inicial",
    studentId: "0192f0c5-0000-7000-8000-000000000001",
    isPrivateLesson: false,
    ...overrides,
  };
}

export function buildStudentAppPackPurchase(
  overrides: Partial<StudentAppPackPurchase> = {},
): StudentAppPackPurchase {
  return {
    id: "purchase-1",
    name: "Pack 8 clases",
    classCount: 8,
    purchasedOn: "2026-09-01",
    expiresOn: "2026-10-31",
    usedClasses: 0,
    remainingClasses: 8,
    status: "Active",
    coversAnyGroupClass: false,
    isForPrivateLessons: false,
    classGroupNames: ["Natación inicial"],
    classes: [],
    ...overrides,
  };
}

export function buildStudentAppClassPacks(
  overrides: Partial<StudentAppClassPacks> = {},
): StudentAppClassPacks {
  return {
    availableClasses: 8,
    unpaidClasses: [],
    purchases: [buildStudentAppPackPurchase()],
    ...overrides,
  };
}

export function buildStudentAppMakeupSlot(
  overrides: Partial<StudentAppMakeupSlot> = {},
): StudentAppMakeupSlot {
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

export function buildStudentAppMakeups(
  overrides: Partial<StudentAppMakeups> = {},
): StudentAppMakeups {
  return {
    credits: [{ missedOn: "2026-09-22", reason: "Notice", expiresOn: "2026-10-22" }],
    slots: [buildStudentAppMakeupSlot()],
    ...overrides,
  };
}

export function buildStudentAppAttendance(
  overrides: Partial<StudentAppAttendance> = {},
): StudentAppAttendance {
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

export function buildAccountStudent(overrides: Partial<AccountStudent> = {}): AccountStudent {
  return {
    id: "0192f0c5-0000-7000-8000-000000000001",
    fullName: "Tomás Pérez",
    nextClasses: [buildStudentAppNextClass()],
    attendance: buildStudentAppAttendance(),
    latestFeedback: null,
    materials: [],
    ...overrides,
  };
}

export function buildStudentAppHome(overrides: Partial<StudentAppHome> = {}): StudentAppHome {
  return {
    businessName: "DF Swimming Tenerife",
    currencyCode: "EUR",
    clientFullName: "Ana Pérez",
    signedInFullName: "Ana Pérez",
    pendingGuardianConsents: [],
    location: null,
    students: [buildAccountStudent()],
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

export function buildStudentAppShop(overrides: Partial<StudentAppShop> = {}): StudentAppShop {
  return {
    currencyCode: "EUR",
    packs: [
      {
        id: "pack-8",
        name: "8 clases",
        description: null,
        classCount: 8,
        price: 120,
        validityMonths: 2,
        images: [],
      },
    ],
    products: [
      {
        id: "product-cap",
        name: "Gorro de natación",
        description: null,
        price: 12,
        variants: [
          { id: "variant-s", size: "S", color: null, availability: "Available", maximumUnits: 2 },
          { id: "variant-m", size: "M", color: null, availability: "SoldOut", maximumUnits: 0 },
        ],
        images: [],
      },
    ],
    deliveryClasses: [
      {
        classGroupId: "class-group-1",
        classGroupName: "Natación inicial",
        studentFullName: "Tomás Pérez",
      },
    ],
    canBuy: true,
    ...overrides,
  };
}

export function buildStudentAppOrder(overrides: Partial<StudentAppOrder> = {}): StudentAppOrder {
  return {
    id: "order-1",
    number: 1043,
    status: "Pending",
    paymentStatus: "Unpaid",
    awaitsPickup: false,
    currencyCode: "EUR",
    total: 120,
    refundedAmount: 0,
    paidAmount: 0,
    balance: 120,
    createdAt: "2026-09-29T10:00:00Z",
    lastPaidOn: null,
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

export function buildStudentAppNewsItem(
  overrides: Partial<StudentAppNewsItem> = {},
): StudentAppNewsItem {
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
    classStartTime: null,
    announcementId: null,
    category: null,
    isPinned: false,
    isGoing: null,
    imageUrl: null,
    audienceClassNames: [],
    action: null,
    actionUrl: null,
    ...overrides,
  };
}

export function buildStudentAppNews(overrides: Partial<StudentAppNews> = {}): StudentAppNews {
  return {
    items: [buildStudentAppNewsItem()],
    unreadCount: 1,
    ...overrides,
  };
}
