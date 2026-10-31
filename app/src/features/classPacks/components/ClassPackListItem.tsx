import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { CatalogThumbnail } from "@/features/catalogImages/components/CatalogThumbnail";
import { ClassPack } from "@/features/classPacks/types";
import { formatMoney } from "@/formatting/money";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";

interface ClassPackListItemProps {
  classPack: ClassPack;
  onPress?: (classPack: ClassPack) => void;
}

export function ClassPackListItem({ classPack, onPress }: ClassPackListItemProps) {
  const currencyCode = useBusinessCurrency();
  const price = formatMoney(classPack.price, currencyCode);
  const summary =
    classPack.classCount === null
      ? translate("classPacks.list.libreSummary", { price })
      : translateCount("classPacks.list.summary", classPack.classCount, { price });
  const validity = classPack.renewsMonthly
    ? translate("classPacks.list.renewsMonthly")
    : classPack.validityMonths === null
      ? translate("classPacks.list.noExpiry")
      : translateCount("classPacks.list.validity", classPack.validityMonths);
  return (
    <HairlineRow
      isDimmed={!classPack.isActive}
      onPress={onPress ? () => onPress(classPack) : undefined}
      accessibilityLabel={classPack.name}
    >
      <CatalogThumbnail imageUrl={classPack.images[0]?.url ?? null} placeholderIcon="classPacks" />
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">{classPack.name}</AppText>
        <AppText variant="caption" tone="subtle">
          {[
            summary,
            classPack.classDurationMinutes
              ? translate("classPacks.list.duration", { minutes: classPack.classDurationMinutes })
              : null,
            validity,
          ]
            .filter(Boolean)
            .join(" · ")}
        </AppText>
      </View>
      {classPack.isActive ? null : <InactiveChip />}
      {onPress ? <Icon name="next" size="medium" tone="subtle-foreground" /> : null}
    </HairlineRow>
  );
}
