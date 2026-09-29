import { View } from "react-native";
import { Icon } from "@/ui/Icon";

interface ProductPhotoProps {
  heightClassName: string;
  isDimmed?: boolean;
}

export function ProductPhoto({ heightClassName, isDimmed = false }: ProductPhotoProps) {
  return (
    <View
      className={`w-full items-center justify-center bg-muted ${heightClassName} ${isDimmed ? "opacity-50" : ""}`}
    >
      <Icon name="products" size="huge" tone="border-strong" />
    </View>
  );
}
