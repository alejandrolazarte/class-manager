import { LayoutChangeEvent, View } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
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

const groupedClassName = "h-8 rounded-full px-3.5";

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
      ? "border-transparent bg-transparent"
      : "border-primary bg-primary"
    : disabled
      ? "border-border bg-muted"
      : hasSlidingSelection
        ? "border-transparent bg-transparent active:bg-muted"
        : "border-border bg-surface active:bg-muted";
  const labelTone: TextTone = isSelected
    ? hasSlidingSelection
      ? "inverse"
      : "onPrimary"
    : disabled
      ? "disabled"
      : hasSlidingSelection
        ? "muted"
        : "default";
  const iconTone: ThemeColorToken = isSelected
    ? hasSlidingSelection
      ? "inverse-foreground"
      : "primary-foreground"
    : hasSlidingSelection
      ? "muted-foreground"
      : "foreground";
  const countClassName = isSelected
    ? hasSlidingSelection
      ? "bg-inverse-foreground/20"
      : "bg-primary-foreground"
    : "bg-muted";
  const countTone: TextTone = isSelected ? (hasSlidingSelection ? "inverse" : "primary") : "muted";
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      accessibilityState={{ selected: isSelected, disabled }}
      disabled={disabled}
      onPress={onPress}
      onLayout={onLayout}
      className={`flex-row items-center justify-center gap-1.5 border-[1.5px] ${hasSlidingSelection ? groupedClassName : shapeClassNames[shape]} ${stateClassName} ${className ?? ""}`}
    >
      {icon ? (
        <View>
          <Icon name={icon} size={hasSlidingSelection ? "small" : "medium"} tone={iconTone} />
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
          className={`h-5 min-w-5 items-center justify-center rounded-full px-1.5 ${countClassName}`}
        >
          <AppText variant="badge" tone={countTone}>
            {count}
          </AppText>
        </View>
      )}
    </PressableScale>
  );
}
