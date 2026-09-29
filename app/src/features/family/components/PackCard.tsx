import { View } from "react-native";
import { FamilyShopPack } from "@/features/family/types";
import { formatMoney } from "@/features/fees/money";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

interface PackCardProps {
  pack: FamilyShopPack;
  currencyCode: string;
  isInCart: boolean;
  onToggle: () => void;
}

const detailSeparator = " · ";

export function PackCard({ pack, currencyCode, isInCart, onToggle }: PackCardProps) {
  const actionLabel = translate(isInCart ? "family.shop.removePack" : "family.shop.addPack");
  const details = [
    formatMoney(pack.price, currencyCode),
    pack.validityMonths === null
      ? null
      : translateCount("family.shop.validity", pack.validityMonths),
    translate("family.shop.pricePerClass", {
      price: formatMoney(Math.round((pack.price / pack.classCount) * 100) / 100, currencyCode),
    }),
  ];
  return (
    <Card className="w-[172px] gap-2 p-3.5">
      <Icon name="classPacks" tone="primary" />
      <AppText variant="headline">{pack.name}</AppText>
      <AppText variant="caption" tone="muted" className="flex-1">
        {details.filter(Boolean).join(detailSeparator)}
      </AppText>
      <View>
        <Button
          size="medium"
          variant={isInCart ? "secondary" : "primary"}
          label={isInCart ? translate("family.shop.inCart") : actionLabel}
          accessibilityLabel={`${actionLabel} ${pack.name}`}
          onPress={onToggle}
        />
      </View>
    </Card>
  );
}
