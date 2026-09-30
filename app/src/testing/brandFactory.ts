import { Brand } from "@/features/brand/types";

export const brandLogoDataUri = "data:image/png;base64,iVBORw0KGgo=";

export function buildBrand(overrides: Partial<Brand> = {}): Brand {
  return {
    displayName: "Club Delta",
    brandName: "Club Delta",
    themeColor: null,
    accentColor: null,
    locksTheme: false,
    logoUpdatedAt: null,
    ...overrides,
  };
}
