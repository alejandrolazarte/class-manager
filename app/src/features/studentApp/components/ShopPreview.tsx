import { ScrollView, View } from "react-native";
import { ProductPhoto } from "@/features/studentApp/components/ProductPhoto";
import { StudentAppShop } from "@/features/studentApp/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { OutlinedBlock } from "@/ui/OutlinedBlock";
import { PressableScale } from "@/ui/motion/PressableScale";
import { SectionTitle } from "@/ui/SectionTitle";
import { TextLink } from "@/ui/TextLink";

const previewProductLimit = 6;

interface ShopPreviewProps {
  shop: StudentAppShop;
  onSeeAll: () => void;
  onOpenProduct: (productId: string) => void;
}

export function ShopPreview({ shop, onSeeAll, onOpenProduct }: ShopPreviewProps) {
  if (shop.products.length === 0) {
    return null;
  }
  return (
    <OutlinedBlock className="gap-2 pt-1">
      <View className="flex-row items-center justify-between">
        <SectionTitle title={translate("student.home.shop")} />
        <TextLink label={translate("student.home.seeAll")} onPress={onSeeAll} />
      </View>
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        className="-mx-4"
        contentContainerClassName="gap-2.5 px-4 pb-1.5"
      >
        {shop.products.slice(0, previewProductLimit).map((product) => (
          <PressableScale
            key={product.id}
            accessibilityRole="button"
            accessibilityLabel={product.name}
            onPress={() => onOpenProduct(product.id)}
            className="w-[112px] gap-0.5 active:opacity-80"
          >
            <ProductPhoto
              imageUrl={product.images[0]?.url ?? null}
              className="mb-1 h-[100px] rounded-xl"
            />
            <AppText variant="link" numberOfLines={1}>
              {product.name}
            </AppText>
            <AppText variant="link" tone="primary">
              {formatMoney(product.price, shop.currencyCode)}
            </AppText>
          </PressableScale>
        ))}
      </ScrollView>
    </OutlinedBlock>
  );
}
