import { View } from "react-native";
import { AppText } from "@/ui/AppText";

interface ValueTagProps {
  label: string;
  value: string;
}

export function ValueTag({ label, value }: ValueTagProps) {
  return (
    <View
      accessible
      accessibilityLabel={`${label} ${value}`}
      className="flex-row items-center gap-1 self-start rounded-lg bg-primary-soft px-2 py-[3px]"
    >
      <AppText variant="footnote" tone="primarySoft">
        {label}
      </AppText>
      <AppText variant="badge" tone="primarySoft">
        {value}
      </AppText>
    </View>
  );
}
