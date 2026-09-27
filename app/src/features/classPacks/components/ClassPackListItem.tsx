import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassPack } from "@/features/classPacks/types";
import { formatMoney } from "@/features/fees/money";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";

interface ClassPackListItemProps {
  classPack: ClassPack;
  onPress: (classPack: ClassPack) => void;
}

export function ClassPackListItem({ classPack, onPress }: ClassPackListItemProps) {
  const currencyCode = useBusinessCurrency();
  const summary = translateCount("classPacks.list.summary", classPack.classCount, {
    price: formatMoney(classPack.price, currencyCode),
  });
  const validity =
    classPack.validityMonths === null
      ? translate("classPacks.list.noExpiry")
      : translateCount("classPacks.list.validity", classPack.validityMonths);
  return (
    <View className={classPack.isActive ? "" : "opacity-60"}>
      <Card
        onPress={() => onPress(classPack)}
        accessibilityLabel={classPack.name}
        className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-3"
      >
        <View className="h-11 w-11 items-center justify-center rounded-full bg-primary-soft">
          <Icon name="classPacks" tone="primary-soft-foreground" />
        </View>
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
        <Icon name="next" tone="subtle-foreground" />
      </Card>
    </View>
  );
}
