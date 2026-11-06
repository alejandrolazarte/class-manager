import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";
import { useStackScreenOptions } from "@/navigation/stackScreenOptions";

export const unstable_settings = { initialRouteName: "index" };

export default function TodayLayout() {
  const stackScreenOptions = useStackScreenOptions();
  return (
    <Stack screenOptions={stackScreenOptions}>
      <Stack.Screen name="index" options={{ title: translate("sessions.day.title") }} />
      <Stack.Screen
        name="[classGroupId]/[sessionDate]"
        options={{ title: translate("sessions.session.title") }}
      />
      <Stack.Screen
        name="[classGroupId]/measurements"
        options={{ title: translate("measurements.class.title") }}
      />
      <Stack.Screen
        name="notifications"
        options={{ title: translate("teamNotifications.title") }}
      />
      <Stack.Screen name="plan" options={{ title: translate("subscriptions.title") }} />
      <Stack.Screen name="profile" options={{ title: translate("profile.title") }} />
      <Stack.Screen name="edit-profile" options={{ title: translate("profile.edit.title") }} />
      <Stack.Screen name="change-email" options={{ title: translate("profile.email.title") }} />
      <Stack.Screen name="delete-account" options={{ title: translate("profile.delete.title") }} />
      <Stack.Screen name="passkeys" options={{ title: translate("profile.passkeys.title") }} />
      <Stack.Screen name="add-passkey" options={{ title: translate("profile.passkeys.add") }} />
      <Stack.Screen name="orders/index" options={{ title: translate("orders.title") }} />
      <Stack.Screen name="orders/new" options={{ title: translate("orders.counterSale.title") }} />
      <Stack.Screen name="orders/[orderId]" options={{ title: translate("orders.detail.title") }} />
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
