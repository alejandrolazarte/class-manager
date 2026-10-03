import { forwardRef } from "react";
import { TextInput, TextInputProps, View } from "react-native";
import { useTheme } from "@/theme/useTheme";
import { AppText, textVariantClassNames } from "@/ui/AppText";

type TextFieldSurface = "surface" | "background";

interface TextFieldProps extends TextInputProps {
  label: string;
  errorMessage?: string;
  hint?: string;
  isHintSatisfied?: boolean;
  prefix?: string;
  fieldSurface?: TextFieldSurface;
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
    multiline,
    ...textInputProps
  },
  forwardedRef,
) {
  const { colors } = useTheme();
  const hasError = errorMessage !== undefined;
  return (
    <View className="gap-1.5">
      <AppText variant="label" tone="muted">
        {label}
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
          className={`min-w-0 flex-1 rounded-2xl border-[1.5px] px-3.5 ${textVariantClassNames.input} text-foreground ${fieldSurfaceClassNames[fieldSurface]} ${hasError ? "border-danger" : "border-border focus:border-primary"} ${multiline ? "min-h-24 py-3" : "h-[52px]"}`}
          textAlignVertical={multiline ? "top" : "center"}
          {...textInputProps}
        />
      </View>
      {hasError ? (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      ) : hint ? (
        <AppText variant="caption" tone={isHintSatisfied ? "success" : "subtle"}>
          {hint}
        </AppText>
      ) : null}
    </View>
  );
});
