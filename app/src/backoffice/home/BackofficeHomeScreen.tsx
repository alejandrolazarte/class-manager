import { Redirect } from "expo-router";
import { View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { useBackofficeSession } from "@/backoffice/session/useBackofficeSession";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { BrandMark } from "@/ui/BrandMark";
import { Button } from "@/ui/Button";
import { SkeletonScreen } from "@/ui/SkeletonScreen";

export function BackofficeHomeScreen() {
  const { session, signOut } = useBackofficeSession();

  if (session.isPending) {
    return <SkeletonScreen />;
  }
  if (session.data === null) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }

  return (
    <SafeAreaView className="flex-1 bg-background">
      <View className="w-full max-w-md flex-1 justify-center gap-4 self-center px-6">
        <BrandMark size="small" />
        {session.isError ? (
          <>
            <Banner message={translate("common.networkError")} tone="error" />
            <Button
              label={translate("common.retry")}
              onPress={() => session.refetch()}
              isLoading={session.isFetching}
            />
          </>
        ) : (
          <>
            <View className="gap-0.5">
              <AppText variant="display" accessibilityRole="header">
                {translate("backoffice.signIn.title")}
              </AppText>
              <AppText tone="muted">
                {translate("backoffice.home.signedInAs", {
                  email: session.data.email,
                  role: translate(`roles.${session.data.role}` as TranslationKey),
                })}
              </AppText>
            </View>
            <Button
              label={translate("backoffice.home.signOut")}
              variant="secondary"
              onPress={signOut}
            />
          </>
        )}
      </View>
    </SafeAreaView>
  );
}
