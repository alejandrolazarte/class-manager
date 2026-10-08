import { useState } from "react";
import { Pressable, View } from "react-native";
import { ClassGroup } from "@/features/classGroups/types";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { searchableText } from "@/forms/searchableText";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { SearchPage } from "@/ui/SearchPage";

interface ClassGroupPickerProps {
  classGroups: ClassGroup[];
  onPick: (classGroup: ClassGroup) => void;
  onClose: () => void;
  excludedClassGroupIds?: string[];
  testID?: string;
}

export function classGroupScheduleLine(classGroup: ClassGroup): string {
  return translate("classGroups.picker.detail", {
    schedule: summarizeSchedule(classGroup.weekdays, classGroup.startTime),
    instructor: classGroup.instructorFullName,
  });
}

function matchesSearch(classGroup: ClassGroup, searchText: string): boolean {
  const normalizedSearch = searchableText(searchText);
  return (
    normalizedSearch.length === 0 ||
    searchableText(classGroup.name).includes(normalizedSearch) ||
    searchableText(classGroup.instructorFullName).includes(normalizedSearch)
  );
}

export function ClassGroupPicker({
  classGroups,
  onPick,
  onClose,
  excludedClassGroupIds = [],
  testID = "class-group-picker",
}: ClassGroupPickerProps) {
  const [searchText, setSearchText] = useState("");
  const trimmedSearch = searchText.trim();
  const hasSearch = trimmedSearch.length > 0;
  const shownClassGroups = classGroups.filter(
    (classGroup) =>
      !excludedClassGroupIds.includes(classGroup.id) && matchesSearch(classGroup, searchText),
  );

  return (
    <SearchPage
      testID={testID}
      searchText={searchText}
      onSearchTextChange={setSearchText}
      searchPlaceholder={translate("classGroups.picker.search")}
      onClose={onClose}
    >
      <AppText variant="overline" tone="subtle">
        {translate(hasSearch ? "classGroups.picker.results" : "classGroups.picker.chooseTitle")}
      </AppText>
      {shownClassGroups.length === 0 ? (
        <AppText variant="body" tone="muted">
          {hasSearch
            ? translate("classGroups.picker.noResults", { search: trimmedSearch })
            : translate("classGroups.picker.noClassGroups")}
        </AppText>
      ) : null}
      <View>
        {shownClassGroups.map((classGroup) => (
          <Pressable
            key={classGroup.id}
            accessibilityRole="button"
            accessibilityLabel={classGroup.name}
            onPress={() => onPick(classGroup)}
            className="flex-row items-center gap-3 rounded-2xl py-2.5 active:bg-muted"
          >
            <Avatar name={classGroup.name} size="small" />
            <View className="min-w-0 flex-1">
              <AppText variant="bodyStrong">{classGroup.name}</AppText>
              <AppText variant="caption" tone="subtle">
                {classGroupScheduleLine(classGroup)}
              </AppText>
            </View>
          </Pressable>
        ))}
      </View>
    </SearchPage>
  );
}
