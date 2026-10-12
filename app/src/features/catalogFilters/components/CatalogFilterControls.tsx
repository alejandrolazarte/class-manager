import { useState } from "react";
import { View } from "react-native";
import {
  CatalogFilter,
  CatalogFilterValues,
  chosenValues,
  filterLabel,
  isFilterActive,
  noCatalogFilterValues,
  withChosenValues,
  withOptionToggled,
} from "@/features/catalogFilters/catalogFilters";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";
import { FilterBar } from "@/ui/FilterBar";
import { FilterOptionRow } from "@/ui/FilterOptionRow";
import { FilterSheet } from "@/ui/FilterSheet";

interface CatalogFilterControlsProps {
  filters: readonly CatalogFilter[];
  values: CatalogFilterValues;
  onChange: (values: CatalogFilterValues) => void;
  countMatching: (values: CatalogFilterValues) => number;
}

export function CatalogFilterControls({
  filters,
  values,
  onChange,
  countMatching,
}: CatalogFilterControlsProps) {
  const [openFilterId, setOpenFilterId] = useState<string | null>(null);
  const openFilter = filters.find((filter) => filter.id === openFilterId);
  if (filters.length === 0) {
    return null;
  }
  return (
    <>
      <FilterBar
        items={filters.map((filter) => ({
          key: filter.id,
          label: filterLabel(filter, values),
          isActive: isFilterActive(values, filter.id),
          onPress: () => setOpenFilterId(filter.id),
        }))}
        onClear={() => onChange(noCatalogFilterValues)}
      />
      {openFilter === undefined ? null : (
        <FilterSheet
          title={openFilter.title}
          hint={openFilter.hint}
          resultsLabel={translateCount("filters.showResults", countMatching(values))}
          onClear={() => onChange(withChosenValues(values, openFilter.id, []))}
          onClose={() => setOpenFilterId(null)}
        >
          <CatalogFilterOptions
            filter={openFilter}
            values={values}
            onChange={onChange}
            countMatching={countMatching}
          />
        </FilterSheet>
      )}
    </>
  );
}

interface CatalogFilterOptionsProps {
  filter: CatalogFilter;
  values: CatalogFilterValues;
  onChange: (values: CatalogFilterValues) => void;
  countMatching: (values: CatalogFilterValues) => number;
}

function CatalogFilterOptions({
  filter,
  values,
  onChange,
  countMatching,
}: CatalogFilterOptionsProps) {
  const chosen = chosenValues(values, filter.id);
  const countWith = (optionValues: readonly string[]) =>
    countMatching(withChosenValues(values, filter.id, optionValues));
  if (filter.showsChips) {
    return (
      <View className="flex-row flex-wrap gap-2">
        {filter.options.map((option) => (
          <Chip
            key={option.value}
            label={option.label}
            count={countWith([option.value])}
            isSelected={chosen.includes(option.value)}
            onPress={() => onChange(withOptionToggled(values, filter, option.value))}
          />
        ))}
      </View>
    );
  }
  return (
    <View>
      {filter.allLabel === undefined ? null : (
        <FilterOptionRow
          label={filter.allLabel}
          count={countWith([])}
          choice={filter.choice}
          isSelected={chosen.length === 0}
          onPress={() => onChange(withChosenValues(values, filter.id, []))}
        />
      )}
      {filter.options.map((option) => (
        <FilterOptionRow
          key={option.value}
          label={option.label}
          count={countWith([option.value])}
          choice={filter.choice}
          swatch={option.swatch}
          isSelected={chosen.includes(option.value)}
          onPress={() => onChange(withOptionToggled(values, filter, option.value))}
        />
      ))}
    </View>
  );
}

interface CatalogFilterSummaryProps {
  shownCount: number;
  totalCount: number;
}

export function CatalogFilterSummary({ shownCount, totalCount }: CatalogFilterSummaryProps) {
  return (
    <AppText variant="caption" tone="muted">
      {translate("filters.showing", { count: shownCount, total: totalCount })}
    </AppText>
  );
}
