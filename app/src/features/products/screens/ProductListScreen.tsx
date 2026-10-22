import { useRouter } from "expo-router";
import { useState } from "react";
import {
  ListFilter,
  ListFilterValues,
  hasActiveFilters,
  isChosenOrUnfiltered,
  noListFilterValues,
} from "@/filters/listFilters";
import { ListFilterControls, ListFilterSummary } from "@/filters/ListFilterControls";
import {
  availabilityFilter,
  availabilityValues,
  productFilterIds,
  sizeFilter,
} from "@/features/catalogFilters/productFilters";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { ProductListItem } from "@/features/products/components/ProductListItem";
import { Product } from "@/features/products/types";
import { useProducts } from "@/features/products/useProducts";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { LockedFeatureNotice } from "@/features/subscriptions/components/LockedFeatureNotice";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useFeature } from "@/features/subscriptions/useSubscription";
import { SearchInput } from "@/ui/SearchInput";

const statusValues = {
  active: "active",
  inactive: "inactive",
  counterOnly: "counterOnly",
} as const;

function availabilitiesOf(product: Product): string[] {
  if (product.stockMode === "Unlimited") {
    return [availabilityValues.available];
  }
  return [
    ...(product.variants.some((variant) => (variant.stock ?? 0) > 0)
      ? [availabilityValues.available]
      : []),
    ...(product.variants.some((variant) => (variant.stock ?? 0) <= 0)
      ? [availabilityValues.soldOut]
      : []),
  ];
}

function statusesOf(product: Product): string[] {
  if (!product.isActive) {
    return [statusValues.inactive];
  }
  return product.isVisibleInApp
    ? [statusValues.active]
    : [statusValues.active, statusValues.counterOnly];
}

function matchesAny(values: ListFilterValues, filterId: string, candidates: string[]): boolean {
  return (
    isChosenOrUnfiltered(values, filterId, null) ||
    candidates.some((candidate) => isChosenOrUnfiltered(values, filterId, candidate))
  );
}

function matchesFilters(product: Product, values: ListFilterValues): boolean {
  return (
    matchesAny(values, productFilterIds.availability, availabilitiesOf(product)) &&
    matchesAny(values, productFilterIds.status, statusesOf(product)) &&
    matchesAny(
      values,
      productFilterIds.size,
      product.variants.map((variant) => variant.size),
    )
  );
}

function matchesSearch(product: Product, searchText: string): boolean {
  const normalizedSearch = searchText.trim().toLocaleLowerCase();
  return (
    normalizedSearch.length === 0 || product.name.toLocaleLowerCase().includes(normalizedSearch)
  );
}

function productFiltersOf(products: readonly Product[]): ListFilter[] {
  return [
    availabilityFilter({ offersSoldOut: true, hint: translate("products.list.soldOutHint") }),
    {
      id: productFilterIds.status,
      title: translate("filters.status"),
      choice: "single",
      allLabel: translate("filters.status.all"),
      options: [
        { value: statusValues.active, label: translate("filters.status.active") },
        { value: statusValues.inactive, label: translate("filters.status.inactive") },
        { value: statusValues.counterOnly, label: translate("filters.status.counterOnly") },
      ],
    },
    ...sizeFilter(products.flatMap((product) => product.variants.map((variant) => variant.size))),
  ];
}

export function ProductListScreen() {
  const hasFeature = useFeature(featureCodes.shop);
  const router = useRouter();
  const canManageProducts = useCan(permissions.productsManage);
  const { data: products = [], isPending, isError, isRefetching, refetch } = useProducts(true);
  const [searchText, setSearchText] = useState("");
  const [filterValues, setFilterValues] = useState<ListFilterValues>(noListFilterValues);
  const productsMatching = (values: ListFilterValues) =>
    products.filter(
      (product) => matchesFilters(product, values) && matchesSearch(product, searchText),
    );
  const visibleProducts = productsMatching(filterValues);
  const isFiltered = hasActiveFilters(filterValues);
  const isSearching = searchText.trim().length > 0;
  return (
    <SettingsListScreenLayout
      notice={
        hasFeature ? undefined : (
          <LockedFeatureNotice message={translate("subscriptions.locked.shop")} />
        )
      }
      filters={
        products.length === 0 ? undefined : (
          <>
            <SearchInput
              value={searchText}
              onChangeText={setSearchText}
              placeholder={translate("products.list.search")}
            />
            <ListFilterControls
              filters={productFiltersOf(products)}
              values={filterValues}
              onChange={setFilterValues}
              countMatching={(values) => productsMatching(values).length}
            />
            {isFiltered ? (
              <ListFilterSummary shownCount={visibleProducts.length} totalCount={products.length} />
            ) : null}
          </>
        )
      }
      noResultsMessage={
        isFiltered
          ? translate("filters.noResults")
          : isSearching
            ? translate("products.list.noResults", { search: searchText.trim() })
            : undefined
      }
      title={translate("products.list.title")}
      items={visibleProducts}
      isPending={isPending}
      isError={isError}
      isRefetching={isRefetching}
      onRefetch={() => refetch()}
      keyExtractor={(product) => product.id}
      renderItem={(product) => (
        <ProductListItem
          product={product}
          onPress={canManageProducts ? () => router.push(routes.product(product.id)) : undefined}
        />
      )}
      emptyIcon="products"
      emptyMessage={translate("products.list.empty")}
      newItemLabel={translate("products.list.newProduct")}
      onNewItem={canManageProducts ? () => router.push(routes.newProduct) : undefined}
    />
  );
}
