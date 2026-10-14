import { useRouter } from "expo-router";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { FooterLinks } from "@/ui/FooterLinks";

export function LegalLinks() {
  const router = useRouter();
  return (
    <FooterLinks
      links={[
        {
          label: translate("legal.terms.title"),
          onPress: () => router.push(routes.legalDocument("terms")),
        },
        {
          label: translate("legal.privacy.title"),
          onPress: () => router.push(routes.legalDocument("privacy")),
        },
        {
          label: translate("legal.notice.title"),
          onPress: () => router.push(routes.legalDocument("notice")),
        },
      ]}
    />
  );
}
