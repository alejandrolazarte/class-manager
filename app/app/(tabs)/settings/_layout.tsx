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
      <Stack.Screen
        name="class-packs/index"
        options={{ title: translate("classPacks.list.title") }}
      />
      <Stack.Screen
        name="class-packs/new"
        options={{ title: translate("classPacks.form.newTitle") }}
      />
      <Stack.Screen
        name="class-packs/[classPackId]"
        options={{ title: translate("classPacks.form.editTitle") }}
      />
    </Stack>
  );
}
