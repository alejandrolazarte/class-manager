import { forwardRef } from "react";
import { Text, TextInput, TextInputProps, View } from "react-native";

interface TextFieldProps extends TextInputProps {
  label: string;
  errorMessage?: string;
}

export const TextField = forwardRef<TextInput, TextFieldProps>(function TextField(
  { label, errorMessage, multiline, ...textInputProps },
  forwardedRef,
) {
  const hasError = errorMessage !== undefined;
  return (
    <View className="gap-1">
      <Text className="text-sm font-medium text-gray-700">{label}</Text>
      <TextInput
        ref={forwardedRef}
        accessibilityLabel={label}
        multiline={multiline}
        className={`rounded-xl border bg-white px-3 py-3 text-base ${hasError ? "border-red-500" : "border-gray-300"} ${multiline ? "min-h-24" : ""}`}
        textAlignVertical={multiline ? "top" : "center"}
        {...textInputProps}
      />
      {hasError ? (
        <Text accessibilityRole="alert" className="text-sm text-red-600">
          {errorMessage}
        </Text>
      ) : null}
    </View>
  );
});
