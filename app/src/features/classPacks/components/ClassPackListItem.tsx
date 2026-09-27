import { Pressable, View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassPack } from "@/features/classPacks/types";
import { formatMoney } from "@/features/fees/money";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

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
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={classPack.name}
      onPress={() => onPress(classPack)}
      className={`flex-row items-center gap-3 border-b border-border-subtle bg-surface px-4 py-4 ${classPack.isActive ? "" : "opacity-50"}`}
    >
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">{classPack.name}</AppText>
        <AppText variant="caption" tone="muted">{`${summary} · ${validity}`}</AppText>
      </View>
      {classPack.isActive ? null : <InactiveChip />}
    </Pressable>
  );
}
