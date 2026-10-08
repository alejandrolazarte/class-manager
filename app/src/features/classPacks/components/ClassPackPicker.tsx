import { useState } from "react";
import { Pressable, View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassPack } from "@/features/classPacks/types";
import { formatMoney } from "@/features/fees/money";
import { searchableText } from "@/forms/searchableText";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { SearchPage } from "@/ui/SearchPage";

interface ClassPackPickerProps {
  classPacks: ClassPack[];
  onPick: (classPack: ClassPack) => void;
  onClose: () => void;
  testID?: string;
}

export function classPackSummaryLine(classPack: ClassPack, currencyCode: string): string {
  return translateCount("classPacks.list.summary", classPack.classCount, {
    price: formatMoney(classPack.price, currencyCode),
  });
}

function matchesSearch(classPack: ClassPack, searchText: string): boolean {
  const normalizedSearch = searchableText(searchText);
  return normalizedSearch.length === 0 || searchableText(classPack.name).includes(normalizedSearch);
}

export function ClassPackPicker({
  classPacks,
  onPick,
  onClose,
  testID = "class-pack-picker",
}: ClassPackPickerProps) {
  const currencyCode = useBusinessCurrency();
  const [searchText, setSearchText] = useState("");
  const trimmedSearch = searchText.trim();
  const hasSearch = trimmedSearch.length > 0;
  const shownClassPacks = classPacks.filter((classPack) => matchesSearch(classPack, searchText));

  return (
    <SearchPage
      testID={testID}
      searchText={searchText}
      onSearchTextChange={setSearchText}
      searchPlaceholder={translate("classPacks.picker.search")}
      onClose={onClose}
    >
      <AppText variant="overline" tone="subtle">
        {translate(hasSearch ? "classPacks.picker.results" : "classPacks.picker.chooseTitle")}
      </AppText>
      {hasSearch && shownClassPacks.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("classPacks.picker.noResults", { search: trimmedSearch })}
        </AppText>
      ) : null}
      <View>
        {shownClassPacks.map((classPack) => (
          <Pressable
            key={classPack.id}
            accessibilityRole="button"
            accessibilityLabel={classPack.name}
            onPress={() => onPick(classPack)}
            className="flex-row items-center gap-3 rounded-2xl py-2.5 active:bg-muted"
          >
            <Avatar name={classPack.name} size="small" />
            <View className="min-w-0 flex-1">
              <AppText variant="bodyStrong">{classPack.name}</AppText>
              <AppText variant="caption" tone="subtle">
                {classPackSummaryLine(classPack, currencyCode)}
              </AppText>
            </View>
          </Pressable>
        ))}
      </View>
    </SearchPage>
  );
}
