import { View } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";

export type CountTone = "success" | "danger" | "default";

export interface CountsRowItem {
  key: string;
  count: number;
  label: string;
  tone: CountTone;
}

interface CountsRowProps {
  items: readonly CountsRowItem[];
  accessibilityLabel: string;
}

function countTone(item: CountsRowItem): TextTone {
  return item.count === 0 && item.tone !== "default" ? "disabled" : item.tone;
}

export function CountsRow({ items, accessibilityLabel }: CountsRowProps) {
  return (
    <View
      accessible
      accessibilityLabel={accessibilityLabel}
      className="flex-row border-y border-border-subtle"
    >
      {items.map((item, index) => (
        <View
          key={item.key}
          className={`flex-1 py-3 ${index > 0 ? "border-l border-border-subtle pl-3.5" : ""}`}
        >
          <AppText variant="display" tone={countTone(item)}>
            {item.count}
          </AppText>
          <AppText variant="footnote" tone="subtle">
            {item.label}
          </AppText>
        </View>
      ))}
    </View>
  );
}
