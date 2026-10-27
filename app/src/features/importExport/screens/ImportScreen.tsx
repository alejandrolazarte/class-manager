import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { ImportColumnsSection } from "@/features/importExport/components/ImportColumnsSection";
import { ImportPreviewSection } from "@/features/importExport/components/ImportPreviewSection";
import { importModuleTexts } from "@/features/importExport/importModuleTexts";
import { fileErrorMessage } from "@/features/importExport/importMessages";
import { PickedImportFile, pickImportFile } from "@/features/importExport/pickImportFile";
import { ImportModule, ImportReport } from "@/features/importExport/types";
import {
  useDownloadTemplate,
  useImportFile,
  useImportSchema,
  usePreviewImport,
} from "@/features/importExport/useImportExport";
import { usePlanAllowing } from "@/features/subscriptions/PlanOfferProvider";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useFeature } from "@/features/subscriptions/useSubscription";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { IconCircle } from "@/ui/IconCircle";
import { NumberedStep } from "@/ui/NumberedStep";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

interface ImportScreenProps {
  module: ImportModule;
}

interface PreviewedFile {
  file: PickedImportFile;
  report: ImportReport;
}

export function ImportScreen({ module }: ImportScreenProps) {
  const router = useRouter();
  const texts = importModuleTexts[module];
  const hasImportExport = useFeature(featureCodes.importExport);
  const withinImportExportPlan = usePlanAllowing(featureCodes.importExport, hasImportExport);
  const [previewedFile, setPreviewedFile] = useState<PreviewedFile | null>(null);
  const [importedRowCount, setImportedRowCount] = useState<number | null>(null);
  const schema = useImportSchema(module, { enabled: hasImportExport });
  const previewImport = usePreviewImport();
  const importFile = useImportFile();
  const downloadTemplate = useDownloadTemplate();
  const isChoosingFile = previewedFile === null && importedRowCount === null;

  function resetImport() {
    setPreviewedFile(null);
    setImportedRowCount(null);
    previewImport.reset();
    importFile.reset();
  }

  async function chooseFile() {
    const file = await pickImportFile();
    if (file === null) {
      return;
    }
    resetImport();
    previewImport.mutate(
      { module, file },
      { onSuccess: (report) => setPreviewedFile({ file, report }) },
    );
  }

  function confirmImport(file: PickedImportFile) {
    importFile.mutate(
      { module, file },
      {
        onSuccess: (report) => {
          setPreviewedFile(null);
          setImportedRowCount(report.summary.valid);
        },
      },
    );
  }

  const requestError = previewImport.error ?? importFile.error;

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          title={translate(texts.importTitle)}
          subtitle={isChoosingFile ? translate(texts.subtitle) : undefined}
        />
      }
    >
      {requestError !== null ? <Banner message={fileErrorMessage(requestError)} /> : null}
      {importedRowCount !== null ? (
        <View className="items-center gap-3 py-6">
          <IconCircle icon="checked" tone="success" size="large" />
          <AppText variant="headline" className="text-center">
            {translateCount("importExport.result.imported", importedRowCount)}
          </AppText>
          <View className="w-full gap-3 pt-3">
            <Button label={translate(texts.viewList)} onPress={() => router.back()} />
            <Button
              variant="secondary"
              size="medium"
              label={translate("importExport.result.importAnother")}
              onPress={resetImport}
            />
          </View>
        </View>
      ) : null}
      {previewedFile !== null ? (
        <ImportPreviewSection
          fileName={previewedFile.file.name}
          report={previewedFile.report}
          isImporting={importFile.isPending}
          onImport={() => confirmImport(previewedFile.file)}
          onChooseAnother={withinImportExportPlan(chooseFile)}
        />
      ) : null}
      {isChoosingFile ? (
        <View className="gap-7">
          <View className="gap-1.5">
            <NumberedStep number={1} title={translate("importExport.steps.template")} />
            <AppText variant="caption" tone="muted">
              {translate("importExport.steps.templateHint")}
            </AppText>
            {downloadTemplate.error !== null ? (
              <Banner message={translate("importExport.downloadFailed")} />
            ) : null}
            <View className="items-start pt-1">
              <Button
                variant="secondary"
                size="medium"
                icon="download"
                label={translate("importExport.downloadTemplate")}
                isLoading={downloadTemplate.isPending}
                onPress={withinImportExportPlan(() => downloadTemplate.mutate(module))}
              />
            </View>
          </View>
          <View className="gap-1.5">
            <NumberedStep number={2} title={translate(texts.fillStep)} />
            <AppText variant="caption" tone="muted">
              {translate("importExport.steps.fillHint")}
            </AppText>
            {hasImportExport ? <ImportColumnsSection module={module} schema={schema.data} /> : null}
          </View>
          <View className="gap-1.5">
            <NumberedStep number={3} title={translate("importExport.steps.upload")} />
            <AppText variant="caption" tone="muted">
              {translate("importExport.columnsHint")}
            </AppText>
            <View className="pt-1">
              <Button
                icon="upload"
                label={translate("importExport.chooseFile")}
                isLoading={previewImport.isPending}
                onPress={withinImportExportPlan(chooseFile)}
              />
            </View>
          </View>
        </View>
      ) : null}
    </ScrollScreen>
  );
}
