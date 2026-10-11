import { View } from "react-native";
import { Icon, IconSize } from "@/ui/Icon";
import { useFloatingShadowStyle } from "@/ui/elevation";

type BrandMarkSize = "tiny" | "small" | "large";

const sizeClassNames: Record<BrandMarkSize, string> = {
  tiny: "h-8 w-8 rounded-[9px]",
  small: "h-[52px] w-[52px] rounded-2xl",
  large: "h-28 w-28 rounded-[36px]",
};

const iconSizes: Record<BrandMarkSize, IconSize> = {
  tiny: "medium",
  small: "large",
  large: "hero",
};

export function BrandMark({ size = "tiny" }: { size?: BrandMarkSize }) {
  const elevationStyle = useFloatingShadowStyle();
  return (
    <View
      style={size === "large" ? elevationStyle : undefined}
      className={`items-center justify-center bg-primary ${sizeClassNames[size]}`}
    >
      <Icon name="brand" size={iconSizes[size]} tone="primary-foreground" />
    </View>
  );
}
