import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useFonts } from "expo-font";
import { Stack } from "expo-router";
import { useState } from "react";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { LanguageProvider } from "@/i18n/LanguageProvider";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { fontAssets } from "@/theme/typography";
import { ToastProvider } from "@/ui/ToastProvider";

const stackScreenOptions = { headerShown: false };

function BackofficeNavigator() {
  const [areFontsLoaded, fontLoadingError] = useFonts(fontAssets);
  if (!areFontsLoaded && fontLoadingError === null) {
    return null;
  }
  return <Stack screenOptions={stackScreenOptions} />;
}

export function BackofficeRootLayout() {
  const [queryClient] = useState(() => new QueryClient());
  return (
    <SafeAreaProvider>
      <ThemeProvider>
        <QueryClientProvider client={queryClient}>
          <LanguageProvider>
            <ToastProvider>
              <BackofficeNavigator />
            </ToastProvider>
          </LanguageProvider>
        </QueryClientProvider>
      </ThemeProvider>
    </SafeAreaProvider>
  );
}
