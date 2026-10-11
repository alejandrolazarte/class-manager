import { View, ViewStyle } from "react-native";
import { appBrandColor, appBrandMarkColor } from "@/theme/appBrand";
import { useFloatingShadowStyle } from "@/ui/elevation";

type BrandMarkSize = "small" | "medium" | "large";

const sizeClassNames: Record<BrandMarkSize, string> = {
  small: "h-[52px] w-[52px] rounded-2xl",
  medium: "h-16 w-16 rounded-[22px]",
  large: "h-28 w-28 rounded-[36px]",
};

const sizeInPixels: Record<BrandMarkSize, number> = {
  small: 52,
  medium: 64,
  large: 112,
};

const markGridSize = 100;
const markToTileRatio = 0.58;
const markSlant = "-12deg";
const markCornerRadius = 13;
const footOpacity = 0.55;
const stem = { left: 24, top: 14, width: 26, height: 72 };
const foot = { left: 24, top: 60, width: 60, height: 26 };

function scaledPiece(piece: typeof stem, gridUnit: number): ViewStyle {
  return {
    position: "absolute",
    left: piece.left * gridUnit,
    top: piece.top * gridUnit,
    width: piece.width * gridUnit,
    height: piece.height * gridUnit,
    borderRadius: markCornerRadius * gridUnit,
    backgroundColor: appBrandMarkColor,
  };
}

export function BrandMark({ size = "medium" }: { size?: BrandMarkSize }) {
  const elevationStyle = useFloatingShadowStyle();
  const markSize = sizeInPixels[size] * markToTileRatio;
  const gridUnit = markSize / markGridSize;
  return (
    <View
      testID="brand-mark"
      style={[
        { backgroundColor: appBrandColor },
        size === "large" ? { ...elevationStyle, shadowColor: appBrandColor } : undefined,
      ]}
      className={`items-center justify-center ${sizeClassNames[size]}`}
    >
      <View style={{ width: markSize, height: markSize, transform: [{ skewX: markSlant }] }}>
        <View style={scaledPiece(stem, gridUnit)} />
        <View style={[scaledPiece(foot, gridUnit), { opacity: footOpacity }]} />
      </View>
    </View>
  );
}
