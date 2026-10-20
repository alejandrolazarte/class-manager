import { BrandStatus, BrandSummary } from "@/backoffice/brands/brandsApi";

export const allValues = "all";

export const planCodes = ["free", "lite", "pro", "enterprise"] as const;

export const filterableStatuses = ["Trial", "Active", "Ended"] as const satisfies BrandStatus[];

export type PlanFilter = (typeof planCodes)[number] | typeof allValues;

export type StatusFilter = (typeof filterableStatuses)[number] | typeof allValues;

export interface BrandFilters {
  search: string;
  plan: PlanFilter;
  status: StatusFilter;
}

function normalize(text: string): string {
  return text
    .normalize("NFD")
    .replace(/\p{Diacritic}/gu, "")
    .toLowerCase()
    .trim();
}

function matchesSearch(brand: BrandSummary, search: string): boolean {
  const normalizedSearch = normalize(search);
  return (
    normalizedSearch === "" ||
    normalize(brand.name).includes(normalizedSearch) ||
    normalize(brand.ownerEmail ?? "").includes(normalizedSearch)
  );
}

export function filterBrands(brands: BrandSummary[], filters: BrandFilters): BrandSummary[] {
  return brands.filter(
    (brand) =>
      matchesSearch(brand, filters.search) &&
      (filters.plan === allValues || brand.planCode === filters.plan) &&
      (filters.status === allValues || brand.status === filters.status),
  );
}
