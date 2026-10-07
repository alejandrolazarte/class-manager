import { useState } from "react";
import { Pressable, View } from "react-native";
import { Instructor } from "@/features/instructors/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { SearchPage } from "@/ui/SearchPage";
import { Spinner } from "@/ui/Spinner";

interface SubstituteInstructorPickerProps {
  instructors: Instructor[];
  isPending: boolean;
  isError: boolean;
  onPick: (instructor: Instructor) => void;
  onClose: () => void;
}

const diacriticalMarks = /[̀-ͯ]/g;

function searchableText(text: string): string {
  return text.normalize("NFD").replace(diacriticalMarks, "").trim().toLocaleLowerCase();
}

function matchesSearch(instructor: Instructor, searchText: string): boolean {
  const normalizedSearch = searchableText(searchText);
  return (
    normalizedSearch.length === 0 ||
    searchableText(instructor.fullName).includes(normalizedSearch) ||
    searchableText(instructor.email ?? "").includes(normalizedSearch)
  );
}

export function SubstituteInstructorPicker({
  instructors,
  isPending,
  isError,
  onPick,
  onClose,
}: SubstituteInstructorPickerProps) {
  const [searchText, setSearchText] = useState("");
  const trimmedSearch = searchText.trim();
  const hasSearch = trimmedSearch.length > 0;
  const shownInstructors = instructors.filter((instructor) =>
    matchesSearch(instructor, searchText),
  );

  return (
    <SearchPage
      testID="substitute-instructor-picker"
      searchText={searchText}
      onSearchTextChange={setSearchText}
      searchPlaceholder={translate("sessions.substitute.search")}
      onClose={onClose}
    >
      <AppText variant="overline" tone="subtle">
        {translate(
          hasSearch ? "sessions.substitute.results" : "sessions.substitute.chooseInstructorTitle",
        )}
      </AppText>
      {isPending ? <Spinner /> : null}
      {isError ? <Banner message={translate("common.unexpectedError")} /> : null}
      {!isPending && !isError && shownInstructors.length === 0 ? (
        <AppText variant="body" tone="muted">
          {hasSearch
            ? translate("sessions.substitute.noResults", { search: trimmedSearch })
            : translate("sessions.substitute.noInstructors")}
        </AppText>
      ) : null}
      <View>
        {shownInstructors.map((instructor) => (
          <Pressable
            key={instructor.id}
            accessibilityRole="button"
            accessibilityLabel={instructor.fullName}
            onPress={() => onPick(instructor)}
            className="flex-row items-center gap-3 rounded-2xl py-2.5 active:bg-muted"
          >
            <Avatar name={instructor.fullName} size="small" />
            <View className="min-w-0 flex-1">
              <AppText variant="bodyStrong">{instructor.fullName}</AppText>
              {instructor.email ? (
                <AppText variant="caption" tone="subtle">
                  {instructor.email}
                </AppText>
              ) : null}
            </View>
          </Pressable>
        ))}
      </View>
    </SearchPage>
  );
}
