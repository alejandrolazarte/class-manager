import { forwardRef } from "react";
import { TextInput, TextInputProps, View } from "react-native";
import { useTheme } from "@/theme/useTheme";
import { AppText, textVariantClassNames } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

export type TextFieldSurface = "surface" | "background";

interface TextFieldProps extends TextInputProps {
  label: string;
  errorMessage?: string;
  hint?: string;
  isHintSatisfied?: boolean;
  prefix?: string;
  fieldSurface?: TextFieldSurface;
  isRequired?: boolean;
}

const fieldSurfaceClassNames: Record<TextFieldSurface, string> = {
  surface: "bg-surface",
  background: "bg-background",
};

export const TextField = forwardRef<TextInput, TextFieldProps>(function TextField(
  {
    label,
    errorMessage,
    hint,
    isHintSatisfied = false,
    prefix,
    fieldSurface = "surface",
    isRequired = false,
    multiline,
    ...textInputProps
  },
  forwardedRef,
) {
  const { colors } = useTheme();
  const hasError = errorMessage !== undefined;
  const isLocked = textInputProps.editable === false;
  const surfaceClassName = isLocked
    ? "bg-muted text-muted-foreground"
    : `text-foreground ${fieldSurfaceClassNames[fieldSurface]}`;
  return (
    <View className="gap-1.5">
      <AppText variant="label" tone="muted">
        {withRequiredMark(label, isRequired)}
      </AppText>
      <View className="flex-row gap-2">
        {prefix ? (
          <View className="h-[52px] justify-center rounded-2xl border-[1.5px] border-border bg-muted px-3">
            <AppText variant="bodyStrong" tone="muted">
              {prefix}
            </AppText>
          </View>
        ) : null}
        <TextInput
          ref={forwardedRef}
          accessibilityLabel={label}
          multiline={multiline}
          placeholderTextColor={colors["subtle-foreground"]}
          className={`min-w-0 flex-1 rounded-2xl border-[1.5px] px-3.5 ${textVariantClassNames.input} ${surfaceClassName} ${hasError ? "border-danger" : "border-border focus:border-primary"} ${multiline ? "min-h-24 py-3" : "h-[52px]"}`}
          textAlignVertical={multiline ? "top" : "center"}
          {...textInputProps}
        />
      </View>
      {hasError ? (
        <View className="flex-row items-center gap-1">
          <Icon name="fieldError" size="small" tone="danger" />
          <AppText variant="label" tone="danger" accessibilityRole="alert" className="flex-1">
            {errorMessage}
          </AppText>
        </View>
      ) : hint ? (
        <View className="flex-row items-center gap-1">
          {isHintSatisfied ? <Icon name="fieldValid" size="small" tone="success" /> : null}
          <AppText
            variant="caption"
            tone={isHintSatisfied ? "success" : "subtle"}
            className="flex-1"
          >
            {hint}
          </AppText>
        </View>
      ) : null}
    </View>
  );
});
