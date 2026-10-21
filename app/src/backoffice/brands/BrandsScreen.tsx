import { useQuery } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useState } from "react";
import { FlatList, View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { brandFiltersOf, filterBrands } from "@/backoffice/brands/brandFilters";
import { BrandListItem } from "@/backoffice/brands/BrandListItem";
import { getBrands } from "@/backoffice/brands/brandsApi";
import { brandsQueryKey } from "@/backoffice/brands/brandQueryKeys";
import { ListFilterControls, ListFilterSummary } from "@/filters/ListFilterControls";
import { hasActiveFilters, ListFilterValues, noListFilterValues } from "@/filters/listFilters";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SearchInput } from "@/ui/SearchInput";

export function BrandsScreen() {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const [filterValues, setFilterValues] = useState<ListFilterValues>(noListFilterValues);
  const brands = useQuery({ queryKey: brandsQueryKey, queryFn: getBrands, retry: false });
  const allBrands = brands.data ?? [];
  const visibleBrands = filterBrands(allBrands, search, filterValues);
  const isFiltered = hasActiveFilters(filterValues);

  if (isBackofficeSessionEnded(brands.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  const hasNoBrands = allBrands.length === 0;

  return (
    <Screen>
      <View className="gap-3.5 pb-2">
        <ScreenHeader title={translate("backoffice.brands.title")} />
        <View className="gap-3.5 px-5">
          <SearchInput
            value={search}
            onChangeText={setSearch}
            placeholder={translate("backoffice.brands.searchPlaceholder")}
          />
          <ListFilterControls
            filters={brandFiltersOf()}
            values={filterValues}
            onChange={setFilterValues}
            countMatching={(values) => filterBrands(allBrands, search, values).length}
          />
          {isFiltered ? (
            <ListFilterSummary shownCount={visibleBrands.length} totalCount={allBrands.length} />
          ) : null}
          {brands.isError ? (
            <Banner message={translate("common.unexpectedError")}>
              <Button
                variant="secondary"
                size="medium"
                label={translate("common.retry")}
                onPress={() => brands.refetch()}
              />
            </Banner>
          ) : null}
        </View>
      </View>
      {brands.isPending ? (
        <SkeletonList className="px-5" />
      ) : (
        <FlatList
          data={visibleBrands}
          keyExtractor={(brand) => brand.id}
          keyboardShouldPersistTaps="handled"
          renderItem={({ item: brand, index }) => (
            <AnimatedListItem index={index} className="px-5">
              <BrandListItem
                brand={brand}
                onPress={() => router.push(backofficeRoutes.brand(brand.id))}
              />
            </AnimatedListItem>
          )}
          ListEmptyComponent={
            brands.isError ? null : (
              <EmptyState
                icon={hasNoBrands ? "business" : "search"}
                message={translate(
                  hasNoBrands ? "backoffice.brands.empty" : "backoffice.brands.noResults",
                )}
              />
            )
          }
          refreshing={brands.isRefetching}
          onRefresh={() => brands.refetch()}
          contentContainerClassName="w-full max-w-2xl self-center pb-28"
        />
      )}
    </Screen>
  );
}
