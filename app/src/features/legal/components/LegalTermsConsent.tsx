import { useRouter } from "expo-router";
import { View } from "react-native";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Checkbox } from "@/ui/Checkbox";
import { TextLink } from "@/ui/TextLink";

interface LegalTermsConsentProps {
  isAccepted: boolean;
  onAcceptedChange: (isAccepted: boolean) => void;
}

export function LegalTermsConsent({ isAccepted, onAcceptedChange }: LegalTermsConsentProps) {
  const router = useRouter();
  return (
    <Checkbox
      label={translate("legal.consent.label")}
      isChecked={isAccepted}
      onCheckedChange={onAcceptedChange}
      isRequired
    >
      <View>
        <TextLink
          label={translate("legal.consent.readTerms")}
          accessibilityRole="link"
          onPress={() => router.push(routes.legalDocument("terms"))}
        />
        <TextLink
          label={translate("legal.consent.readPrivacy")}
          accessibilityRole="link"
          onPress={() => router.push(routes.legalDocument("privacy"))}
        />
      </View>
    </Checkbox>
  );
}
