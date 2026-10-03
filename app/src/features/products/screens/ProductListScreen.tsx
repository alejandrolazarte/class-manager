import { useRouter } from "expo-router";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { ProductListItem } from "@/features/products/components/ProductListItem";
import { useProducts } from "@/features/products/useProducts";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";

export function ProductListScreen() {
  const router = useRouter();
  const canManageProducts = useCan(permissions.productsManage);
  const { data: products = [], isPending, isError, isRefetching, refetch } = useProducts(true);
  return (
    <SettingsListScreenLayout
      title={translate("products.list.title")}
      items={products}
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
