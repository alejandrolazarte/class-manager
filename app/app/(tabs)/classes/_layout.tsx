import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function ClassesLayout() {
  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: translate("classGroups.week.title") }} />
      <Stack.Screen name="new" options={{ title: translate("classGroups.form.newTitle") }} />
      <Stack.Screen
        name="[classGroupId]"
        options={{ title: translate("classGroups.form.editTitle") }}
      />
    </Stack>
  );
}
