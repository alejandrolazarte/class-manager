import { View } from "react-native";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { StudentAppShopPack } from "@/features/studentApp/types";
import { formatMoney } from "@/formatting/money";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { PressableScale } from "@/ui/motion/PressableScale";

interface PackCardProps {
  pack: StudentAppShopPack;
  currencyCode: string;
  isInCart: boolean;
  onOpen: () => void;
  onToggle?: () => void;
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
      className="absolute left-2.5 top-2.5 h-6 flex-row items-center rounded-full bg-surface px-2"
    >
      <AppText variant="badge" tone="primarySoft">
        {translateCount("student.shop.classCount", classCount)}
      </AppText>
    </View>
  );
}

export function PackCard({ pack, currencyCode, isInCart, onOpen, onToggle }: PackCardProps) {
  const actionLabel = translate(isInCart ? "student.shop.removePack" : "student.shop.addPack");
  return (
    <View className="w-[240px] gap-0.5">
      <PressableScale
        accessibilityRole="button"
        accessibilityLabel={pack.name}
        onPress={onOpen}
        className="gap-0.5 active:opacity-80"
      >
        <View className="overflow-hidden rounded-[14px]">
          <ProductPhotoCarousel
            images={pack.images}
            sizeClassName="h-[110px]"
            placeholderIcon="classPacks"
          />
          <PackClassCountPill classCount={pack.classCount} />
        </View>
        <AppText variant="bodyStrong" className="mt-1.5">
          {pack.name}
        </AppText>
        <AppText variant="footnote" tone="muted">
          {packDetails(pack, currencyCode)}
        </AppText>
      </PressableScale>
      {onToggle === undefined ? null : (
        <View className="mt-2">
          <Button
            size="medium"
            variant={isInCart ? "secondary" : "primary"}
            label={isInCart ? translate("student.shop.inCart") : actionLabel}
            accessibilityLabel={`${actionLabel} ${pack.name}`}
            onPress={onToggle}
          />
        </View>
      )}
    </View>
  );
}
