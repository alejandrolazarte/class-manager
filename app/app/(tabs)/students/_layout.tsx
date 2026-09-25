import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function StudentsLayout() {
  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: translate("students.list.title") }} />
      <Stack.Screen name="new" options={{ title: translate("clients.register.title") }} />
      <Stack.Screen
        name="clients/[clientId]/index"
        options={{ title: translate("clients.detail.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/new-student"
        options={{ title: translate("students.add.title") }}
      />
    </Stack>
  );
}
