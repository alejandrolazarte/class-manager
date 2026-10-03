import { Linking } from "react-native";
import { appConfiguration } from "@/config/appConfiguration";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";

export function PlansContact() {
  const contactUrl = appConfiguration.plansContactUrl;
  if (contactUrl === null) {
    return (
      <AppText variant="body" tone="muted">
        {translate("subscriptions.contactHint")}
      </AppText>
    );
  }
  return (
    <Button
      label={translate("subscriptions.contact")}
      icon="whatsApp"
      onPress={() => Linking.openURL(contactUrl)}
    />
  );
}
