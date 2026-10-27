import { useRouter } from "expo-router";
import { importModuleTexts } from "@/features/importExport/importModuleTexts";
import { ImportModule } from "@/features/importExport/types";
import { useDownloadExport } from "@/features/importExport/useImportExport";
import { usePlanAllowing } from "@/features/subscriptions/PlanOfferProvider";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useFeature } from "@/features/subscriptions/useSubscription";
import { translate } from "@/i18n/translate";
import { OverflowMenu } from "@/ui/OverflowMenu";
import { useToast } from "@/ui/ToastProvider";

interface ImportExportMenuProps {
  module: ImportModule;
  importRoute: string;
}

export function ImportExportMenu({ module, importRoute }: ImportExportMenuProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const texts = importModuleTexts[module];
  const hasImportExport = useFeature(featureCodes.importExport);
  const withinImportExportPlan = usePlanAllowing(featureCodes.importExport, hasImportExport);
  const downloadExport = useDownloadExport();

  return (
    <OverflowMenu
      accessibilityLabel={translate("importExport.moreOptions")}
      items={[
        {
          key: "import",
          label: translate(texts.importTitle),
          onPress: withinImportExportPlan(() => router.push(importRoute)),
        },
        {
          key: "export",
          label: translate(texts.exportLabel),
          onPress: withinImportExportPlan(() =>
            downloadExport.mutate(module, {
              onError: () => showToast(translate("importExport.downloadFailed")),
            }),
          ),
        },
      ]}
    />
  );
}
