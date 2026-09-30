import { Image, StyleSheet, View } from "react-native";
import { brandInitials } from "@/features/brand/brandInitials";
import { AppText, TextVariant } from "@/ui/AppText";
import { useElevationStyle } from "@/ui/elevation";

export type BrandLogoSize = "small" | "medium" | "large";

interface BrandLogoProps {
  displayName: string;
  logoUri: string | null;
  size?: BrandLogoSize;
  onPrimary?: boolean;
}

const sizeClassNames: Record<BrandLogoSize, string> = {
  small: "h-12 w-12 rounded-2xl",
  medium: "h-[76px] w-[76px] rounded-3xl",
  large: "h-28 w-28 rounded-[36px]",
};

const initialsVariants: Record<BrandLogoSize, TextVariant> = {
  small: "title",
  medium: "headline",
  large: "hero",
};

export function BrandLogo({
  displayName,
  logoUri,
  size = "small",
  onPrimary = false,
}: BrandLogoProps) {
  const elevationStyle = useElevationStyle("card");
  const backgroundClassName =
    logoUri !== null ? "bg-surface" : onPrimary ? "bg-primary-foreground/15" : "bg-primary";
  return (
    <View
      accessibilityRole="image"
      accessibilityLabel={displayName}
      style={onPrimary ? undefined : elevationStyle}
      className={`items-center justify-center overflow-hidden ${backgroundClassName} ${sizeClassNames[size]}`}
    >
      {logoUri !== null ? (
        <Image
          testID="brand-logo-image"
          source={{ uri: logoUri }}
          resizeMode="cover"
          style={StyleSheet.absoluteFill}
        />
      ) : (
        <AppText variant={initialsVariants[size]} tone="onPrimary">
          {brandInitials(displayName)}
        </AppText>
      )}
    </View>
  );
}
