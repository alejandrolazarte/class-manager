import {
  commandLabel,
  commandOutcomes,
  commandSources,
  eventLabel,
  outcomeLabel,
  securityEventTypes,
  sourceLabel,
} from "@/backoffice/logs/logLabels";
import { CommandLogEntry, SecurityEventEntry } from "@/backoffice/logs/logsApi";
import {
  isChosenOrUnfiltered,
  ListFilter,
  ListFilterOption,
  ListFilterValues,
  uniqueOptions,
} from "@/filters/listFilters";
import { translate } from "@/i18n/translate";

const withoutBrandValue = "none";

export const commandLogFilterIds = {
  brand: "brand",
  outcome: "outcome",
  source: "source",
  command: "command",
} as const;

export const securityEventFilterIds = {
  event: "event",
  brand: "brand",
} as const;

interface BrandOfEntry {
  brandId: string | null;
  brandName: string | null;
}

function brandValueOf(entry: BrandOfEntry): string {
  return entry.brandId ?? withoutBrandValue;
}

function sortedByLabel(options: ListFilterOption[]): ListFilterOption[] {
  return [...options].sort((first, second) => first.label.localeCompare(second.label));
}

function brandFilter(entries: readonly BrandOfEntry[]): ListFilter {
  return {
    id: commandLogFilterIds.brand,
    title: translate("backoffice.logs.filters.brand"),
    choice: "single",
    allLabel: translate("backoffice.logs.filters.allBrands"),
    options: [
      ...sortedByLabel(
        uniqueOptions(
          entries
            .filter((entry) => entry.brandId !== null)
            .map((entry) => ({ value: brandValueOf(entry), label: entry.brandName ?? "" })),
        ),
      ),
      ...(entries.some((entry) => entry.brandId === null)
        ? [{ value: withoutBrandValue, label: translate("backoffice.logs.withoutBrand") }]
        : []),
    ],
  };
}

export function commandLogFiltersOf(entries: readonly CommandLogEntry[]): ListFilter[] {
  return [
    brandFilter(entries),
    {
      id: commandLogFilterIds.outcome,
      title: translate("backoffice.logs.filters.outcome"),
      choice: "multiple",
      showsChips: true,
      options: commandOutcomes.map((outcome) => ({ value: outcome, label: outcomeLabel(outcome) })),
    },
    {
      id: commandLogFilterIds.source,
      title: translate("backoffice.logs.filters.source"),
      choice: "single",
      allLabel: translate("backoffice.logs.filters.allSources"),
      options: commandSources.map((source) => ({ value: source, label: sourceLabel(source) })),
    },
    {
      id: commandLogFilterIds.command,
      title: translate("backoffice.logs.filters.command"),
      choice: "multiple",
      options: sortedByLabel(
        uniqueOptions(
          entries.map((entry) => ({ value: entry.command, label: commandLabel(entry.command) })),
        ),
      ),
    },
  ];
}

export function matchesCommandLogFilters(
  entry: CommandLogEntry,
  values: ListFilterValues,
): boolean {
  return (
    isChosenOrUnfiltered(values, commandLogFilterIds.brand, brandValueOf(entry)) &&
    isChosenOrUnfiltered(values, commandLogFilterIds.outcome, entry.outcome) &&
    isChosenOrUnfiltered(values, commandLogFilterIds.source, entry.source) &&
    isChosenOrUnfiltered(values, commandLogFilterIds.command, entry.command)
  );
}

export function securityEventFiltersOf(events: readonly SecurityEventEntry[]): ListFilter[] {
  return [
    {
      id: securityEventFilterIds.event,
      title: translate("backoffice.logs.filters.event"),
      choice: "multiple",
      showsChips: true,
      options: securityEventTypes.map((event) => ({ value: event, label: eventLabel(event) })),
    },
    brandFilter(events),
  ];
}

export function matchesSecurityEventFilters(
  entry: SecurityEventEntry,
  values: ListFilterValues,
): boolean {
  return (
    isChosenOrUnfiltered(values, securityEventFilterIds.event, entry.event) &&
    isChosenOrUnfiltered(values, securityEventFilterIds.brand, brandValueOf(entry))
  );
}
