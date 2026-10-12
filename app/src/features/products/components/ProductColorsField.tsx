import { useState } from "react";
import { View } from "react-native";
import { ColorPickerSheet } from "@/features/products/components/ColorPickerSheet";
import { productColorName, productColorSwatch } from "@/features/products/productColors";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ColorSwatch } from "@/ui/ColorSwatch";
import { IconButton } from "@/ui/IconButton";

interface ProductColorsFieldProps {
  colors: string[];
  onChange: (colors: string[]) => void;
}

export function ProductColorsField({ colors, onChange }: ProductColorsFieldProps) {
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {translate("products.form.colors")}
      </AppText>
      {colors.length > 0 ? (
        <View className="flex-row flex-wrap gap-1.5">
          {colors.map((color) => {
            const swatch = productColorSwatch(color);
            const name = productColorName(color);
            return (
              <View
                key={color}
                className="h-[34px] flex-row items-center gap-2 rounded-full bg-primary-soft pl-2.5"
              >
                {swatch === null ? null : <ColorSwatch swatch={swatch} />}
                <AppText variant="bodyStrong" tone="primarySoft">
                  {name}
                </AppText>
                <IconButton
                  variant="compact"
                  icon="close"
                  tone="primary-soft-foreground"
                  accessibilityLabel={translate("products.form.colorRemove", { name })}
                  onPress={() => onChange(colors.filter((currentColor) => currentColor !== color))}
                />
              </View>
            );
          })}
        </View>
      ) : null}
      <View className="self-start">
        <Button
          variant="dashed"
          size="medium"
          icon="add"
          label={translate("products.form.chooseColors")}
          onPress={() => setIsPickerOpen(true)}
        />
      </View>
      {isPickerOpen ? (
        <ColorPickerSheet
          selectedColors={colors}
          onDone={(chosenColors) => {
            onChange(chosenColors);
            setIsPickerOpen(false);
          }}
          onClose={() => setIsPickerOpen(false)}
        />
      ) : null}
    </View>
  );
}
