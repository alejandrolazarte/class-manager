import { TextInput } from "react-native";

interface ClientSearchInputProps {
  value: string;
  onChangeText: (searchText: string) => void;
  placeholder: string;
  autoFocus?: boolean;
}

export function ClientSearchInput({
  value,
  onChangeText,
  placeholder,
  autoFocus = false,
}: ClientSearchInputProps) {
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
      className="rounded-xl border border-gray-300 bg-white px-3 py-3 text-base"
    />
  );
}
