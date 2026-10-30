import {
  CurrentSubscription,
  OrganizationSubscription,
  Plan,
} from "@/features/subscriptions/types";

export function buildSubscription(
  overrides: Partial<CurrentSubscription> = {},
): CurrentSubscription {
  return {
    planCode: "free",
    isActive: true,
    expiredOn: null,
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

export function buildOrganizationSubscription(
  overrides: Partial<OrganizationSubscription> = {},
): OrganizationSubscription {
  return {
    planCode: "free",
    price: 0,
    currency: "EUR",
    createdOn: "2026-10-01T12:00:00Z",
    expiredOn: null,
    isActive: true,
    selfService: false,
    isExternal: false,
    paymentStatus: null,
    offer: null,
    ...overrides,
  };
}
