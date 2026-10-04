import { useState } from "react";
import { View } from "react-native";
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
import { Chip } from "@/ui/Chip";
import { ScreenFooter } from "@/ui/Screen";
import { SearchPage } from "@/ui/SearchPage";

type CatalogFilter = "all" | "packs" | "products";

const catalogFilters: readonly CatalogFilter[] = ["all", "packs", "products"];

const filterKinds: Record<CatalogFilter, CounterSaleItemKind | null> = {
  all: null,
  packs: "pack",
  products: "product",
};

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

export function CounterSaleCatalogPicker({
  items,
  unitsByItem,
  onChangeUnits,
  hasStudent,
  onClose,
}: CounterSaleCatalogPickerProps) {
  const [searchText, setSearchText] = useState("");
  const [filter, setFilter] = useState<CatalogFilter>("all");
  const filterKind = filterKinds[filter];
  const visibleItems = items.filter(
    (item) => (filterKind === null || item.kind === filterKind) && matchesSearch(item, searchText),
  );
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
        <View className="flex-row flex-wrap gap-2">
          {catalogFilters.map((catalogFilter) => (
            <Chip
              key={catalogFilter}
              label={translate(`orders.counterSale.filter.${catalogFilter}`)}
              isSelected={filter === catalogFilter}
              onPress={() => setFilter(catalogFilter)}
            />
          ))}
        </View>
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
      {hasPacks && !hasStudent && filter !== "products" ? (
        <Banner tone="warning" message={translate("orders.counterSale.packsNeedStudent")} />
      ) : null}
      {items.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("orders.counterSale.noProducts")}
        </AppText>
      ) : null}
      {items.length > 0 && visibleItems.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("orders.counterSale.noCatalogResults", { search: searchText.trim() })}
        </AppText>
      ) : null}
      {visibleItems.map((item) => (
        <CounterSaleItemRow
          key={item.key}
          item={item}
          units={unitsOf(unitsByItem, item)}
          onChangeUnits={(units) => onChangeUnits(item, units)}
          isLocked={item.kind === "pack" && !hasStudent}
        />
      ))}
    </SearchPage>
  );
}
