import { View } from "react-native";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { StudentAppShopPack } from "@/features/studentApp/types";
import { formatMoney } from "@/features/fees/money";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

interface PackCardProps {
  pack: StudentAppShopPack;
  currencyCode: string;
  isInCart: boolean;
  onOpen: () => void;
  onToggle: () => void;
}

const detailSeparator = " · ";

export function packDetails(pack: StudentAppShopPack, currencyCode: string): string {
  return [
    formatMoney(pack.price, currencyCode),
    pack.validityMonths === null
      ? null
      : translateCount("student.shop.validity", pack.validityMonths),
    translate("student.shop.pricePerClass", {
      price: formatMoney(Math.round((pack.price / pack.classCount) * 100) / 100, currencyCode),
    }),
  ]
    .filter(Boolean)
    .join(detailSeparator);
}

export function PackClassCountPill({ classCount }: { classCount: number }) {
  return (
    <View
      pointerEvents="none"
      className="absolute left-2.5 top-2.5 h-[26px] flex-row items-center gap-1 rounded-full bg-primary-soft px-[9px]"
    >
      <Icon name="classPacks" size="small" tone="primary-soft-foreground" />
      <AppText variant="badge" tone="primarySoft">
        {translateCount("student.shop.classCount", classCount)}
      </AppText>
    </View>
  );
}

export function PackCard({ pack, currencyCode, isInCart, onOpen, onToggle }: PackCardProps) {
  const actionLabel = translate(isInCart ? "student.shop.removePack" : "student.shop.addPack");
  return (
    <Card className="w-[300px]" accessibilityLabel={pack.name} onPress={onOpen}>
      <View>
        <ProductPhotoCarousel
          images={pack.images}
          sizeClassName="h-[170px]"
          placeholderIcon="classPacks"
        />
        <PackClassCountPill classCount={pack.classCount} />
      </View>
      <View className="flex-1 gap-2 p-3.5">
        <AppText variant="headline">{pack.name}</AppText>
        <AppText variant="caption" tone="muted" className="flex-1">
          {packDetails(pack, currencyCode)}
        </AppText>
        <Button
          size="medium"
          variant={isInCart ? "secondary" : "primary"}
          label={isInCart ? translate("student.shop.inCart") : actionLabel}
          accessibilityLabel={`${actionLabel} ${pack.name}`}
          onPress={onToggle}
        />
      </View>
    </Card>
  );
}
