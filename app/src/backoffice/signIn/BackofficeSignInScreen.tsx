import { View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BrandMark } from "@/ui/BrandMark";

export function BackofficeSignInScreen() {
  return (
    <SafeAreaView className="flex-1 bg-background">
      <View className="w-full max-w-md flex-1 justify-center gap-4 self-center px-6">
        <BrandMark size="small" />
        <View className="gap-0.5">
          <AppText variant="display" accessibilityRole="header">
            {translate("backoffice.signIn.title")}
          </AppText>
          <AppText tone="muted">{translate("backoffice.signIn.description")}</AppText>
        </View>
      </View>
    </SafeAreaView>
  );
}
