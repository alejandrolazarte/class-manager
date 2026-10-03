import { useRouter } from "expo-router";
import { translate } from "@/i18n/translate";
import { planTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

export function LockedFeatureNotice({ message }: { message: string }) {
  const router = useRouter();
  const tab = useCurrentTab(planTabs);
  return (
    <Banner tone="info" icon="locked" message={message}>
      <Button
        variant="secondary"
        size="medium"
        label={translate("subscriptions.seePlans")}
        onPress={() => router.push(routes.plan(tab))}
      />
    </Banner>
  );
}
