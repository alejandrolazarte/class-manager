import { Controller, UseFormReturn } from "react-hook-form";
import { Linking, View } from "react-native";
import { ClassGroupFormValues } from "@/features/classGroups/classGroupSchema";
import { materialFileSizeLabel } from "@/features/classGroups/materialFileSize";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Icon } from "@/ui/Icon";
import { TextLink } from "@/ui/TextLink";
import { TextField } from "@/ui/TextField";
import { TonalBlock } from "@/ui/TonalBlock";

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

function MaterialFileRow({
  materialFile,
  onRemoveFile,
}: {
  materialFile: ShownMaterialFile;
  onRemoveFile: () => void;
}) {
  const { url } = materialFile;
  return (
    <TonalBlock tone="muted" isCompact>
      <View className="flex-row items-center gap-3">
        <Icon name="material" tone="primary" />
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong" numberOfLines={1}>
            {materialFile.name}
          </AppText>
          {materialFile.sizeInBytes === undefined ? null : (
            <AppText variant="caption" tone="muted">
              {materialFileSizeLabel(materialFile.sizeInBytes)}
            </AppText>
          )}
          {url === null ? null : (
            <TextLink
              accessibilityRole="link"
              label={translate("classGroups.form.openMaterialFile")}
              onPress={() => Linking.openURL(url)}
            />
          )}
        </View>
        <Button
          variant="dangerOutline"
          size="medium"
          label={translate("classGroups.form.removeMaterialFile")}
          onPress={onRemoveFile}
        />
      </View>
    </TonalBlock>
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
      {materialFile === null ? (
        <Button
          variant="secondary"
          size="medium"
          label={translate("classGroups.form.uploadMaterialFile")}
          onPress={onPickFile}
        />
      ) : (
        <MaterialFileRow materialFile={materialFile} onRemoveFile={onRemoveFile} />
      )}
      <AppText variant="caption" tone="muted">
        {translate("classGroups.form.materialHint")}
      </AppText>
      {errorMessage === null ? null : (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      )}
    </View>
  );
}
