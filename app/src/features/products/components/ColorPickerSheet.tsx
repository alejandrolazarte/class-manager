import { useState } from "react";
import { View } from "react-native";
import { productColorKeys, productColorName } from "@/features/products/productColors";
import { translate, translateCount } from "@/i18n/translate";
import { checkColorOn, productColorSwatches } from "@/theme/productColorSwatches";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";
import { Button } from "@/ui/Button";
import { ColorSwatchOption } from "@/ui/ColorSwatch";

interface ColorPickerSheetProps {
  selectedColors: string[];
  onDone: (colors: string[]) => void;
  onClose: () => void;
}

export function ColorPickerSheet({ selectedColors, onDone, onClose }: ColorPickerSheetProps) {
  const [colors, setColors] = useState(selectedColors);
  const toggle = (color: string) =>
    setColors((current) =>
      current.includes(color)
        ? current.filter((currentColor) => currentColor !== color)
        : [...current, color],
    );
  return (
    <BottomSheet onClose={onClose}>
      <AppText variant="headline">{translate("products.form.colors")}</AppText>
      <AppText variant="caption" tone="muted">
        {translate("products.colors.hint")}
      </AppText>
      <View className="flex-row flex-wrap">
        {productColorKeys.map((color) => (
          <ColorSwatchOption
            key={color}
            swatch={productColorSwatches[color]}
            label={productColorName(color)}
            isSelected={colors.includes(color)}
            checkColor={checkColorOn(productColorSwatches[color])}
            onPress={() => toggle(color)}
          />
        ))}
      </View>
      <Button
        label={
          colors.length === 0
            ? translate("common.done")
            : translateCount("products.colors.done", colors.length)
        }
        onPress={() => onDone(productColorKeys.filter((color) => colors.includes(color)))}
      />
    </BottomSheet>
  );
}
