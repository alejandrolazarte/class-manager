import { useQuery } from "@tanstack/react-query";
import { Redirect, useRouter } from "expo-router";
import { useMemo, useState } from "react";
import { FlatList, View } from "react-native";
import { isBackofficeSessionEnded } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import {
  allValues,
  filterableStatuses,
  filterBrands,
  planCodes,
  PlanFilter,
  StatusFilter,
} from "@/backoffice/brands/brandFilters";
import { planLabel, statusLabel } from "@/backoffice/brands/brandLabels";
import { BrandListItem } from "@/backoffice/brands/BrandListItem";
import { getBrands } from "@/backoffice/brands/brandsApi";
import { brandsQueryKey } from "@/backoffice/brands/brandQueryKeys";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ChipGroup, ChipGroupOption } from "@/ui/ChipGroup";
import { EmptyState } from "@/ui/EmptyState";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SearchInput } from "@/ui/SearchInput";

export function BrandsScreen() {
  const router = useRouter();
  const [search, setSearch] = useState("");
  const [plan, setPlan] = useState<PlanFilter>(allValues);
  const [status, setStatus] = useState<StatusFilter>(allValues);
  const planOptions: ChipGroupOption<PlanFilter>[] = [
    { value: allValues, label: translate("backoffice.brands.allPlans") },
    ...planCodes.map((planCode) => ({ value: planCode, label: planLabel(planCode) })),
  ];
  const statusOptions: ChipGroupOption<StatusFilter>[] = [
    { value: allValues, label: translate("backoffice.brands.allStatuses") },
    ...filterableStatuses.map((status) => ({ value: status, label: statusLabel(status, null) })),
  ];
  const brands = useQuery({ queryKey: brandsQueryKey, queryFn: getBrands, retry: false });
  const visibleBrands = useMemo(
    () => filterBrands(brands.data ?? [], { search, plan, status }),
    [brands.data, search, plan, status],
  );

  if (isBackofficeSessionEnded(brands.error)) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  const hasNoBrands = (brands.data ?? []).length === 0;

  return (
    <Screen>
      <View className="gap-3.5 pb-2">
        <ScreenHeader
          title={translate("backoffice.brands.title")}
          navigation="back"
          onNavigate={() => router.replace(backofficeRoutes.home)}
        />
        <View className="gap-3.5 px-5">
          <SearchInput
            value={search}
            onChangeText={setSearch}
            placeholder={translate("backoffice.brands.searchPlaceholder")}
          />
          <ChipGroup options={planOptions} selectedValue={plan} onChange={setPlan} isScrollable />
          <ChipGroup
            options={statusOptions}
            selectedValue={status}
            onChange={setStatus}
            isScrollable
          />
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
