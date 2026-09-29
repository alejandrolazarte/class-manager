import { FamilyHome, FamilyNextClass, FamilyStudent } from "@/features/family/types";

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
