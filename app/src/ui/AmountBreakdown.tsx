import { View } from "react-native";
import { AppText } from "@/ui/AppText";

export interface AmountBreakdownItem {
  key: string;
  label: string;
  amount: string;
}

interface AmountBreakdownProps {
  items: readonly AmountBreakdownItem[];
}

export function AmountBreakdown({ items }: AmountBreakdownProps) {
  return (
    <View className="gap-0.5 border-t border-border-subtle pt-1.5">
      {items.map((item) => (
        <View key={item.key} className="flex-row items-baseline justify-between gap-1.5">
          <AppText
            variant="footnote"
            tone="subtle"
            numberOfLines={1}
            className="min-w-0 flex-shrink"
          >
            {item.label}
          </AppText>
          <AppText variant="footnote" tone="subtle" numberOfLines={1}>
            {item.amount}
          </AppText>
        </View>
      ))}
    </View>
  );
}
