import { TextInput, View } from "react-native";
import { useTheme } from "@/theme/useTheme";
import { textVariantClassNames } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";

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
    <View className="h-[42px] flex-row items-center gap-2 rounded-xl bg-muted px-3">
      <Icon name="search" size="medium" tone="subtle-foreground" />
      <TextInput
        accessibilityLabel={placeholder}
        accessibilityRole="search"
        placeholder={placeholder}
        value={value}
        onChangeText={onChangeText}
        autoFocus={autoFocus}
        autoCorrect={false}
        clearButtonMode="while-editing"
        placeholderTextColor={colors["muted-foreground"]}
        className={`h-full min-w-0 flex-1 ${textVariantClassNames.input} text-foreground`}
      />
    </View>
  );
}
