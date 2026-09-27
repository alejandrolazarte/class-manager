import { TextInput } from "react-native";
import { useTheme } from "@/theme/useTheme";

interface SearchInputProps {
  value: string;
  onChangeText: (searchText: string) => void;
  placeholder: string;
  autoFocus?: boolean;
}

export function SearchInput({
  value,
  onChangeText,
  placeholder,
  autoFocus = false,
}: SearchInputProps) {
  const { colors } = useTheme();
  return (
    <TextInput
      accessibilityLabel={placeholder}
      accessibilityRole="search"
      placeholder={placeholder}
      value={value}
      onChangeText={onChangeText}
      autoFocus={autoFocus}
      autoCorrect={false}
      clearButtonMode="while-editing"
      placeholderTextColor={colors["subtle-foreground"]}
      className="rounded-xl border border-border-strong bg-surface px-3 py-3 text-base text-foreground"
    />
  );
}
