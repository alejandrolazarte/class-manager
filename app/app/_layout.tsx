import "@/global.css";
import { QueryClientProvider } from "@tanstack/react-query";
import { useFonts } from "expo-font";
import { SplashScreen, Stack } from "expo-router";
import { useEffect, useState } from "react";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { refetchWhenAppReturns } from "@/api/appFocus";
import { startErrorReporting } from "@/errorReporting/startErrorReporting";
import { AppUpdateBanner } from "@/features/appUpdate/AppUpdateBanner";
import { SessionProvider } from "@/features/authentication/SessionProvider";
import { useSession } from "@/features/authentication/useSession";
import { createAppQueryClient } from "@/features/subscriptions/appQueryClient";
import { LanguageProvider } from "@/i18n/LanguageProvider";
import { PersistedThemePreferences } from "@/theme/PersistedThemePreferences";
import { ThemedStatusBar } from "@/theme/ThemedStatusBar";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { fontAssets } from "@/theme/typography";
import { ToastProvider } from "@/ui/ToastProvider";
import { useStackScreenOptions } from "@/navigation/stackScreenOptions";

startErrorReporting();
SplashScreen.preventAutoHideAsync().catch(() => undefined);
refetchWhenAppReturns();

function RootNavigator() {
  const stackScreenOptions = useStackScreenOptions();
  const { session } = useSession();
  const [areFontsLoaded, fontLoadingError] = useFonts(fontAssets);
  const isRestoringSession = session.status === "restoring";
  const areFontsSettled = areFontsLoaded || fontLoadingError !== null;

  useEffect(() => {
    if (!isRestoringSession && areFontsSettled) {
      SplashScreen.hideAsync().catch(() => undefined);
    }
  }, [isRestoringSession, areFontsSettled]);

  if (!areFontsSettled) {
    return null;
  }

  return <Stack screenOptions={stackScreenOptions} />;
}

export default function RootLayout() {
  const [queryClient] = useState(createAppQueryClient);
  return (
    <SafeAreaProvider>
      <ThemeProvider>
        <QueryClientProvider client={queryClient}>
          <SessionProvider>
            <ToastProvider>
              <PersistedThemePreferences />
              <ThemedStatusBar />
              <LanguageProvider>
                <RootNavigator />
                <AppUpdateBanner />
              </LanguageProvider>
            </ToastProvider>
          </SessionProvider>
        </QueryClientProvider>
      </ThemeProvider>
    </SafeAreaProvider>
  );
}
