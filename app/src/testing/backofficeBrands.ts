import { BrandDetail, BrandPlanOffer, BrandSummary } from "@/backoffice/brands/brandsApi";
import { SubscriptionCatalog } from "@/backoffice/brands/brandSubscriptionApi";

export function aBrandSummary(overrides: Partial<BrandSummary> = {}): BrandSummary {
  return {
    id: "brand-1",
    name: "Estudio Centro",
    ownerName: "Ana Pérez",
    ownerEmail: "ana@example.com",
    planCode: "pro",
    status: "Active",
    trialDaysLeft: null,
    expiredOn: null,
    branchCount: 2,
    studentCount: 40,
    createdAt: "2026-09-01T10:00:00Z",
    ...overrides,
  };
}

export function aBrandDetail(overrides: Partial<BrandDetail> = {}): BrandDetail {
  return {
    id: "brand-1",
    name: "Estudio Centro",
    createdAt: "2026-09-01T10:00:00Z",
    owners: [{ fullName: "Ana Pérez", email: "ana@example.com" }],
    status: "Active",
    subscription: {
      planCode: "pro",
      price: 29,
      currency: "EUR",
      note: "Precio de lanzamiento",
      createdOn: "2026-10-01T10:00:00Z",
      expiredOn: null,
      status: "Active",
      trialDaysLeft: null,
      isExternal: false,
      paymentStatus: null,
    },
    features: [
      { code: "branches", limit: 3 },
      { code: "shop", limit: null },
    ],
    branches: [
      {
        id: "branch-1",
        name: "Sede Centro",
        countryCode: "AR",
        createdAt: "2026-09-01T10:00:00Z",
        studentCount: 25,
        teamCount: 2,
        storageMegabytes: 12,
      },
    ],
    addOns: [],
    pastSubscriptions: [
      {
        planCode: "free",
        price: 0,
        currency: "EUR",
        note: null,
        createdOn: "2026-09-01T10:00:00Z",
        expiredOn: "2026-09-30",
        deletedAt: "2026-10-01T10:00:00Z",
      },
    ],
    offer: null,
    billingCustomerUrl: null,
    ...overrides,
  };
}

export function aPlanOffer(overrides: Partial<BrandPlanOffer> = {}): BrandPlanOffer {
  return {
    planCode: "lite",
    price: 10,
    currency: "EUR",
    note: "Precio de lanzamiento",
    createdOn: "2026-10-20T10:00:00Z",
    expiredOn: "2026-11-30",
    isActive: true,
    paymentLink: "https://app.example.com/settings/plan?offer=1",
    ...overrides,
  };
}

export function aSubscriptionCatalog(): SubscriptionCatalog {
  return {
    plans: [
      { code: "free", listPrice: 0, currency: "EUR", durationInDays: 30 },
      { code: "lite", listPrice: 19, currency: "EUR", durationInDays: null },
      { code: "pro", listPrice: 29, currency: "EUR", durationInDays: null },
      { code: "enterprise", listPrice: null, currency: "EUR", durationInDays: null },
    ],
    features: [
      { code: "branches", isCounted: true, isAddOn: true, addOnListPrice: 10, currency: "EUR" },
      { code: "shop", isCounted: false, isAddOn: true, addOnListPrice: 5, currency: "EUR" },
    ],
  };
}
