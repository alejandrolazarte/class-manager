import { ClassBalance, ClassPack } from "@/features/classPacks/types";

export function buildClassPack(overrides: Partial<ClassPack> = {}): ClassPack {
  return {
    id: "0192f0c4-0000-7000-8000-00000000c1a5",
    name: "8 clases",
    classCount: 8,
    price: 160,
    validityMonths: 2,
    isActive: true,
    classDurationMinutes: null,
    materialUrl: null,
    classGroupIds: [],
    ...overrides,
  };
}

export function buildClassBalance(overrides: Partial<ClassBalance> = {}): ClassBalance {
  return {
    availableClasses: 5,
    unpaidClasses: 0,
    purchases: [
      {
        id: "0192f0c4-0000-7000-8000-0000000b0a11",
        name: "8 clases",
        classCount: 8,
        price: 150,
        purchasedOn: "2026-09-05",
        expiresOn: "2026-11-04",
        method: "Cash",
        usedClasses: 3,
        remainingClasses: 5,
        status: "Active",
        classDurationMinutes: null,
        materialUrl: null,
        recordedByUserId: null,
      },
    ],
    unpaidAttendances: [],
    deductibleTrials: [],
    ...overrides,
  };
}
