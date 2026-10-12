import { Stack } from "expo-router";
import { translate } from "@/i18n/translate";
import { useStackScreenOptions } from "@/navigation/stackScreenOptions";

export const unstable_settings = { initialRouteName: "index" };

export default function FeesLayout() {
  const stackScreenOptions = useStackScreenOptions();
  return (
    <Stack screenOptions={stackScreenOptions}>
      <Stack.Screen name="index" options={{ title: translate("fees.month.title") }} />
      <Stack.Screen name="monthly-fee" options={{ title: translate("fees.defaultFee.title") }} />
      <Stack.Screen name="orders/new" options={{ title: translate("orders.counterSale.title") }} />
      <Stack.Screen
        name="clients/[clientId]/index"
        options={{ title: translate("clients.detail.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/edit"
        options={{ title: translate("clients.edit.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/billing-plan"
        options={{ title: translate("fees.billingPlan.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/new-student"
        options={{ title: translate("students.add.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/sell-pack"
        options={{ title: translate("classPacks.sell.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/counter-sale"
        options={{ title: translate("orders.counterSale.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/pay"
        options={{ title: translate("fees.payment.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/new-class-pack"
        options={{ title: translate("classPacks.form.newTitle") }}
      />
      <Stack.Screen
        name="clients/[clientId]/measurements/[studentId]/[measurementId]"
        options={{ title: translate("measurements.title") }}
      />
      <Stack.Screen
        name="clients/[clientId]/record-measurement"
        options={{ title: translate("measurements.recordForm.title") }}
      />
    </Stack>
  );
}
