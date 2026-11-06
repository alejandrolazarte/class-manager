export const backofficeRoutes = {
  home: "/",
  signIn: "/sign-in",
  brands: "/brands",
  brand: (brandId: string) => `/brands/${encodeURIComponent(brandId)}`,
  changePlan: (brandId: string) => `/brands/${encodeURIComponent(brandId)}/plan`,
  offerPlan: (brandId: string) => `/brands/${encodeURIComponent(brandId)}/offer`,
  giveAddOn: (brandId: string) => `/brands/${encodeURIComponent(brandId)}/add-on`,
  brandSalesFee: (brandId: string) => `/brands/${encodeURIComponent(brandId)}/sales-fee`,
  plans: "/plans",
  planSalesFee: (planCode: string) => `/plans/${encodeURIComponent(planCode)}`,
  logs: "/logs",
  operators: "/operators",
  enableOperator: (accessRequestId: string) =>
    `/operators/requests/${encodeURIComponent(accessRequestId)}`,
} as const;
