import { Image, StyleSheet, View } from "react-native";
import { Icon, IconName } from "@/ui/Icon";

interface CatalogThumbnailProps {
  imageUrl: string | null;
  placeholderIcon: IconName;
}

export function CatalogThumbnail({ imageUrl, placeholderIcon }: CatalogThumbnailProps) {
  return (
    <View className="h-11 w-11 items-center justify-center overflow-hidden rounded-full bg-primary-soft">
      {imageUrl === null ? (
        <Icon name={placeholderIcon} tone="primary-soft-foreground" />
      ) : (
        <Image
          testID="catalog-thumbnail"
          source={{ uri: imageUrl }}
          resizeMode="cover"
          style={StyleSheet.absoluteFill}
        />
      )}
    </View>
  );
}
