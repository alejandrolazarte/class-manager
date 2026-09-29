import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";

interface OptionCardProps {
  label: string;
  hint?: string;
  isSelected: boolean;
  onPress: () => void;
}

export function OptionCard({ label, hint, isSelected, onPress }: OptionCardProps) {
  return (
    <Pressable
      accessibilityRole="radio"
      accessibilityLabel={label}
      accessibilityState={{ checked: isSelected }}
      onPress={onPress}
      className={`flex-row items-center gap-2.5 rounded-2xl border-[1.5px] bg-surface px-3.5 py-3 active:bg-muted ${isSelected ? "border-primary" : "border-border"}`}
    >
      <Icon
        name={isSelected ? "selected" : "unselected"}
        tone={isSelected ? "primary" : "disabled-foreground"}
      />
      <View className="min-w-0 flex-1">
        <AppText variant="bodyStrong">{label}</AppText>
        {hint ? (
          <AppText variant="caption" tone="subtle">
            {hint}
          </AppText>
        ) : null}
      </View>
    </Pressable>
  );
}
