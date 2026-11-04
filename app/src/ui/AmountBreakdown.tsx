import { View } from "react-native";
import { AppText } from "@/ui/AppText";

export interface AmountBreakdownItem {
  key: string;
  label: string;
  amount: string;
  isNested?: boolean;
}

interface AmountBreakdownProps {
  items: readonly AmountBreakdownItem[];
}

export function AmountBreakdown({ items }: AmountBreakdownProps) {
  return (
    <View className="mb-1.5 gap-1.5 border-b border-border-subtle pb-2.5">
      {items.map((item) => (
        <View
          key={item.key}
          className={`flex-row items-baseline justify-between gap-3 ${item.isNested ? "pl-3" : ""}`}
        >
          <AppText
            variant={item.isNested ? "caption" : "body"}
            tone={item.isNested ? "subtle" : "muted"}
            numberOfLines={1}
            className="min-w-0 flex-1"
          >
            {item.label}
          </AppText>
          <AppText
            variant={item.isNested ? "caption" : "bodyStrong"}
            tone={item.isNested ? "subtle" : "default"}
            numberOfLines={1}
          >
            {item.amount}
          </AppText>
        </View>
      ))}
    </View>
  );
}
