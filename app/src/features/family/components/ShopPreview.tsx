import { Pressable, ScrollView, View } from "react-native";
import { ProductPhoto } from "@/features/family/components/ProductPhoto";
import { FamilyShop } from "@/features/family/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { SectionTitle } from "@/ui/SectionTitle";

const previewProductLimit = 6;

interface ShopPreviewProps {
  shop: FamilyShop;
  onSeeAll: () => void;
  onOpenProduct: (productId: string) => void;
}

export function ShopPreview({ shop, onSeeAll, onOpenProduct }: ShopPreviewProps) {
  if (shop.products.length === 0) {
    return null;
  }
  return (
    <View className="gap-3">
      <View className="flex-row items-baseline justify-between">
        <SectionTitle title={translate("family.home.shop")} />
        <Pressable accessibilityRole="button" onPress={onSeeAll} className="active:opacity-70">
          <AppText variant="link" tone="primary">
            {translate("family.home.seeAll")}
          </AppText>
        </Pressable>
      </View>
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        className="-mx-5"
        contentContainerClassName="gap-3 px-5 pb-1.5"
      >
        {shop.products.slice(0, previewProductLimit).map((product) => (
          <Card
            key={product.id}
            className="w-[150px]"
            accessibilityLabel={product.name}
            onPress={() => onOpenProduct(product.id)}
          >
            <ProductPhoto heightClassName="h-[120px]" />
            <View className="gap-0.5 px-3 pb-3 pt-2.5">
              <AppText variant="bodyStrong" numberOfLines={1}>
                {product.name}
              </AppText>
              <AppText variant="bodyStrong" tone="primary" className="font-heavy">
                {formatMoney(product.price, shop.currencyCode)}
              </AppText>
            </View>
          </Card>
        ))}
      </ScrollView>
    </View>
  );
}
