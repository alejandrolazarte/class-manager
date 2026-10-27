import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";
import { useStackScreenOptions } from "@/navigation/stackScreenOptions";

export const unstable_settings = { initialRouteName: "index" };

export default function SettingsLayout() {
  const stackScreenOptions = useStackScreenOptions();
  return (
    <Stack screenOptions={stackScreenOptions}>
      <Stack.Screen name="index" options={{ title: translate("settings.title") }} />
      <Stack.Screen name="business" options={{ title: translate("businessSettings.title") }} />
      <Stack.Screen name="profile" options={{ title: translate("profile.title") }} />
      <Stack.Screen name="edit-profile" options={{ title: translate("profile.edit.title") }} />
      <Stack.Screen name="change-email" options={{ title: translate("profile.email.title") }} />
      <Stack.Screen name="delete-account" options={{ title: translate("profile.delete.title") }} />
      <Stack.Screen name="passkeys" options={{ title: translate("profile.passkeys.title") }} />
      <Stack.Screen name="add-passkey" options={{ title: translate("profile.passkeys.add") }} />
      <Stack.Screen name="feedback" options={{ title: translate("appFeedback.title") }} />
      <Stack.Screen name="brand" options={{ title: translate("brand.settings.title") }} />
      <Stack.Screen name="plan" options={{ title: translate("subscriptions.title") }} />
      <Stack.Screen
        name="achievements"
        options={{ title: translate("achievements.settings.title") }}
      />
      <Stack.Screen name="monthly-fee" options={{ title: translate("fees.billingPlan.title") }} />
      <Stack.Screen
        name="instructors/index"
        options={{ title: translate("instructors.list.title") }}
      />
      <Stack.Screen
        name="instructors/new"
        options={{ title: translate("instructors.form.newTitle") }}
      />
      <Stack.Screen
        name="instructors/import"
        options={{ title: translate("importExport.import.instructors") }}
      />
      <Stack.Screen
        name="instructors/[instructorId]/index"
        options={{ title: translate("instructors.detail.title") }}
      />
      <Stack.Screen
        name="instructors/[instructorId]/edit"
        options={{ title: translate("instructors.form.editTitle") }}
      />
      <Stack.Screen
        name="instructors/[instructorId]/separate"
        options={{ title: translate("people.separate.title") }}
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
      <Stack.Screen name="products/index" options={{ title: translate("products.list.title") }} />
      <Stack.Screen name="products/new" options={{ title: translate("products.form.newTitle") }} />
      <Stack.Screen
        name="products/[productId]"
        options={{ title: translate("products.form.editTitle") }}
      />
      <Stack.Screen name="orders/index" options={{ title: translate("orders.title") }} />
      <Stack.Screen name="orders/new" options={{ title: translate("orders.counterSale.title") }} />
      <Stack.Screen name="orders/[orderId]" options={{ title: translate("orders.detail.title") }} />
      <Stack.Screen
        name="announcements/index"
        options={{ title: translate("announcements.title") }}
      />
      <Stack.Screen
        name="announcements/new"
        options={{ title: translate("announcements.newTitle") }}
      />
      <Stack.Screen
        name="announcements/[announcementId]/index"
        options={{ title: translate("announcements.title") }}
      />
      <Stack.Screen
        name="announcements/[announcementId]/edit"
        options={{ title: translate("announcements.editTitle") }}
      />
      <Stack.Screen
        name="measurements/index"
        options={{ title: translate("measurements.title") }}
      />
      <Stack.Screen name="measurements/new" options={{ title: translate("measurements.new") }} />
      <Stack.Screen name="team/index" options={{ title: translate("team.title") }} />
      <Stack.Screen name="team/invite" options={{ title: translate("team.invite.title") }} />
      <Stack.Screen name="team/[memberId]" options={{ title: translate("team.title") }} />
      <Stack.Screen name="roles/index" options={{ title: translate("roles.title") }} />
      <Stack.Screen name="roles/new" options={{ title: translate("roles.editor.newTitle") }} />
      <Stack.Screen name="roles/[roleKey]" options={{ title: translate("roles.title") }} />
      <Stack.Screen name="branches/index" options={{ title: translate("branches.title") }} />
      <Stack.Screen name="branches/new" options={{ title: translate("branches.form.title") }} />
    </Stack>
  );
}
