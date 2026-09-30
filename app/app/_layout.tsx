import "@/global.css";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useFonts } from "expo-font";
import { SplashScreen, Stack } from "expo-router";
import { useEffect, useState } from "react";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { refetchWhenAppReturns } from "@/api/appFocus";
import { SessionProvider } from "@/features/authentication/SessionProvider";
import { useSession } from "@/features/authentication/useSession";
import { PersistedThemePreferences } from "@/theme/PersistedThemePreferences";
import { ThemedStatusBar } from "@/theme/ThemedStatusBar";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { fontAssets } from "@/theme/typography";
import { ToastProvider } from "@/ui/ToastProvider";

SplashScreen.preventAutoHideAsync().catch(() => undefined);
refetchWhenAppReturns();

function RootNavigator() {
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

  return <Stack screenOptions={{ headerShown: false }} />;
}

export default function RootLayout() {
  const [queryClient] = useState(() => new QueryClient());
  return (
    <SafeAreaProvider>
      <ThemeProvider>
        <QueryClientProvider client={queryClient}>
          <SessionProvider>
            <ToastProvider>
              <PersistedThemePreferences />
              <ThemedStatusBar />
              <RootNavigator />
            </ToastProvider>
          </SessionProvider>
        </QueryClientProvider>
      </ThemeProvider>
    </SafeAreaProvider>
  );
}
