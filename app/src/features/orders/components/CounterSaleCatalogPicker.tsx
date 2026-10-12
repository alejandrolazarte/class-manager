import { useState } from "react";
import { View } from "react-native";
import {
  CatalogFilter,
  CatalogFilterValues,
  chosenValues,
  hasActiveFilters,
  isChosenOrUnfiltered,
  noCatalogFilterValues,
} from "@/features/catalogFilters/catalogFilters";
import {
  CatalogFilterControls,
  CatalogFilterSummary,
} from "@/features/catalogFilters/components/CatalogFilterControls";
import {
  availabilityFilter,
  availabilityValues,
  colorFilter,
  productFilterIds,
  sizeFilter,
} from "@/features/catalogFilters/productFilters";
import {
  CounterSaleItem,
  CounterSaleItemKind,
  counterSaleUnitCount,
  UnitsByItem,
  unitsOf,
} from "@/features/orders/counterSale";
import { CounterSaleItemRow } from "@/features/orders/components/CounterSaleItemRow";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { ScreenFooter } from "@/ui/Screen";
import { SearchPage } from "@/ui/SearchPage";

const itemKinds: readonly CounterSaleItemKind[] = ["pack", "product"];

const kindLabelKeys = {
  pack: "orders.counterSale.filter.packs",
  product: "orders.counterSale.filter.products",
} as const;

interface CounterSaleCatalogPickerProps {
  items: readonly CounterSaleItem[];
  unitsByItem: UnitsByItem;
  onChangeUnits: (item: CounterSaleItem, units: number) => void;
  hasStudent: boolean;
  onClose: () => void;
}

function matchesSearch(item: CounterSaleItem, searchText: string): boolean {
  const normalizedSearch = searchText.trim().toLocaleLowerCase();
  return normalizedSearch.length === 0 || item.name.toLocaleLowerCase().includes(normalizedSearch);
}

function availabilityOf(item: CounterSaleItem): string {
  return item.isSoldOut ? availabilityValues.soldOut : availabilityValues.available;
}

function matchesFilters(item: CounterSaleItem, values: CatalogFilterValues): boolean {
  return (
    isChosenOrUnfiltered(values, productFilterIds.kind, item.kind) &&
    isChosenOrUnfiltered(values, productFilterIds.availability, availabilityOf(item)) &&
    isChosenOrUnfiltered(values, productFilterIds.size, item.size) &&
    isChosenOrUnfiltered(values, productFilterIds.color, item.color)
  );
}

function catalogFiltersOf(items: readonly CounterSaleItem[]): CatalogFilter[] {
  const products = items.filter((item) => item.kind === "product");
  const hasPacks = items.some((item) => item.kind === "pack");
  return [
    ...(hasPacks && products.length > 0
      ? [
          {
            id: productFilterIds.kind,
            title: translate("filters.kind"),
            choice: "single" as const,
            allLabel: translate("orders.counterSale.filter.all"),
            options: itemKinds.map((kind) => ({
              value: kind,
              label: translate(kindLabelKeys[kind]),
            })),
          },
        ]
      : []),
    ...(products.length > 0
      ? [
          availabilityFilter({
            offersSoldOut: true,
            hint: hasPacks ? translate("orders.counterSale.packsAlwaysAvailable") : undefined,
          }),
        ]
      : []),
    ...sizeFilter(products.map((item) => item.size ?? "")),
    ...colorFilter(products.map((item) => item.color)),
  ];
}

export function CounterSaleCatalogPicker({
  items,
  unitsByItem,
  onChangeUnits,
  hasStudent,
  onClose,
}: CounterSaleCatalogPickerProps) {
  const [searchText, setSearchText] = useState("");
  const [filterValues, setFilterValues] = useState<CatalogFilterValues>(noCatalogFilterValues);
  const itemsMatching = (values: CatalogFilterValues) =>
    items.filter((item) => matchesFilters(item, values) && matchesSearch(item, searchText));
  const visibleItems = itemsMatching(filterValues);
  const isFiltered = hasActiveFilters(filterValues);
  const showsPacks = !chosenValues(filterValues, productFilterIds.kind).includes("product");
  const hasPacks = items.some((item) => item.kind === "pack");
  const unitCount = counterSaleUnitCount(items, unitsByItem);

  return (
    <SearchPage
      testID="counter-sale-catalog"
      searchText={searchText}
      onSearchTextChange={setSearchText}
      searchPlaceholder={translate("orders.counterSale.catalogSearch")}
      onClose={onClose}
      filters={
        <CatalogFilterControls
          filters={catalogFiltersOf(items)}
          values={filterValues}
          onChange={setFilterValues}
          countMatching={(values) => itemsMatching(values).length}
        />
      }
      footer={
        <ScreenFooter>
          <Button
            label={
              unitCount === 0
                ? translate("common.done")
                : translateCount("orders.counterSale.doneWithCount", unitCount)
            }
            accessibilityLabel={translate("common.done")}
            onPress={onClose}
          />
        </ScreenFooter>
      }
    >
      {hasPacks && !hasStudent && showsPacks ? (
        <Banner tone="warning" message={translate("orders.counterSale.packsNeedStudent")} />
      ) : null}
      {items.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("orders.counterSale.noProducts")}
        </AppText>
      ) : null}
      {isFiltered ? (
        <CatalogFilterSummary shownCount={visibleItems.length} totalCount={items.length} />
      ) : null}
      {items.length > 0 && visibleItems.length === 0 && isFiltered ? (
        <EmptyState icon="filter" message={translate("filters.noResults")} />
      ) : null}
      {items.length > 0 && visibleItems.length === 0 && !isFiltered ? (
        <AppText variant="body" tone="muted">
          {translate("orders.counterSale.noCatalogResults", { search: searchText.trim() })}
        </AppText>
      ) : null}
      <View>
        {visibleItems.map((item) => (
          <CounterSaleItemRow
            key={item.key}
            item={item}
            units={unitsOf(unitsByItem, item)}
            onChangeUnits={(units) => onChangeUnits(item, units)}
            isLocked={item.kind === "pack" && !hasStudent}
          />
        ))}
      </View>
    </SearchPage>
  );
}
