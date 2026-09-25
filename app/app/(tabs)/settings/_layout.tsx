import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function SettingsLayout() {
  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: translate("settings.title") }} />
      <Stack.Screen name="business" options={{ title: translate("businessSettings.title") }} />
      <Stack.Screen name="monthly-fee" options={{ title: translate("fees.defaultFee.title") }} />
      <Stack.Screen
        name="instructors/index"
        options={{ title: translate("instructors.list.title") }}
      />
      <Stack.Screen
        name="instructors/new"
        options={{ title: translate("instructors.form.newTitle") }}
      />
      <Stack.Screen
        name="instructors/[instructorId]"
        options={{ title: translate("instructors.form.editTitle") }}
      />
    </Stack>
  );
}
