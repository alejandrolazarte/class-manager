import { ActivityIndicator, View } from "react-native";
import { translate } from "@/i18n/translate";
import { brandColor } from "@/ui/colors";

export function LoadingScreen() {
  return (
    <View
      className="flex-1 items-center justify-center bg-gray-50"
      accessibilityLabel={translate("common.loading")}
    >
      <ActivityIndicator size="large" color={brandColor} />
    </View>
  );
}
