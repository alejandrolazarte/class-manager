import { View } from "react-native";
import { ImportModule, ImportSchema } from "@/features/importExport/types";
import { translate } from "@/i18n/translate";
import { AccentNote } from "@/ui/AccentNote";
import { AppText } from "@/ui/AppText";
import { ListDivider } from "@/ui/ListRow";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";

interface ImportColumnsSectionProps {
  module: ImportModule;
  schema: ImportSchema | undefined;
}

export function ImportColumnsSection({ module, schema }: ImportColumnsSectionProps) {
  return (
    <View className="gap-3">
      {schema === undefined ? (
        <Spinner />
      ) : (
        <View>
          {schema.columns.map((column, index) => (
            <View key={column.key}>
              {index > 0 ? <ListDivider /> : null}
              <View className="gap-0.5 py-2.5">
                <View className="flex-row flex-wrap items-center gap-2">
                  <AppText variant="bodyStrong">{column.header}</AppText>
                  {column.isRequired ? (
                    <StatusPill
                      tone="primary"
                      isSmall
                      label={translate("importExport.column.required")}
                    />
                  ) : null}
                </View>
                {column.example ? (
                  <AppText variant="caption" tone="muted">
                    {translate("importExport.column.example", { example: column.example })}
                  </AppText>
                ) : null}
              </View>
            </View>
          ))}
        </View>
      )}
      {module === "students" ? (
        <AccentNote tone="primary">
          <AppText variant="caption" tone="muted">
            {translate("importExport.studentsHint")}
          </AppText>
        </AccentNote>
      ) : null}
    </View>
  );
}
