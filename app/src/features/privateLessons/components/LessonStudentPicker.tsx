import { useState } from "react";
import { View } from "react-native";
import { LessonStudent, privateLessonLimits } from "@/features/privateLessons/privateLessonSchema";
import { StudentPicker } from "@/features/students/components/StudentPicker";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { PickedItemRow, PickerChooseButton } from "@/ui/PickerField";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface LessonStudentPickerProps {
  students: LessonStudent[];
  onChange: (students: LessonStudent[]) => void;
  onStudentAdded?: (student: LessonStudent) => void;
  errorMessage?: string;
  isReadOnly?: boolean;
}

export function LessonStudentPicker({
  students,
  onChange,
  onStudentAdded,
  errorMessage,
  isReadOnly = false,
}: LessonStudentPickerProps) {
  const [isPickerOpen, setIsPickerOpen] = useState(false);
  const canAddMore = !isReadOnly && students.length < privateLessonLimits.maximumStudents;

  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {withRequiredMark(translate("privateLessons.form.students"))}
      </AppText>
      {students.map((student) => (
        <PickedItemRow
          key={student.id}
          name={student.fullName}
          removeLabel={translate("privateLessons.form.removeStudent", { name: student.fullName })}
          onRemove={
            isReadOnly
              ? undefined
              : () => onChange(students.filter((selected) => selected.id !== student.id))
          }
        />
      ))}
      {canAddMore ? (
        <PickerChooseButton
          label={translate(
            students.length === 0
              ? "privateLessons.form.chooseStudent"
              : "privateLessons.form.addStudent",
          )}
          onPress={() => setIsPickerOpen(true)}
        />
      ) : null}
      {errorMessage ? (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      ) : null}
      {isPickerOpen ? (
        <StudentPicker
          testID="private-lesson-student-picker"
          excludedStudentIds={students.map((student) => student.id)}
          onPick={(student) => {
            const addedStudent = {
              id: student.id,
              fullName: student.fullName,
              clientId: student.clientId,
            };
            onChange([...students, addedStudent]);
            onStudentAdded?.(addedStudent);
            setIsPickerOpen(false);
          }}
          onClose={() => setIsPickerOpen(false)}
        />
      ) : null}
    </View>
  );
}
