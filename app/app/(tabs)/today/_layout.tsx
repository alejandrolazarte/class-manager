import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function TodayLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" options={{ title: translate("sessions.day.title") }} />
      <Stack.Screen
        name="[classGroupId]/[sessionDate]"
        options={{ title: translate("sessions.session.title") }}
      />
    </Stack>
  );
}
