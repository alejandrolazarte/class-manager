import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Entrance } from "@/ui/motion/Entrance";
import { PressableScale } from "@/ui/motion/PressableScale";

interface OptionCardProps {
  label: string;
  hint?: string;
  isSelected: boolean;
  onPress?: () => void;
}

const rowClassName = "flex-row items-center gap-3 border-b border-border-subtle py-3";

export function OptionCard({ label, hint, isSelected, onPress }: OptionCardProps) {
  const content = (
    <>
      <View
        className={`h-5 w-5 items-center justify-center rounded-full border-[1.5px] ${isSelected ? "border-primary" : "border-border-strong"}`}
      >
        {isSelected ? (
          <Entrance kind="pop">
            <View testID="option-selected-mark" className="h-2.5 w-2.5 rounded-full bg-primary" />
          </Entrance>
        ) : null}
      </View>
      <View className="min-w-0 flex-1">
        <AppText variant="bodyStrong">{label}</AppText>
        {hint ? (
          <AppText variant="footnote" tone="subtle">
            {hint}
          </AppText>
        ) : null}
      </View>
    </>
  );
  if (onPress === undefined) {
    return <View className={rowClassName}>{content}</View>;
  }
  return (
    <PressableScale
      accessibilityRole="radio"
      accessibilityLabel={label}
      accessibilityState={{ checked: isSelected }}
      onPress={onPress}
      className={`${rowClassName} active:bg-muted`}
    >
      {content}
    </PressableScale>
  );
}
