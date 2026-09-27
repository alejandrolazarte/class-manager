import { forwardRef } from "react";
import { TextInput, TextInputProps, View } from "react-native";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";

interface TextFieldProps extends TextInputProps {
  label: string;
  errorMessage?: string;
}

export const TextField = forwardRef<TextInput, TextFieldProps>(function TextField(
  { label, errorMessage, multiline, ...textInputProps },
  forwardedRef,
) {
  const { colors } = useTheme();
  const hasError = errorMessage !== undefined;
  return (
    <View className="gap-1">
      <AppText variant="label" tone="muted">
        {label}
      </AppText>
      <TextInput
        ref={forwardedRef}
        accessibilityLabel={label}
        multiline={multiline}
        placeholderTextColor={colors["subtle-foreground"]}
        className={`rounded-xl border bg-surface px-3 py-3 text-base text-foreground ${hasError ? "border-danger" : "border-border-strong"} ${multiline ? "min-h-24" : ""}`}
        textAlignVertical={multiline ? "top" : "center"}
        {...textInputProps}
      />
      {hasError ? (
        <AppText variant="caption" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      ) : null}
    </View>
  );
});
