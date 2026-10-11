import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { BrandMark } from "@/ui/BrandMark";
import { Breathing } from "@/ui/motion/Breathing";

export function LoadingScreen() {
  return (
    <View
      accessible
      accessibilityRole="progressbar"
      accessibilityLabel={translate("common.loading")}
      className="flex-1 items-center justify-center bg-background"
    >
      <Breathing>
        <BrandMark size="large" />
      </Breathing>
    </View>
  );
}
