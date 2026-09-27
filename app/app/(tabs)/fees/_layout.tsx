import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";

export default function FeesLayout() {
  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" options={{ title: translate("fees.month.title") }} />
      <Stack.Screen name="[clientId]/pay" options={{ title: translate("fees.payment.title") }} />
    </Stack>
  );
}
