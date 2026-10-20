import { useLocalSearchParams } from "expo-router";
import { View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { googleSignInUrl } from "@/backoffice/session/backofficeSessionApi";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";

export function BackofficeSignInScreen() {
  const { accessRequestCode } = useLocalSearchParams<{ accessRequestCode?: string }>();

  return (
    <SafeAreaView className="flex-1 bg-background">
      <View className="w-full max-w-md flex-1 justify-center gap-4 self-center px-6">
        <BrandMark size="small" />
        <AppText variant="display" accessibilityRole="header">
          {translate("backoffice.signIn.title")}
        </AppText>
        {accessRequestCode ? (
          <Banner message={translate("backoffice.signIn.accessDenied")} tone="warning">
            <AppText tone="warningSoft">{translate("backoffice.signIn.accessDeniedHint")}</AppText>
            <AppText variant="heading" tone="warningSoft" selectable>
              {accessRequestCode}
            </AppText>
          </Banner>
        ) : null}
        <Button
          label={translate("backoffice.signIn.withGoogle")}
          onPress={() => globalThis.location.assign(googleSignInUrl())}
        />
      </View>
    </SafeAreaView>
  );
}
