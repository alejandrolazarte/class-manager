import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function SettingsLayout() {
  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: translate("settings.title") }} />
      <Stack.Screen name="business" options={{ title: translate("businessSettings.title") }} />
    </Stack>
  );
}
