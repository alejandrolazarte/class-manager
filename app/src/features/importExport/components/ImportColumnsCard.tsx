import { ImportModule, ImportSchema } from "@/features/importExport/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { SectionTitle } from "@/ui/SectionTitle";
import { Spinner } from "@/ui/Spinner";

const columnSeparator = " · ";

interface ImportColumnsCardProps {
  module: ImportModule;
  schema: ImportSchema | undefined;
}

export function ImportColumnsCard({ module, schema }: ImportColumnsCardProps) {
  return (
    <Card className="gap-2 p-4">
      <SectionTitle title={translate("importExport.columnsTitle")} isOverline />
      {schema === undefined ? (
        <Spinner />
      ) : (
        <AppText variant="bodyStrong">
          {schema.columns
            .map((column) =>
              column.isRequired
                ? translate("importExport.columnRequired", { column: column.header })
                : column.header,
            )
            .join(columnSeparator)}
        </AppText>
      )}
      <AppText variant="caption" tone="subtle">
        {translate("importExport.columnsHint")}
      </AppText>
      {module === "students" ? (
        <AppText variant="caption" tone="subtle">
          {translate("importExport.studentsHint")}
        </AppText>
      ) : null}
    </Card>
  );
}
