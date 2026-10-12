import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { CatalogThumbnail } from "@/features/catalogImages/components/CatalogThumbnail";
import { formatMoney } from "@/features/fees/money";
import { productColorName } from "@/features/products/productColors";
import { stockSummary } from "@/features/products/stockLabels";
import { Product } from "@/features/products/types";
import { colorsOf, sizesOf } from "@/features/products/variantOptions";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";

interface ProductListItemProps {
  product: Product;
  onPress?: (product: Product) => void;
}

export function ProductListItem({ product, onPress }: ProductListItemProps) {
  const currencyCode = useBusinessCurrency();
  const sizes = sizesOf(product.variants);
  const colorNames = colorsOf(product.variants).map(productColorName);
  return (
    <HairlineRow
      isDimmed={!product.isActive}
      onPress={onPress ? () => onPress(product) : undefined}
      accessibilityLabel={product.name}
    >
      <CatalogThumbnail imageUrl={product.images[0]?.url ?? null} placeholderIcon="products" />
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">{product.name}</AppText>
        <AppText variant="caption" tone="subtle">
          {[
            formatMoney(product.price, currencyCode),
            sizes.length > 0 ? sizes.join(", ") : null,
            colorNames.length > 0 ? colorNames.join(", ") : null,
            stockSummary(product),
            product.isVisibleInApp ? null : translate("products.list.counterOnly"),
          ]
            .filter(Boolean)
            .join(" · ")}
        </AppText>
      </View>
      {product.isActive ? null : <InactiveChip />}
      {onPress ? <Icon name="next" size="medium" tone="subtle-foreground" /> : null}
    </HairlineRow>
  );
}
