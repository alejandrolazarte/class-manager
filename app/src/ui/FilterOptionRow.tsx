import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { ColorSwatch } from "@/ui/ColorSwatch";
import { Icon } from "@/ui/Icon";
import { Entrance } from "@/ui/motion/Entrance";
import { PressableScale } from "@/ui/motion/PressableScale";

export type FilterChoice = "single" | "multiple";

interface FilterOptionRowProps {
  label: string;
  count: number;
  choice: FilterChoice;
  isSelected: boolean;
  onPress: () => void;
  swatch?: string | null;
}

export function FilterOptionRow({
  label,
  count,
  choice,
  isSelected,
  onPress,
  swatch,
}: FilterOptionRowProps) {
  const markClassName = isSelected ? "border-primary" : "border-border-strong";
  return (
    <PressableScale
      accessibilityRole={choice === "single" ? "radio" : "checkbox"}
      accessibilityLabel={label}
      accessibilityState={{ checked: isSelected }}
      onPress={onPress}
      className="min-h-[52px] flex-row items-center gap-3.5 border-b border-border-subtle active:bg-muted"
    >
      {choice === "single" ? (
        <View
          className={`h-[22px] w-[22px] items-center justify-center rounded-full border-2 ${markClassName}`}
        >
          {isSelected ? (
            <Entrance kind="pop">
              <View className="h-2.5 w-2.5 rounded-full bg-primary" />
            </Entrance>
          ) : null}
        </View>
      ) : (
        <View
          className={`h-[22px] w-[22px] items-center justify-center rounded-md border-2 ${markClassName} ${isSelected ? "bg-primary" : "bg-surface"}`}
        >
          {isSelected ? <Icon name="present" size="small" tone="primary-foreground" /> : null}
        </View>
      )}
      {swatch ? <ColorSwatch swatch={swatch} /> : null}
      <AppText variant="bodyStrong" className="min-w-0 flex-1">
        {label}
      </AppText>
      <AppText variant="caption" tone="subtle">
        {count}
      </AppText>
    </PressableScale>
  );
}
