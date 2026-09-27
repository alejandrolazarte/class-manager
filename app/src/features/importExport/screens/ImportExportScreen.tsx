import { useState } from "react";
import { ImportColumnsCard } from "@/features/importExport/components/ImportColumnsCard";
import { ImportPreviewCard } from "@/features/importExport/components/ImportPreviewCard";
import { fileModuleName } from "@/features/importExport/spreadsheetFileNames";
import { fileErrorMessage } from "@/features/importExport/importMessages";
import { PickedImportFile, pickImportFile } from "@/features/importExport/pickImportFile";
import { ImportModule, ImportReport } from "@/features/importExport/types";
import {
  useDownloadExport,
  useDownloadTemplate,
  useImportFile,
  useImportSchema,
  usePreviewImport,
} from "@/features/importExport/useImportExport";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SegmentedControl, SegmentedOption } from "@/ui/SegmentedControl";

const moduleOptions: readonly SegmentedOption<ImportModule>[] = [
  { value: "students", label: translate("importExport.module.students") },
  { value: "instructors", label: translate("importExport.module.instructors") },
];

interface PreviewedFile {
  file: PickedImportFile;
  report: ImportReport;
}

export function ImportExportScreen() {
  const [module, setModule] = useState<ImportModule>("students");
  const [previewedFile, setPreviewedFile] = useState<PreviewedFile | null>(null);
  const [importedRowCount, setImportedRowCount] = useState<number | null>(null);
  const schema = useImportSchema(module);
  const previewImport = usePreviewImport();
  const importFile = useImportFile();
  const downloadTemplate = useDownloadTemplate();
  const downloadExport = useDownloadExport();

  function resetImport() {
    setPreviewedFile(null);
    setImportedRowCount(null);
    previewImport.reset();
    importFile.reset();
  }

  function changeModule(nextModule: ImportModule) {
    setModule(nextModule);
    resetImport();
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
  const downloadError = downloadTemplate.error ?? downloadExport.error;

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("importExport.title")} />}
    >
      <SegmentedControl options={moduleOptions} selectedValue={module} onChange={changeModule} />
      <ImportColumnsCard module={module} schema={schema.data} />

      <SectionTitle title={translate("importExport.importTitle")} />
      {importedRowCount !== null ? (
        <Banner
          tone="info"
          icon="paid"
          message={translateCount("importExport.result.imported", importedRowCount)}
        />
      ) : null}
      {requestError !== null ? <Banner message={fileErrorMessage(requestError)} /> : null}
      {previewedFile !== null ? (
        <ImportPreviewCard
          fileName={previewedFile.file.name}
          report={previewedFile.report}
          isImporting={importFile.isPending}
          onImport={() => confirmImport(previewedFile.file)}
          onChooseAnother={chooseFile}
        />
      ) : (
        <Card className="gap-3 p-4">
          <Button
            icon="upload"
            label={translate("importExport.chooseFile")}
            isLoading={previewImport.isPending}
            onPress={chooseFile}
          />
          <Button
            variant="secondary"
            size="medium"
            icon="download"
            label={translate("importExport.downloadTemplate")}
            isLoading={downloadTemplate.isPending}
            onPress={() => downloadTemplate.mutate(module)}
          />
        </Card>
      )}

      <SectionTitle title={translate("importExport.exportTitle")} />
      {downloadError !== null ? (
        <Banner message={translate("importExport.downloadFailed")} />
      ) : null}
      <Card className="gap-3 p-4">
        <AppText variant="caption" tone="subtle">
          {translate("importExport.exportHint")}
        </AppText>
        <Button
          variant="secondary"
          size="medium"
          icon="download"
          label={translate("importExport.export", { module: fileModuleName(module) })}
          isLoading={downloadExport.isPending}
          onPress={() => downloadExport.mutate(module)}
        />
      </Card>
    </ScrollScreen>
  );
}
