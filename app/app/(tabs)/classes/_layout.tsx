import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function ClassesLayout() {
  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: translate("classGroups.week.title") }} />
      <Stack.Screen name="new" options={{ title: translate("classGroups.form.newTitle") }} />
      <Stack.Screen
        name="[classGroupId]/index"
        options={{ title: translate("enrollments.detail.title") }}
      />
      <Stack.Screen
        name="[classGroupId]/edit"
        options={{ title: translate("classGroups.form.editTitle") }}
      />
      <Stack.Screen
        name="[classGroupId]/enroll"
        options={{ title: translate("enrollments.enroll.title") }}
      />
    </Stack>
  );
}
