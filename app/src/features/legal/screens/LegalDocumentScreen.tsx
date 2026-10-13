import { useLocalSearchParams, useRouter } from "expo-router";
import { View } from "react-native";
import {
  LegalDocumentKind,
  legalDocumentKinds,
  legalDocuments,
} from "@/features/legal/legalDocuments";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";

function isLegalDocumentKind(kind: string | undefined): kind is LegalDocumentKind {
  return legalDocumentKinds.some((documentKind) => documentKind === kind);
}

export function LegalDocumentScreen() {
  const router = useRouter();
  const { kind } = useLocalSearchParams<{ kind?: string }>();
  const document = legalDocuments[isLegalDocumentKind(kind) ? kind : "notice"];
  const goBack = () => (router.canGoBack() ? router.back() : router.replace(routes.appEntry));

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          onNavigate={goBack}
          title={translate(document.titleKey)}
          caption={translate("legal.updatedOn", { date: document.updatedOn })}
        />
      }
    >
      {document.sections.map((section) => (
        <View key={section.heading} className="gap-2">
          <SectionTitle title={section.heading} />
          {section.paragraphs.map((paragraph) => (
            <AppText key={paragraph} variant="body" tone="muted">
              {paragraph}
            </AppText>
          ))}
        </View>
      ))}
    </ScrollScreen>
  );
}
