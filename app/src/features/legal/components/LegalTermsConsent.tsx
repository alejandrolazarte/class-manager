import { useRouter } from "expo-router";
import { Fragment } from "react";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Checkbox } from "@/ui/Checkbox";
import { InlineLink } from "@/ui/InlineLink";

interface LegalTermsConsentProps {
  isAccepted: boolean;
  onAcceptedChange: (isAccepted: boolean) => void;
}

const linkPlaceholderPattern = /(\{terms\}|\{privacy\})/;

export function LegalTermsConsent({ isAccepted, onAcceptedChange }: LegalTermsConsentProps) {
  const router = useRouter();
  const links: Record<string, { label: string; kind: "terms" | "privacy" }> = {
    "{terms}": { label: translate("legal.consent.terms"), kind: "terms" },
    "{privacy}": { label: translate("legal.consent.privacy"), kind: "privacy" },
  };
  const labelParts = translate("legal.consent.template").split(linkPlaceholderPattern);

  return (
    <Checkbox
      label={translate("legal.consent.label")}
      isChecked={isAccepted}
      onCheckedChange={onAcceptedChange}
      isRequired
      labelContent={labelParts.map((part, index) => {
        const link = links[part];
        return link ? (
          <InlineLink
            key={part}
            label={link.label}
            onPress={() => router.push(routes.legalDocument(link.kind))}
          />
        ) : (
          <Fragment key={index}>{part}</Fragment>
        );
      })}
    />
  );
}
