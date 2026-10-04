import { Image, StyleSheet, View } from "react-native";
import { Icon, IconName, IconSize } from "@/ui/Icon";

interface CatalogImageProps {
  imageUri: string | null;
  placeholderIcon: IconName;
  className: string;
  isDimmed?: boolean;
  placeholderIconSize?: IconSize;
}

export function CatalogImage({
  imageUri,
  placeholderIcon,
  className,
  isDimmed = false,
  placeholderIconSize = "huge",
}: CatalogImageProps) {
  return (
    <View
      className={`items-center justify-center overflow-hidden bg-muted ${className} ${isDimmed ? "opacity-50" : ""}`}
    >
      {imageUri === null ? (
        <Icon name={placeholderIcon} size={placeholderIconSize} tone="border-strong" />
      ) : (
        <Image
          testID="catalog-image"
          source={{ uri: imageUri }}
          resizeMode="cover"
          style={StyleSheet.absoluteFill}
        />
      )}
    </View>
  );
}
