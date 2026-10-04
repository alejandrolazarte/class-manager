import { View } from "react-native";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { StudentAppShopProduct } from "@/features/studentApp/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { StatusPill } from "@/ui/StatusPill";

interface ShopProductCardProps {
  product: StudentAppShopProduct;
  currencyCode: string;
  isInCart: boolean;
  onOpen: () => void;
}

export function isSoldOut(product: StudentAppShopProduct): boolean {
  return product.variants.every((variant) => variant.availability === "SoldOut");
}

function isOnOrder(product: StudentAppShopProduct): boolean {
  return product.variants.some((variant) => variant.availability === "OnOrder");
}

export function ShopProductCard({ product, currencyCode, isInCart, onOpen }: ShopProductCardProps) {
  const soldOut = isSoldOut(product);
  return (
    <Card className="flex-1" accessibilityLabel={product.name} onPress={onOpen}>
      <ProductPhotoCarousel
        images={product.images}
        sizeClassName="aspect-square"
        isDimmed={soldOut}
      />
      {soldOut || isOnOrder(product) ? (
        <View className="absolute left-2.5 top-2.5">
          <StatusPill
            isSmall
            label={translate(soldOut ? "student.shop.soldOut" : "student.shop.onOrder")}
            tone={soldOut ? "neutral" : "warning"}
          />
        </View>
      ) : null}
      <View className="gap-1 px-3 pb-3 pt-2.5">
        <AppText variant="bodyStrong" numberOfLines={2}>
          {product.name}
        </AppText>
        <View className="flex-row items-center justify-between">
          <AppText variant="heading" tone="primary" className="font-heavy">
            {formatMoney(product.price, currencyCode)}
          </AppText>
          <View
            className={`h-8 w-8 items-center justify-center rounded-full ${isInCart ? "bg-success-soft" : "bg-primary-soft"}`}
          >
            <Icon
              name={isInCart ? "present" : "add"}
              size="medium"
              tone={isInCart ? "success-soft-foreground" : "primary-soft-foreground"}
            />
          </View>
        </View>
      </View>
    </Card>
  );
}
