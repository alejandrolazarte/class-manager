import { useFonts } from "expo-font";
import { Stack } from "expo-router";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { LanguageProvider } from "@/i18n/LanguageProvider";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { fontAssets } from "@/theme/typography";

const stackScreenOptions = { headerShown: false };

function BackofficeNavigator() {
  const [areFontsLoaded, fontLoadingError] = useFonts(fontAssets);
  if (!areFontsLoaded && fontLoadingError === null) {
    return null;
  }
  return <Stack screenOptions={stackScreenOptions} />;
}

export function BackofficeRootLayout() {
  return (
    <SafeAreaProvider>
      <ThemeProvider>
        <LanguageProvider>
          <BackofficeNavigator />
        </LanguageProvider>
      </ThemeProvider>
    </SafeAreaProvider>
  );
}
