import { useState } from "react";
import { Pressable, View } from "react-native";
import { LessonStudent, privateLessonLimits } from "@/features/privateLessons/privateLessonSchema";
import { useStudentSearch } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Card } from "@/ui/Card";
import { IconButton } from "@/ui/IconButton";
import { SearchInput } from "@/ui/SearchInput";

interface LessonStudentPickerProps {
  students: LessonStudent[];
  onChange: (students: LessonStudent[]) => void;
  errorMessage?: string;
  isReadOnly?: boolean;
}

const maximumSuggestions = 5;

export function LessonStudentPicker({
  students,
  onChange,
  errorMessage,
  isReadOnly = false,
}: LessonStudentPickerProps) {
  const [searchText, setSearchText] = useState("");
  const { data: foundStudents = [], debouncedSearch } = useStudentSearch(searchText);
  const selectedIds = new Set(students.map((student) => student.id));
  const canAddMore = !isReadOnly && students.length < privateLessonLimits.maximumStudents;
  const suggestions =
    canAddMore && debouncedSearch.length > 0
      ? foundStudents.filter((student) => !selectedIds.has(student.id)).slice(0, maximumSuggestions)
      : [];

  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {translate("privateLessons.form.students")}
      </AppText>
      {students.map((student) => (
        <View
          key={student.id}
          className="flex-row items-center gap-3 rounded-2xl border-[1.5px] border-border bg-surface py-2 pl-3 pr-1"
        >
          <Avatar name={student.fullName} size="small" />
          <AppText variant="bodyStrong" className="min-w-0 flex-1">
            {student.fullName}
          </AppText>
          {isReadOnly ? null : (
            <IconButton
              icon="close"
              tone="subtle-foreground"
              accessibilityLabel={translate("privateLessons.form.removeStudent", {
                name: student.fullName,
              })}
              onPress={() => onChange(students.filter((selected) => selected.id !== student.id))}
            />
          )}
        </View>
      ))}
      {canAddMore ? (
        <SearchInput
          value={searchText}
          onChangeText={setSearchText}
          placeholder={translate("privateLessons.form.searchStudent")}
        />
      ) : null}
      {suggestions.length > 0 ? (
        <Card>
          {suggestions.map((student) => (
            <Pressable
              key={student.id}
              accessibilityRole="button"
              accessibilityLabel={student.fullName}
              onPress={() => {
                onChange([...students, { id: student.id, fullName: student.fullName }]);
                setSearchText("");
              }}
              className="flex-row items-center gap-3 border-b border-border px-4 py-3 active:bg-muted"
            >
              <Avatar name={student.fullName} size="small" tone="muted" />
              <View className="min-w-0 flex-1 gap-0.5">
                <AppText variant="bodyStrong">{student.fullName}</AppText>
                <AppText variant="caption" tone="subtle">
                  {translate("students.list.responsible", { name: student.clientFullName })}
                </AppText>
              </View>
            </Pressable>
          ))}
        </Card>
      ) : null}
      {errorMessage ? (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      ) : null}
    </View>
  );
}
