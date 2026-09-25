import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function ClientsLayout() {
  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: translate("clients.list.title") }} />
      <Stack.Screen name="new" options={{ title: translate("clients.register.title") }} />
      <Stack.Screen name="[clientId]" options={{ title: translate("clients.detail.title") }} />
    </Stack>
  );
}
