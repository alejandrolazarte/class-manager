import { LayoutChangeEvent, View } from "react-native";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

type ChipShape = "pill" | "tile";

interface ChipProps {
  label: string;
  isSelected: boolean;
  onPress: () => void;
  disabled?: boolean;
  accessibilityLabel?: string;
  icon?: IconName;
  shape?: ChipShape;
  className?: string;
  count?: number;
  hasSlidingSelection?: boolean;
  onLayout?: (event: LayoutChangeEvent) => void;
}

const shapeClassNames: Record<ChipShape, string> = {
  pill: "h-10 rounded-full px-4",
  tile: "h-12 rounded-[14px] px-1.5",
};

export function Chip({
  label,
  isSelected,
  onPress,
  disabled = false,
  accessibilityLabel,
  icon,
  shape = "pill",
  className,
  count,
  hasSlidingSelection = false,
  onLayout,
}: ChipProps) {
  const stateClassName = isSelected
    ? hasSlidingSelection
      ? "border-primary bg-transparent"
      : "border-primary bg-primary"
    : disabled
      ? "border-border bg-muted"
      : hasSlidingSelection
        ? "border-border bg-transparent active:bg-muted"
        : "border-border bg-surface active:bg-muted";
  const labelTone: TextTone = isSelected ? "onPrimary" : disabled ? "disabled" : "default";
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityState={{ selected: isSelected, disabled }}
      disabled={disabled}
      onPress={onPress}
      onLayout={onLayout}
      className={`flex-row items-center justify-center gap-1.5 border-[1.5px] ${shapeClassNames[shape]} ${stateClassName} ${className ?? ""}`}
    >
      {icon ? (
        <View>
          <Icon name={icon} size="medium" tone={isSelected ? "primary-foreground" : "foreground"} />
        </View>
      ) : null}
      <AppText
        variant="link"
        tone={labelTone}
        numberOfLines={1}
        className={disabled ? "line-through" : ""}
      >
        {label}
      </AppText>
      {count === undefined ? null : (
        <View
          className={`h-5 min-w-5 items-center justify-center rounded-full px-1.5 ${isSelected ? "bg-primary-foreground" : "bg-muted"}`}
        >
          <AppText variant="badge" tone={isSelected ? "primary" : "muted"}>
            {count}
          </AppText>
        </View>
      )}
    </PressableScale>
  );
}
