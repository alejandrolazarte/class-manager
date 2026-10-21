import { planLabel, statusLabel } from "@/backoffice/brands/brandLabels";
import { BrandStatus, BrandSummary } from "@/backoffice/brands/brandsApi";
import { isChosenOrUnfiltered, ListFilter, ListFilterValues } from "@/filters/listFilters";
import { translate } from "@/i18n/translate";

const planCodes = ["free", "lite", "pro", "enterprise"] as const;

const filterableStatuses = ["Trial", "Active", "Ended"] as const satisfies BrandStatus[];

export const brandFilterIds = {
  plan: "plan",
  status: "status",
} as const;

export function brandFiltersOf(): ListFilter[] {
  return [
    {
      id: brandFilterIds.plan,
      title: translate("backoffice.brands.filters.plan"),
      choice: "single",
      allLabel: translate("backoffice.brands.allPlans"),
      options: planCodes.map((planCode) => ({ value: planCode, label: planLabel(planCode) })),
    },
    {
      id: brandFilterIds.status,
      title: translate("backoffice.brands.filters.status"),
      choice: "single",
      allLabel: translate("backoffice.brands.allStatuses"),
      options: filterableStatuses.map((status) => ({
        value: status,
        label: statusLabel(status, null),
      })),
    },
  ];
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

export function filterBrands(
  brands: readonly BrandSummary[],
  search: string,
  values: ListFilterValues,
): BrandSummary[] {
  return brands.filter(
    (brand) =>
      matchesSearch(brand, search) &&
      isChosenOrUnfiltered(values, brandFilterIds.plan, brand.planCode) &&
      isChosenOrUnfiltered(values, brandFilterIds.status, brand.status),
  );
}
