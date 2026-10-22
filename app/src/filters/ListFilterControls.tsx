import { useState } from "react";
import { View } from "react-native";
import {
  ListFilter,
  ListFilterValues,
  chosenValues,
  filterLabel,
  isFilterActive,
  noListFilterValues,
  withChosenValues,
  withOptionToggled,
} from "@/filters/listFilters";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";
import { FilterBar } from "@/ui/FilterBar";
import { FilterOptionRow } from "@/ui/FilterOptionRow";
import { FilterSheet } from "@/ui/FilterSheet";

interface ListFilterControlsProps {
  filters: readonly ListFilter[];
  values: ListFilterValues;
  onChange: (values: ListFilterValues) => void;
  countMatching: (values: ListFilterValues) => number;
}

export function ListFilterControls({
  filters,
  values,
  onChange,
  countMatching,
}: ListFilterControlsProps) {
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
        onClear={() => onChange(noListFilterValues)}
      />
      {openFilter === undefined ? null : (
        <FilterSheet
          title={openFilter.title}
          hint={openFilter.hint}
          resultsLabel={translateCount("filters.showResults", countMatching(values))}
          onClear={() => onChange(withChosenValues(values, openFilter.id, []))}
          onClose={() => setOpenFilterId(null)}
        >
          <ListFilterOptions
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

interface ListFilterOptionsProps {
  filter: ListFilter;
  values: ListFilterValues;
  onChange: (values: ListFilterValues) => void;
  countMatching: (values: ListFilterValues) => number;
}

function ListFilterOptions({ filter, values, onChange, countMatching }: ListFilterOptionsProps) {
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

interface ListFilterSummaryProps {
  shownCount: number;
  totalCount: number;
}

export function ListFilterSummary({ shownCount, totalCount }: ListFilterSummaryProps) {
  return (
    <AppText variant="caption" tone="muted">
      {translate("filters.showing", { count: shownCount, total: totalCount })}
    </AppText>
  );
}
