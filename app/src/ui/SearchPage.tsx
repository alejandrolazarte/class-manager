import { PropsWithChildren, ReactNode } from "react";
import { Modal, ScrollView, View } from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { ThemeScope } from "@/theme/ThemeScope";
import { IconButton } from "@/ui/IconButton";
import { SearchInput } from "@/ui/SearchInput";

interface SearchPageProps extends PropsWithChildren {
  searchText: string;
  onSearchTextChange: (searchText: string) => void;
  searchPlaceholder: string;
  onClose: () => void;
  filters?: ReactNode;
  footer?: ReactNode;
  testID?: string;
}

export function SearchPage({
  searchText,
  onSearchTextChange,
  searchPlaceholder,
  onClose,
  filters,
  footer,
  testID,
  children,
}: SearchPageProps) {
  return (
    <Modal visible animationType="slide" onRequestClose={onClose}>
      <ThemeScope className="flex-1 bg-background">
        <SafeAreaView
          edges={["top", "bottom"]}
          className="w-full max-w-2xl flex-1 self-center"
          testID={testID}
        >
          <View className="flex-row items-center gap-1 py-2 pl-2 pr-5">
            <IconButton
              icon="back"
              accessibilityLabel={translate("common.back")}
              onPress={onClose}
            />
            <View className="flex-1">
              <SearchInput
                value={searchText}
                onChangeText={onSearchTextChange}
                placeholder={searchPlaceholder}
              />
            </View>
          </View>
          {filters ? <View className="border-b border-border px-5 pb-3">{filters}</View> : null}
          <ScrollView
            className="flex-1"
            contentContainerClassName="gap-2.5 px-5 pb-6 pt-3"
            keyboardShouldPersistTaps="handled"
          >
            {children}
          </ScrollView>
          {footer}
        </SafeAreaView>
      </ThemeScope>
    </Modal>
  );
}
