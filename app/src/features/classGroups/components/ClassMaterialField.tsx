import { Controller, UseFormReturn } from "react-hook-form";
import { Linking, Pressable, View } from "react-native";
import { ClassGroupFormValues } from "@/features/classGroups/classGroupSchema";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { TextField } from "@/ui/TextField";

const bytesPerMegabyte = 1024 * 1024;

export interface ShownMaterialFile {
  name: string;
  sizeInBytes: number | undefined;
  url: string | null;
}

interface ClassMaterialFieldProps {
  form: UseFormReturn<ClassGroupFormValues>;
  materialFile: ShownMaterialFile | null;
  errorMessage: string | null;
  onPickFile: () => void;
  onRemoveFile: () => void;
}

function formatMegabytes(sizeInBytes: number): string {
  return translate("classGroups.form.materialFileSize", {
    megabytes: (sizeInBytes / bytesPerMegabyte).toFixed(1).replace(".", ","),
  });
}

function MaterialFileRow({
  materialFile,
  onRemoveFile,
}: {
  materialFile: ShownMaterialFile;
  onRemoveFile: () => void;
}) {
  const { url } = materialFile;
  return (
    <Card className="flex-row items-center gap-3 px-4 py-3">
      <Icon name="material" tone="primary" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong" numberOfLines={1}>
          {materialFile.name}
        </AppText>
        {materialFile.sizeInBytes === undefined ? null : (
          <AppText variant="caption" tone="muted">
            {formatMegabytes(materialFile.sizeInBytes)}
          </AppText>
        )}
        {url === null ? null : (
          <Pressable
            accessibilityRole="link"
            onPress={() => Linking.openURL(url)}
            className="self-start active:opacity-70"
          >
            <AppText variant="link" tone="primary">
              {translate("classGroups.form.openMaterialFile")}
            </AppText>
          </Pressable>
        )}
      </View>
      <Button
        variant="dangerOutline"
        size="medium"
        label={translate("classGroups.form.removeMaterialFile")}
        onPress={onRemoveFile}
      />
    </Card>
  );
}

export function ClassMaterialField({
  form,
  materialFile,
  errorMessage,
  onPickFile,
  onRemoveFile,
}: ClassMaterialFieldProps) {
  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {translate("classGroups.form.material")}
      </AppText>
      {materialFile === null ? (
        <>
          <Controller
            control={form.control}
            name="materialUrl"
            render={({ field, fieldState }) => (
              <TextField
                label={translate("classGroups.form.materialUrl")}
                placeholder={translate("classGroups.form.materialUrlPlaceholder")}
                keyboardType="url"
                autoCapitalize="none"
                autoCorrect={false}
                value={field.value}
                onChangeText={field.onChange}
                onBlur={field.onBlur}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
          <Button
            variant="secondary"
            size="medium"
            label={translate("classGroups.form.uploadMaterialFile")}
            onPress={onPickFile}
          />
          <AppText variant="caption" tone="muted">
            {translate("classGroups.form.materialHint")}
          </AppText>
        </>
      ) : (
        <MaterialFileRow materialFile={materialFile} onRemoveFile={onRemoveFile} />
      )}
      {errorMessage === null ? null : (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      )}
    </View>
  );
}
