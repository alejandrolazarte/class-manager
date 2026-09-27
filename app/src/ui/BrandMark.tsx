import { View } from "react-native";
import { Icon } from "@/ui/Icon";
import { useElevationStyle } from "@/ui/elevation";

type BrandMarkSize = "small" | "medium" | "large";

const sizeClassNames: Record<BrandMarkSize, string> = {
  small: "h-[52px] w-[52px] rounded-2xl",
  medium: "h-16 w-16 rounded-[22px]",
  large: "h-28 w-28 rounded-[36px]",
};

export function BrandMark({ size = "medium" }: { size?: BrandMarkSize }) {
  const elevationStyle = useElevationStyle("floating");
  return (
    <View
      style={size === "large" ? elevationStyle : undefined}
      className={`items-center justify-center bg-primary ${sizeClassNames[size]}`}
    >
      <Icon
        name="brand"
        size={size === "large" ? "hero" : size === "medium" ? "huge" : "extraLarge"}
        tone="primary-foreground"
      />
    </View>
  );
}
