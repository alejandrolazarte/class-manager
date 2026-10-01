import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export const unstable_settings = { initialRouteName: "index" };

export default function TodayLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" options={{ title: translate("sessions.day.title") }} />
      <Stack.Screen
        name="[classGroupId]/[sessionDate]"
        options={{ title: translate("sessions.session.title") }}
      />
      <Stack.Screen
        name="notifications"
        options={{ title: translate("teamNotifications.title") }}
      />
      <Stack.Screen name="orders/index" options={{ title: translate("orders.title") }} />
      <Stack.Screen name="orders/new" options={{ title: translate("orders.counterSale.title") }} />
      <Stack.Screen
        name="private/new"
        options={{ title: translate("privateLessons.form.newTitle") }}
      />
      <Stack.Screen
        name="private/[privateLessonId]/index"
        options={{ title: translate("privateLessons.title") }}
      />
      <Stack.Screen
        name="private/[privateLessonId]/edit"
        options={{ title: translate("privateLessons.form.editTitle") }}
      />
    </Stack>
  );
}
