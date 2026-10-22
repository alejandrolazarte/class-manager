import { FilterChoice } from "@/ui/FilterOptionRow";

export interface ListFilterOption {
  value: string;
  label: string;
  swatch?: string | null;
}

export interface ListFilter {
  id: string;
  title: string;
  choice: FilterChoice;
  options: readonly ListFilterOption[];
  allLabel?: string;
  hint?: string;
  showsChips?: boolean;
}

export type ListFilterValues = Readonly<Record<string, readonly string[]>>;

export const noListFilterValues: ListFilterValues = {};

const chosenValuesSeparator = ", ";

export function chosenValues(values: ListFilterValues, filterId: string): readonly string[] {
  return values[filterId] ?? [];
}

export function withChosenValues(
  values: ListFilterValues,
  filterId: string,
  chosen: readonly string[],
): ListFilterValues {
  return { ...values, [filterId]: chosen };
}

export function withOptionToggled(
  values: ListFilterValues,
  filter: ListFilter,
  optionValue: string,
): ListFilterValues {
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

export function isFilterActive(values: ListFilterValues, filterId: string): boolean {
  return chosenValues(values, filterId).length > 0;
}

export function hasActiveFilters(values: ListFilterValues): boolean {
  return Object.keys(values).some((filterId) => isFilterActive(values, filterId));
}

export function isChosenOrUnfiltered(
  values: ListFilterValues,
  filterId: string,
  value: string | null,
): boolean {
  const chosen = chosenValues(values, filterId);
  return chosen.length === 0 || (value !== null && chosen.includes(value));
}

export function filterLabel(filter: ListFilter, values: ListFilterValues): string {
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

export function uniqueOptions(options: readonly ListFilterOption[]): ListFilterOption[] {
  return options.filter(
    (option, index) =>
      option.value.length > 0 &&
      options.findIndex((other) => other.value === option.value) === index,
  );
}
