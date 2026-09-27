import { View } from "react-native";
import { rowErrorMessage } from "@/features/importExport/importMessages";
import { ImportReport } from "@/features/importExport/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ListDivider } from "@/ui/ListRow";
import { SectionTitle } from "@/ui/SectionTitle";
import { StatusPill } from "@/ui/StatusPill";

const maximumProblemRowsShown = 50;
const ignoredColumnSeparator = ", ";

interface ImportPreviewCardProps {
  fileName: string;
  report: ImportReport;
  isImporting: boolean;
  onImport: () => void;
  onChooseAnother: () => void;
}

export function ImportPreviewCard({
  fileName,
  report,
  isImporting,
  onImport,
  onChooseAnother,
}: ImportPreviewCardProps) {
  const problemRows = report.rows.filter((row) => row.status !== "Valid");
  const shownProblemRows = problemRows.slice(0, maximumProblemRowsShown);
  const hiddenProblemRowCount = problemRows.length - shownProblemRows.length;
  const ignoredHeaders = report.columns
    .filter((column) => column.key === null)
    .map((column) => column.header);

  return (
    <Card className="gap-3 p-4">
      <AppText variant="caption" tone="subtle">
        {translate("importExport.preview.file", { fileName })}
      </AppText>
      <View className="flex-row flex-wrap gap-2">
        <StatusPill
          tone="success"
          label={translate("importExport.preview.valid", { count: report.summary.valid })}
        />
        <StatusPill
          tone="danger"
          label={translate("importExport.preview.errors", { count: report.summary.errors })}
        />
        <StatusPill
          tone="neutral"
          label={translate("importExport.preview.skipped", { count: report.summary.skipped })}
        />
      </View>
      {ignoredHeaders.length > 0 ? (
        <Banner
          tone="info"
          icon="info"
          message={translate("importExport.preview.ignoredColumns", {
            columns: ignoredHeaders.join(ignoredColumnSeparator),
          })}
        />
      ) : null}
      {shownProblemRows.length > 0 ? (
        <View className="gap-2">
          <SectionTitle title={translate("importExport.preview.problemsTitle")} isOverline />
          {shownProblemRows.map((row, index) => (
            <View key={row.line} className="gap-1">
              {index > 0 ? <ListDivider /> : null}
              <AppText variant="bodyStrong">
                {translate("importExport.preview.line", { line: row.line })}
              </AppText>
              {row.errors.map((error) => (
                <AppText
                  key={`${error.key}-${error.code}`}
                  variant="caption"
                  tone={row.status === "Error" ? "danger" : "subtle"}
                >
                  {rowErrorMessage(error, report.columns)}
                </AppText>
              ))}
            </View>
          ))}
          {hiddenProblemRowCount > 0 ? (
            <AppText variant="caption" tone="subtle">
              {translate("importExport.preview.moreRows", { count: hiddenProblemRowCount })}
            </AppText>
          ) : null}
        </View>
      ) : null}
      {report.summary.valid > 0 ? (
        <Button
          icon="upload"
          label={translateCount("importExport.preview.import", report.summary.valid)}
          isLoading={isImporting}
          onPress={onImport}
        />
      ) : (
        <AppText variant="body" tone="muted">
          {translate("importExport.preview.nothingToImport")}
        </AppText>
      )}
      <Button
        variant="ghost"
        size="medium"
        label={translate("importExport.preview.chooseAnother")}
        onPress={onChooseAnother}
      />
    </Card>
  );
}
