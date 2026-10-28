import { useLocalSearchParams, useRouter } from "expo-router";
import { useState } from "react";
import { ScrollView, View } from "react-native";
import {
  ListFilter,
  ListFilterValues,
  chosenValues,
  hasActiveFilters,
  isChosenOrUnfiltered,
  noListFilterValues,
} from "@/filters/listFilters";
import { ListFilterControls, ListFilterSummary } from "@/filters/ListFilterControls";
import {
  availabilityFilter,
  availabilityValues,
  colorFilter,
  productFilterIds,
  sizeFilter,
} from "@/features/catalogFilters/productFilters";
import { CartBar } from "@/features/studentApp/components/CartBar";
import { CartPage } from "@/features/studentApp/components/CartPage";
import { PackCard } from "@/features/studentApp/components/PackCard";
import { PackDetailPage } from "@/features/studentApp/components/PackDetailPage";
import { ProductDetailPage } from "@/features/studentApp/components/ProductDetailPage";
import { ShopProductCard } from "@/features/studentApp/components/ShopProductCard";
import { useStudentAppCart } from "@/features/studentApp/StudentAppCartProvider";
import { cartTotal, variantMaximumUnits } from "@/features/studentApp/studentAppCart";
import {
  StudentAppShop,
  StudentAppShopPack,
  StudentAppShopProduct,
  StudentAppShopVariant,
} from "@/features/studentApp/types";
import { useStudentAppShop } from "@/features/studentApp/useStudentAppShop";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { EmptyState } from "@/ui/EmptyState";
import { Notice } from "@/ui/Notice";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SearchInput } from "@/ui/SearchInput";
import { SectionTitle } from "@/ui/SectionTitle";
import { TonalButton } from "@/ui/TonalButton";
import { SkeletonList } from "@/ui/motion/SkeletonList";

const shopKinds = {
  packs: "packs",
  products: "products",
} as const;

const productsPerRow = 2;

function matchesSearch(name: string, searchText: string): boolean {
  return name.toLocaleLowerCase().includes(searchText.trim().toLocaleLowerCase());
}

function availabilityOf(variant: StudentAppShopVariant): string {
  return variant.availability === "SoldOut"
    ? availabilityValues.soldOut
    : availabilityValues.available;
}

function hasMatchingVariant(product: StudentAppShopProduct, values: ListFilterValues): boolean {
  return product.variants.some(
    (variant) =>
      isChosenOrUnfiltered(values, productFilterIds.availability, availabilityOf(variant)) &&
      isChosenOrUnfiltered(values, productFilterIds.size, variant.size) &&
      isChosenOrUnfiltered(values, productFilterIds.color, variant.color),
  );
}

function filteredPacks(
  packs: readonly StudentAppShopPack[],
  values: ListFilterValues,
): StudentAppShopPack[] {
  const isFilteredByVariant =
    chosenValues(values, productFilterIds.size).length > 0 ||
    chosenValues(values, productFilterIds.color).length > 0;
  return isFilteredByVariant ||
    !isChosenOrUnfiltered(values, productFilterIds.kind, shopKinds.packs)
    ? []
    : [...packs];
}

function filteredProducts(
  products: readonly StudentAppShopProduct[],
  values: ListFilterValues,
): StudentAppShopProduct[] {
  return isChosenOrUnfiltered(values, productFilterIds.kind, shopKinds.products)
    ? products.filter((product) => hasMatchingVariant(product, values))
    : [];
}

function shopFiltersOf(shop: StudentAppShop): ListFilter[] {
  const variants = shop.products.flatMap((product) => product.variants);
  return [
    ...(shop.packs.length > 0 && shop.products.length > 0
      ? [
          {
            id: productFilterIds.kind,
            title: translate("filters.kind"),
            choice: "single" as const,
            allLabel: translate("student.shop.all"),
            options: [
              { value: shopKinds.packs, label: translate("student.shop.packs") },
              { value: shopKinds.products, label: translate("student.shop.products") },
            ],
          },
        ]
      : []),
    ...(variants.some((variant) => variant.availability === "SoldOut")
      ? [availabilityFilter({ offersSoldOut: false })]
      : []),
    ...sizeFilter(variants.map((variant) => variant.size)),
    ...colorFilter(variants.map((variant) => variant.color)),
  ];
}

function rowsOf(products: readonly StudentAppShopProduct[]): StudentAppShopProduct[][] {
  const rows: StudentAppShopProduct[][] = [];
  for (let index = 0; index < products.length; index += productsPerRow) {
    rows.push(products.slice(index, index + productsPerRow));
  }
  return rows;
}

function MyOrdersButton({ onPress }: { onPress: () => void }) {
  return (
    <TonalButton
      tone="neutral"
      icon="orders"
      label={translate("student.orders.title")}
      onPress={onPress}
    />
  );
}

export function StudentAppShopScreen() {
  const router = useRouter();
  const { productId } = useLocalSearchParams<{ productId?: string }>();
  const { data: shop, isPending, isError, refetch } = useStudentAppShop();
  useRefetchOnFocus(refetch);
  const { cart, units, setUnits, addUnits } = useStudentAppCart();
  const [searchText, setSearchText] = useState("");
  const [filterValues, setFilterValues] = useState<ListFilterValues>(noListFilterValues);
  const [openProductId, setOpenProductId] = useState<string | null>(null);
  const [openPackId, setOpenPackId] = useState<string | null>(null);
  const [isCartOpen, setIsCartOpen] = useState(false);

  const closeProduct = () => {
    setOpenProductId(null);
    if (productId) {
      router.setParams({ productId: undefined });
    }
  };

  const header = (
    <ScreenHeader
      title={translate("student.shop.title")}
      accessory={<MyOrdersButton onPress={() => router.navigate(routes.studentAppOrders)} />}
    />
  );

  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <SkeletonList />
      </ScrollScreen>
    );
  }
  if (isError || shop === undefined) {
    return (
      <ScrollScreen header={header}>
        <Banner message={translate("student.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      </ScrollScreen>
    );
  }

  const packsMatching = (values: ListFilterValues) =>
    filteredPacks(shop.packs, values).filter((pack) => matchesSearch(pack.name, searchText));
  const productsMatching = (values: ListFilterValues) =>
    filteredProducts(shop.products, values).filter((product) =>
      matchesSearch(product.name, searchText),
    );
  const packs = packsMatching(filterValues);
  const products = productsMatching(filterValues);
  const isFiltered = hasActiveFilters(filterValues);
  const showsPacks = packs.length > 0;
  const showsProducts = products.length > 0;
  const shownProductId = openProductId ?? productId ?? null;
  const openProduct = shop.products.find((product) => product.id === shownProductId);
  const openPack = shop.packs.find((pack) => pack.id === openPackId);
  const isPackInCart = (pack: StudentAppShopPack) => (cart[pack.id] ?? 0) > 0;
  const togglePack = (pack: StudentAppShopPack) => setUnits(pack.id, isPackInCart(pack) ? 0 : 1);
  const isProductInCart = (product: StudentAppShopProduct) =>
    product.variants.some((variant) => (cart[variant.id] ?? 0) > 0);

  return (
    <ScrollScreen
      header={header}
      hasFloatingAction={units > 0}
      overlay={
        units > 0 && !isCartOpen && openProduct === undefined && openPack === undefined ? (
          <CartBar
            units={units}
            totalLabel={formatMoney(cartTotal(shop, cart), shop.currencyCode)}
            onPress={() => setIsCartOpen(true)}
          />
        ) : null
      }
    >
      {shop.canBuy ? null : (
        <Notice tone="primary" message={translate("student.shop.guardianBuys")} />
      )}
      {shop.packs.length === 0 && shop.products.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("student.shop.empty")}
        </AppText>
      ) : (
        <>
          <SearchInput
            value={searchText}
            onChangeText={setSearchText}
            placeholder={translate("student.shop.search")}
          />
          <ListFilterControls
            filters={shopFiltersOf(shop)}
            values={filterValues}
            onChange={setFilterValues}
            countMatching={(values) =>
              packsMatching(values).length + productsMatching(values).length
            }
          />
          {isFiltered ? (
            <ListFilterSummary
              shownCount={packs.length + products.length}
              totalCount={shop.packs.length + shop.products.length}
            />
          ) : null}
        </>
      )}
      {showsPacks ? (
        <>
          <SectionTitle title={translate("student.shop.packs")} />
          <ScrollView
            horizontal
            showsHorizontalScrollIndicator={false}
            className="-mx-5"
            contentContainerClassName="gap-3 px-5 pb-1.5"
          >
            {packs.map((pack) => (
              <PackCard
                key={pack.id}
                pack={pack}
                currencyCode={shop.currencyCode}
                isInCart={isPackInCart(pack)}
                onOpen={() => setOpenPackId(pack.id)}
                onToggle={shop.canBuy ? () => togglePack(pack) : undefined}
              />
            ))}
          </ScrollView>
        </>
      ) : null}
      {showsProducts ? (
        <>
          <SectionTitle title={translate("student.shop.products")} />
          {rowsOf(products).map((row) => (
            <View key={row.map((product) => product.id).join("-")} className="flex-row gap-3">
              {row.map((product) => (
                <ShopProductCard
                  key={product.id}
                  product={product}
                  currencyCode={shop.currencyCode}
                  isInCart={isProductInCart(product)}
                  onOpen={() => setOpenProductId(product.id)}
                />
              ))}
              {row.length < productsPerRow ? <View className="flex-1" /> : null}
            </View>
          ))}
        </>
      ) : null}
      {isFiltered && packs.length === 0 && products.length === 0 ? (
        <EmptyState icon="filter" message={translate("filters.noResults")} />
      ) : null}
      {!isFiltered &&
      searchText.trim().length > 0 &&
      packs.length === 0 &&
      products.length === 0 ? (
        <EmptyState icon="search" message={translate("student.shop.noResults")} />
      ) : null}
      {openProduct === undefined ? null : (
        <ProductDetailPage
          key={openProduct.id}
          product={openProduct}
          currencyCode={shop.currencyCode}
          onClose={closeProduct}
          onAdd={
            shop.canBuy
              ? (variant, addedUnits) => {
                  addUnits(variant.id, addedUnits, variantMaximumUnits(variant));
                  closeProduct();
                }
              : undefined
          }
        />
      )}
      {openPack === undefined ? null : (
        <PackDetailPage
          key={openPack.id}
          pack={openPack}
          currencyCode={shop.currencyCode}
          isInCart={isPackInCart(openPack)}
          onClose={() => setOpenPackId(null)}
          onToggle={
            shop.canBuy
              ? () => {
                  togglePack(openPack);
                  setOpenPackId(null);
                }
              : undefined
          }
        />
      )}
      {isCartOpen ? (
        <CartPage
          shop={shop}
          onClose={() => setIsCartOpen(false)}
          onSeeOrders={() => {
            setIsCartOpen(false);
            router.navigate(routes.studentAppOrders);
          }}
        />
      ) : null}
    </ScrollScreen>
  );
}
