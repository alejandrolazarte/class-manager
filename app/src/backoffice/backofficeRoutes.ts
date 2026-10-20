export const backofficeRoutes = {
  home: "/",
  signIn: "/sign-in",
  brands: "/brands",
  brand: (brandId: string) => `/brands/${encodeURIComponent(brandId)}`,
  changePlan: (brandId: string) => `/brands/${encodeURIComponent(brandId)}/plan`,
  giveAddOn: (brandId: string) => `/brands/${encodeURIComponent(brandId)}/add-on`,
} as const;
