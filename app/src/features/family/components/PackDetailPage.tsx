import { View } from "react-native";
import { PackClassCountPill, packDetails } from "@/features/family/components/PackCard";
import { ProductPhotoCarousel } from "@/features/family/components/ProductPhotoCarousel";
import { FamilyShopPack } from "@/features/family/types";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { FullScreenPage } from "@/ui/FullScreenPage";

interface PackDetailPageProps {
  pack: FamilyShopPack;
  currencyCode: string;
  isInCart: boolean;
  onClose: () => void;
  onToggle: () => void;
}

export function PackDetailPage({
  pack,
  currencyCode,
  isInCart,
  onClose,
  onToggle,
}: PackDetailPageProps) {
  const price = formatMoney(pack.price, currencyCode);
  return (
    <FullScreenPage
      title={pack.name}
      onClose={onClose}
      footer={
        <View className="border-t border-border bg-background px-5 py-3">
          <Button
            variant={isInCart ? "secondary" : "primary"}
            label={
              isInCart
                ? translate("family.shop.removePack")
                : translate("family.shop.addToCart", { total: price })
            }
            onPress={onToggle}
          />
        </View>
      }
    >
      <View>
        <ProductPhotoCarousel
          images={pack.images}
          sizeClassName="aspect-square"
          placeholderIcon="classPacks"
          size="page"
        />
        <PackClassCountPill classCount={pack.classCount} />
      </View>
      <View className="gap-3.5 px-5 pb-6 pt-[18px]">
        <View className="gap-1">
          <AppText variant="display">{pack.name}</AppText>
          <AppText variant="headline" tone="primary">
            {price}
          </AppText>
        </View>
        {pack.description ? (
          <AppText variant="body" tone="muted">
            {pack.description}
          </AppText>
        ) : null}
        <AppText variant="label" tone="muted">
          {packDetails(pack, currencyCode)}
        </AppText>
      </View>
    </FullScreenPage>
  );
}
