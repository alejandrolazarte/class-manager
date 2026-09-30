import { Pressable, View } from "react-native";
import { contrastRatio } from "@/theme/contrastRatio";
import { palette } from "@/theme/palette";
import { Icon } from "@/ui/Icon";

interface ColorSwatchGridProps {
  colors: readonly string[];
  selectedColor: string | null;
  onSelect: (color: string) => void;
  accessibilityLabelPrefix: string;
}

const minimumCheckContrast = 3;

export function ColorSwatchGrid({
  colors,
  selectedColor,
  onSelect,
  accessibilityLabelPrefix,
}: ColorSwatchGridProps) {
  return (
    <View className="flex-row gap-2">
      {colors.map((color) => {
        const isSelected = color === selectedColor?.toLowerCase();
        const checkTone =
          contrastRatio(color, palette.white) >= minimumCheckContrast ? "surface" : "foreground";
        return (
          <Pressable
            key={color}
            accessibilityRole="button"
            accessibilityLabel={`${accessibilityLabelPrefix} ${color}`}
            accessibilityState={{ selected: isSelected }}
            onPress={() => onSelect(color)}
            style={{ backgroundColor: color }}
            className={`aspect-square flex-1 items-center justify-center rounded-[14px] border-[1.5px] ${isSelected ? "border-foreground" : "border-border"}`}
          >
            {isSelected ? <Icon name="present" tone={checkTone} /> : null}
          </Pressable>
        );
      })}
    </View>
  );
}
