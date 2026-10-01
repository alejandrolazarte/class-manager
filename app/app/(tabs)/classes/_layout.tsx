import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export const unstable_settings = { initialRouteName: "index" };

export default function ClassesLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
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
      <Stack.Screen
        name="instructors/new"
        options={{ title: translate("instructors.form.newTitle") }}
      />
    </Stack>
  );
}
