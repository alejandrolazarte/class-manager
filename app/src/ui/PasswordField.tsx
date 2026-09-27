import { useState } from "react";
import { Pressable, TextInputProps, View } from "react-native";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
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
        <AppText variant="label" tone="primary">
          {translate(isPasswordVisible ? "common.hidePassword" : "common.showPassword")}
        </AppText>
      </Pressable>
    </View>
  );
}
