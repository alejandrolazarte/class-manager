import { useState } from "react";
import { Pressable, View } from "react-native";
import { formatPhoneNumberForDisplay } from "@/features/clients/phoneNumberFormatting";
import { StudentSummary } from "@/features/students/types";
import { useStudentSearch } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { SearchPage } from "@/ui/SearchPage";
import { Spinner } from "@/ui/Spinner";

interface StudentPickerProps {
  onPick: (student: StudentSummary) => void;
  onClose: () => void;
  excludedStudentIds?: string[];
  testID?: string;
}

function normalizedName(fullName: string): string {
  return fullName.trim().toLocaleLowerCase();
}

export function studentContactLine(student: StudentSummary): string {
  const phone = formatPhoneNumberForDisplay(student.clientPhoneNumber);
  return normalizedName(student.fullName) === normalizedName(student.clientFullName)
    ? phone
    : translate("students.picker.studentOfClient", { client: student.clientFullName, phone });
}

export function StudentPicker({
  onPick,
  onClose,
  excludedStudentIds = [],
  testID = "student-picker",
}: StudentPickerProps) {
  const [searchText, setSearchText] = useState("");
  const {
    data: foundStudents = [],
    isPending,
    isError,
    debouncedSearch,
  } = useStudentSearch(searchText);
  const hasSearch = debouncedSearch.length > 0;
  const students = foundStudents.filter((student) => !excludedStudentIds.includes(student.id));

  return (
    <SearchPage
      testID={testID}
      searchText={searchText}
      onSearchTextChange={setSearchText}
      searchPlaceholder={translate("students.picker.search")}
      onClose={onClose}
    >
      <AppText variant="overline" tone="subtle">
        {translate(hasSearch ? "students.picker.results" : "students.picker.chooseTitle")}
      </AppText>
      {isPending ? <Spinner /> : null}
      {isError ? <Banner message={translate("common.unexpectedError")} /> : null}
      {!isPending && hasSearch && students.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("students.picker.noResults", { search: debouncedSearch })}
        </AppText>
      ) : null}
      <View>
        {students.map((student) => (
          <Pressable
            key={student.id}
            accessibilityRole="button"
            accessibilityLabel={student.fullName}
            onPress={() => onPick(student)}
            className="flex-row items-center gap-3 rounded-2xl py-2.5 active:bg-muted"
          >
            <Avatar name={student.fullName} size="small" />
            <View className="min-w-0 flex-1">
              <AppText variant="bodyStrong">{student.fullName}</AppText>
              <AppText variant="caption" tone="subtle">
                {studentContactLine(student)}
              </AppText>
            </View>
          </Pressable>
        ))}
      </View>
    </SearchPage>
  );
}
