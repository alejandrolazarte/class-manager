import { useState } from "react";
import { Pressable, Text, TextInputProps, View } from "react-native";
import { translate } from "@/i18n/translate";
import { TextField } from "@/ui/TextField";

interface PasswordFieldProps extends Omit<TextInputProps, "secureTextEntry"> {
  label: string;
  errorMessage?: string;
}

export function PasswordField(passwordFieldProps: PasswordFieldProps) {
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);
  return (
    <View className="gap-1">
      <TextField
        autoCapitalize="none"
        autoCorrect={false}
        {...passwordFieldProps}
        secureTextEntry={!isPasswordVisible}
      />
      <Pressable
        accessibilityRole="button"
        className="self-end"
        onPress={() => setIsPasswordVisible((wasPasswordVisible) => !wasPasswordVisible)}
      >
        <Text className="text-sm font-medium text-brand">
          {translate(isPasswordVisible ? "common.hidePassword" : "common.showPassword")}
        </Text>
      </Pressable>
    </View>
  );
}
