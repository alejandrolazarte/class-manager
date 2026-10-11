import { View } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";
import { HairlineColumns } from "@/ui/HairlineColumns";

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
    <View accessible accessibilityLabel={accessibilityLabel}>
      <HairlineColumns>
        {items.map((item) => (
          <View key={item.key}>
            <AppText variant="display" tone={countTone(item)}>
              {item.count}
            </AppText>
            <AppText variant="footnote" tone="subtle">
              {item.label}
            </AppText>
          </View>
        ))}
      </HairlineColumns>
    </View>
  );
}
