import { TextInput } from "react-native";

interface PasswordManagerUsernameProps {
  email: string;
}

export function PasswordManagerUsername({ email }: PasswordManagerUsernameProps) {
  return (
    <TextInput
      className="hidden"
      autoComplete="username"
      textContentType="username"
      value={email}
      accessibilityElementsHidden
      importantForAccessibility="no-hide-descendants"
    />
  );
}
