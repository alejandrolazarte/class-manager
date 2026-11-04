import { ClassOfferCard } from "@/features/studentApp/components/ClassOfferCard";
import { StudentAppShopPack } from "@/features/studentApp/types";
import { formatMoney } from "@/formatting/money";
import { translate, translateCount } from "@/i18n/translate";

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

export function packBadge(pack: StudentAppShopPack): string {
  return translateCount("student.shop.classCount", pack.classCount);
}

export function PackCard({ pack, currencyCode, isInCart, onOpen, onToggle }: PackCardProps) {
  const actionLabel = translate(isInCart ? "student.shop.removePack" : "student.shop.addPack");
  return (
    <ClassOfferCard
      name={pack.name}
      images={pack.images}
      badge={packBadge(pack)}
      details={packDetails(pack, currencyCode)}
      onOpen={onOpen}
      action={
        onToggle === undefined
          ? undefined
          : {
              label: isInCart ? translate("student.shop.inCart") : actionLabel,
              accessibilityLabel: `${actionLabel} ${pack.name}`,
              isSelected: isInCart,
              onPress: onToggle,
            }
      }
    />
  );
}
