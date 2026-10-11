import { View } from "react-native";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { StudentAppShopProduct } from "@/features/studentApp/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";
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
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={product.name}
      onPress={onOpen}
      className="flex-1 gap-0.5 active:opacity-80"
    >
      <View className="overflow-hidden rounded-[14px]">
        <ProductPhotoCarousel
          images={product.images}
          sizeClassName="aspect-square"
          isDimmed={soldOut}
        />
        {soldOut || isOnOrder(product) ? (
          <View className="absolute left-2 top-2">
            <StatusPill
              isSmall
              label={translate(soldOut ? "student.shop.soldOut" : "student.shop.onOrder")}
              tone={soldOut ? "neutral" : "warning"}
            />
          </View>
        ) : null}
      </View>
      <AppText variant="bodyStrong" numberOfLines={2} className="mt-1.5">
        {product.name}
      </AppText>
      <View className="flex-row items-center justify-between">
        <AppText variant="bodyStrong" tone="primary">
          {formatMoney(product.price, currencyCode)}
        </AppText>
        <View
          className={`h-7 w-7 items-center justify-center rounded-full ${isInCart ? "bg-success-soft" : "bg-primary-soft"}`}
        >
          <Icon
            name={isInCart ? "present" : "add"}
            size="small"
            tone={isInCart ? "success-soft-foreground" : "primary-soft-foreground"}
          />
        </View>
      </View>
    </PressableScale>
  );
}
