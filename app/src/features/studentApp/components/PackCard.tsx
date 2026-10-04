import { View } from "react-native";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
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
  onToggle: () => void;
}

const detailSeparator = " · ";

export function PackCard({ pack, currencyCode, isInCart, onToggle }: PackCardProps) {
  const mainImage = pack.images[0];
  const actionLabel = translate(isInCart ? "student.shop.removePack" : "student.shop.addPack");
  const details = [
    formatMoney(pack.price, currencyCode),
    pack.validityMonths === null
      ? null
      : translateCount("student.shop.validity", pack.validityMonths),
    translate("student.shop.pricePerClass", {
      price: formatMoney(Math.round((pack.price / pack.classCount) * 100) / 100, currencyCode),
    }),
  ];
  return (
    <Card className="w-[172px] gap-2 p-3.5">
      {mainImage === undefined ? (
        <Icon name="classPacks" tone="primary" />
      ) : (
        <CatalogImage
          imageUri={mainImage.url}
          placeholderIcon="classPacks"
          className="h-[96px] w-full rounded-2xl"
        />
      )}
      <AppText variant="headline">{pack.name}</AppText>
      <AppText variant="caption" tone="muted" className="flex-1">
        {details.filter(Boolean).join(detailSeparator)}
      </AppText>
      <View>
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
