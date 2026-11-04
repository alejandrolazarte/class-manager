import { StudentAppPlanStudent, StudentAppShopPlan } from "@/features/studentApp/types";

export function buildStudentAppShopPlan(
  overrides: Partial<StudentAppShopPlan> = {},
): StudentAppShopPlan {
  return {
    id: "plan-monthly",
    name: "Principiante",
    description: null,
    classCount: 8,
    price: 20,
    images: [],
    ...overrides,
  };
}

export function buildStudentAppPlanStudent(
  overrides: Partial<StudentAppPlanStudent> = {},
): StudentAppPlanStudent {
  return {
    studentId: "student-1",
    fullName: "Tomás Pérez",
    currentPlanId: null,
    currentPlanName: null,
    nextMonthPlanId: null,
    ...overrides,
  };
}
