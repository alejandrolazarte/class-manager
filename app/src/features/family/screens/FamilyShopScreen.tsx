import { useLocalSearchParams, useRouter } from "expo-router";
import { useState } from "react";
import { Pressable, ScrollView, View } from "react-native";
import { CartBar } from "@/features/family/components/CartBar";
import { CartSheet } from "@/features/family/components/CartSheet";
import { PackCard } from "@/features/family/components/PackCard";
import { ProductSheet } from "@/features/family/components/ProductSheet";
import { ShopProductCard } from "@/features/family/components/ShopProductCard";
import { useFamilyCart } from "@/features/family/FamilyCartProvider";
import { cartTotal } from "@/features/family/familyCart";
import { FamilyShopProduct } from "@/features/family/types";
import { useFamilyShop } from "@/features/family/useFamilyShop";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { formatMoney } from "@/features/fees/money";
import { translate, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { EmptyState } from "@/ui/EmptyState";
import { Icon } from "@/ui/Icon";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SearchInput } from "@/ui/SearchInput";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";
import { useElevationStyle } from "@/ui/elevation";

type ShopCategory = "all" | "packs" | "products";

const shopCategories: Record<ShopCategory, TranslationKey> = {
  all: "family.shop.all",
  packs: "family.shop.packs",
  products: "family.shop.products",
};

const productsPerRow = 2;

function matchesSearch(name: string, searchText: string): boolean {
  return name.toLocaleLowerCase().includes(searchText.trim().toLocaleLowerCase());
}

function rowsOf(products: readonly FamilyShopProduct[]): FamilyShopProduct[][] {
  const rows: FamilyShopProduct[][] = [];
  for (let index = 0; index < products.length; index += productsPerRow) {
    rows.push(products.slice(index, index + productsPerRow));
  }
  return rows;
}

function MyOrdersButton({ onPress }: { onPress: () => void }) {
  const elevationStyle = useElevationStyle();
  return (
    <Pressable
      accessibilityRole="button"
      onPress={onPress}
      style={elevationStyle}
      className="h-10 flex-row items-center gap-1.5 rounded-full bg-surface px-3 active:bg-muted"
    >
      <Icon name="orders" size="medium" tone="primary" />
      <AppText variant="link">{translate("family.orders.title")}</AppText>
    </Pressable>
  );
}

export function FamilyShopScreen() {
  const router = useRouter();
  const { productId } = useLocalSearchParams<{ productId?: string }>();
  const { data: shop, isPending, isError, refetch } = useFamilyShop();
  useRefetchOnFocus(refetch);
  const { cart, units, setUnits, addUnits } = useFamilyCart();
  const [searchText, setSearchText] = useState("");
  const [category, setCategory] = useState<ShopCategory>("all");
  const [openProductId, setOpenProductId] = useState<string | null>(null);
  const [isCartOpen, setIsCartOpen] = useState(false);

  const closeProduct = () => {
    setOpenProductId(null);
    if (productId) {
      router.setParams({ productId: undefined });
    }
  };

  const header = (
    <ScreenHeader
      title={translate("family.shop.title")}
      accessory={<MyOrdersButton onPress={() => router.navigate(routes.familyOrders)} />}
    />
  );

  if (isPending) {
    return (
      <ScrollScreen header={header}>
        <Spinner className="mt-6" />
      </ScrollScreen>
    );
  }
  if (isError || shop === undefined) {
    return (
      <ScrollScreen header={header}>
        <Banner message={translate("family.loadError")}>
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

  const packs = shop.packs.filter((pack) => matchesSearch(pack.name, searchText));
  const products = shop.products.filter((product) => matchesSearch(product.name, searchText));
  const showsPacks = category !== "products" && packs.length > 0;
  const showsProducts = category !== "packs" && products.length > 0;
  const shownProductId = openProductId ?? productId ?? null;
  const openProduct = shop.products.find((product) => product.id === shownProductId);
  const isProductInCart = (product: FamilyShopProduct) =>
    product.variants.some((variant) => (cart[variant.id] ?? 0) > 0);

  return (
    <ScrollScreen
      header={header}
      hasFloatingAction={units > 0}
      overlay={
        units > 0 && !isCartOpen && openProduct === undefined ? (
          <CartBar
            units={units}
            totalLabel={formatMoney(cartTotal(shop, cart), shop.currencyCode)}
            onPress={() => setIsCartOpen(true)}
          />
        ) : null
      }
    >
      {shop.packs.length === 0 && shop.products.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("family.shop.empty")}
        </AppText>
      ) : (
        <>
          <SearchInput
            value={searchText}
            onChangeText={setSearchText}
            placeholder={translate("family.shop.search")}
          />
          {shop.packs.length > 0 && shop.products.length > 0 ? (
            <ScrollView
              horizontal
              showsHorizontalScrollIndicator={false}
              className="-mx-5"
              contentContainerClassName="gap-2 px-5"
            >
              {(Object.keys(shopCategories) as ShopCategory[]).map((shopCategory) => (
                <Chip
                  key={shopCategory}
                  label={translate(shopCategories[shopCategory])}
                  isSelected={category === shopCategory}
                  onPress={() => setCategory(shopCategory)}
                />
              ))}
            </ScrollView>
          ) : null}
        </>
      )}
      {showsPacks ? (
        <>
          <SectionTitle title={translate("family.shop.packs")} />
          <ScrollView
            horizontal
            showsHorizontalScrollIndicator={false}
            className="-mx-5"
            contentContainerClassName="gap-3 px-5 pb-1.5"
          >
            {packs.map((pack) => {
              const isInCart = (cart[pack.id] ?? 0) > 0;
              return (
                <PackCard
                  key={pack.id}
                  pack={pack}
                  currencyCode={shop.currencyCode}
                  isInCart={isInCart}
                  onToggle={() => setUnits(pack.id, isInCart ? 0 : 1)}
                />
              );
            })}
          </ScrollView>
        </>
      ) : null}
      {showsProducts ? (
        <>
          <SectionTitle title={translate("family.shop.products")} />
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
      {searchText.trim().length > 0 && packs.length === 0 && products.length === 0 ? (
        <EmptyState icon="search" message={translate("family.shop.noResults")} />
      ) : null}
      {openProduct === undefined ? null : (
        <ProductSheet
          key={openProduct.id}
          product={openProduct}
          currencyCode={shop.currencyCode}
          onClose={closeProduct}
          onAdd={(variantId, addedUnits) => {
            addUnits(variantId, addedUnits);
            closeProduct();
          }}
        />
      )}
      {isCartOpen ? (
        <CartSheet
          shop={shop}
          onClose={() => setIsCartOpen(false)}
          onSeeOrders={() => {
            setIsCartOpen(false);
            router.navigate(routes.familyOrders);
          }}
        />
      ) : null}
    </ScrollScreen>
  );
}
