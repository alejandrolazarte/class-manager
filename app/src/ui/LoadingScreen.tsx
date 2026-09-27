import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { Spinner } from "@/ui/Spinner";

export function LoadingScreen() {
  return (
    <View
      className="flex-1 items-center justify-center bg-background"
      accessibilityLabel={translate("common.loading")}
    >
      <Spinner size="large" />
    </View>
  );
}
