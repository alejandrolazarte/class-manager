import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { CatalogThumbnail } from "@/features/catalogImages/components/CatalogThumbnail";
import { formatMoney } from "@/features/fees/money";
import { stockSummary } from "@/features/products/stockLabels";
import { Product } from "@/features/products/types";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

interface ProductListItemProps {
  product: Product;
  onPress?: (product: Product) => void;
}

export function ProductListItem({ product, onPress }: ProductListItemProps) {
  const currencyCode = useBusinessCurrency();
  const namedVariants = product.variants.filter((variant) => variant.name.length > 0);
  return (
    <View className={product.isActive ? "" : "opacity-60"}>
      <Card
        onPress={onPress ? () => onPress(product) : undefined}
        accessibilityLabel={product.name}
        className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-3"
      >
        <CatalogThumbnail imageUrl={product.images[0]?.url ?? null} placeholderIcon="products" />
        <View className="flex-1 gap-1">
          <AppText variant="bodyStrong">{product.name}</AppText>
          <AppText variant="caption" tone="subtle">
            {[
              formatMoney(product.price, currencyCode),
              namedVariants.length > 0
                ? namedVariants.map((variant) => variant.name).join(", ")
                : null,
              stockSummary(product),
              product.isVisibleInApp ? null : translate("products.list.counterOnly"),
            ]
              .filter(Boolean)
              .join(" · ")}
          </AppText>
        </View>
        {product.isActive ? null : <InactiveChip />}
        {onPress ? <Icon name="next" tone="subtle-foreground" /> : null}
      </Card>
    </View>
  );
}
