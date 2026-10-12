import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

type ColorSwatchSize = "small" | "large";

interface ColorSwatchProps {
  swatch: string;
  size?: ColorSwatchSize;
  isSelected?: boolean;
  checkColor?: string;
}

const swatchClassNames: Record<ColorSwatchSize, string> = {
  small: "h-3.5 w-3.5",
  large: "h-10 w-10",
};

export function ColorSwatch({
  swatch,
  size = "small",
  isSelected = false,
  checkColor,
}: ColorSwatchProps) {
  const circle = (
    <View
      className={`items-center justify-center rounded-full border border-border-strong ${swatchClassNames[size]}`}
      style={{ backgroundColor: swatch }}
    >
      {isSelected && size === "large" ? (
        <Icon name="present" size="medium" color={checkColor} />
      ) : null}
    </View>
  );
  if (size === "small") {
    return circle;
  }
  return (
    <View
      className={`h-[52px] w-[52px] items-center justify-center rounded-full border-2 ${isSelected ? "border-primary" : "border-transparent"}`}
    >
      {circle}
    </View>
  );
}

interface ColorSwatchOptionProps {
  swatch: string;
  label: string;
  isSelected: boolean;
  checkColor: string;
  onPress: () => void;
}

export function ColorSwatchOption({
  swatch,
  label,
  isSelected,
  checkColor,
  onPress,
}: ColorSwatchOptionProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={label}
      accessibilityState={{ selected: isSelected }}
      onPress={onPress}
      className="w-1/5 items-center gap-1 pb-3"
    >
      <ColorSwatch swatch={swatch} size="large" isSelected={isSelected} checkColor={checkColor} />
      <AppText variant="footnote" tone="muted" className="text-center">
        {label}
      </AppText>
    </PressableScale>
  );
}
