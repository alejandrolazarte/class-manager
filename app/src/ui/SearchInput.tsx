import { TextInput } from "react-native";

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
