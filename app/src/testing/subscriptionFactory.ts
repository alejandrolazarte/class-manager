import { CurrentSubscription, Plan } from "@/features/subscriptions/types";

export function buildSubscription(
  overrides: Partial<CurrentSubscription> = {},
): CurrentSubscription {
  return {
    planCode: "free",
    isActive: true,
    endsOn: null,
    features: [
      { code: "students", limit: 30 },
      { code: "branches", limit: 1 },
      { code: "team", limit: 0 },
    ],
    ...overrides,
  };
}

export function buildPlans(): Plan[] {
  return [
    {
      code: "free",
      listPrice: 0,
      currency: "USD",
      billingPeriod: "Monthly",
      durationInDays: 30,
      features: [{ code: "students", limit: 30 }],
    },
    {
      code: "lite",
      listPrice: 9,
      currency: "USD",
      billingPeriod: "Monthly",
      durationInDays: null,
      features: [
        { code: "import-export", limit: null },
        { code: "students", limit: 150 },
      ],
    },
    {
      code: "enterprise",
      listPrice: null,
      currency: "USD",
      billingPeriod: "Monthly",
      durationInDays: null,
      features: [{ code: "students", limit: null }],
    },
  ];
}
