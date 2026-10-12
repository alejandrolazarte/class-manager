import { FilterChoice } from "@/ui/FilterOptionRow";

export interface CatalogFilterOption {
  value: string;
  label: string;
  swatch?: string | null;
}

export interface CatalogFilter {
  id: string;
  title: string;
  choice: FilterChoice;
  options: readonly CatalogFilterOption[];
  allLabel?: string;
  hint?: string;
  showsChips?: boolean;
}

export type CatalogFilterValues = Readonly<Record<string, readonly string[]>>;

export const noCatalogFilterValues: CatalogFilterValues = {};

const chosenValuesSeparator = ", ";

export function chosenValues(values: CatalogFilterValues, filterId: string): readonly string[] {
  return values[filterId] ?? [];
}

export function withChosenValues(
  values: CatalogFilterValues,
  filterId: string,
  chosen: readonly string[],
): CatalogFilterValues {
  return { ...values, [filterId]: chosen };
}

export function withOptionToggled(
  values: CatalogFilterValues,
  filter: CatalogFilter,
  optionValue: string,
): CatalogFilterValues {
  const current = chosenValues(values, filter.id);
  if (filter.choice === "single") {
    return withChosenValues(values, filter.id, [optionValue]);
  }
  return withChosenValues(
    values,
    filter.id,
    current.includes(optionValue)
      ? current.filter((value) => value !== optionValue)
      : [...current, optionValue],
  );
}

export function isFilterActive(values: CatalogFilterValues, filterId: string): boolean {
  return chosenValues(values, filterId).length > 0;
}

export function hasActiveFilters(values: CatalogFilterValues): boolean {
  return Object.keys(values).some((filterId) => isFilterActive(values, filterId));
}

export function isChosenOrUnfiltered(
  values: CatalogFilterValues,
  filterId: string,
  value: string | null,
): boolean {
  const chosen = chosenValues(values, filterId);
  return chosen.length === 0 || (value !== null && chosen.includes(value));
}

export function filterLabel(filter: CatalogFilter, values: CatalogFilterValues): string {
  const chosenLabels = filter.options
    .filter((option) => chosenValues(values, filter.id).includes(option.value))
    .map((option) => option.label);
  if (chosenLabels.length === 0) {
    return filter.title;
  }
  if (filter.choice === "single") {
    return chosenLabels[0];
  }
  return `${filter.title}: ${chosenLabels.join(chosenValuesSeparator)}`;
}

export function uniqueOptions(options: readonly CatalogFilterOption[]): CatalogFilterOption[] {
  return options.filter(
    (option, index) =>
      option.value.length > 0 &&
      options.findIndex((other) => other.value === option.value) === index,
  );
}
