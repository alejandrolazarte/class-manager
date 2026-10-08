import { useState } from "react";
import { View } from "react-native";
import { LessonStudent, privateLessonLimits } from "@/features/privateLessons/privateLessonSchema";
import { StudentPicker } from "@/features/students/components/StudentPicker";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { IconButton } from "@/ui/IconButton";
import { PickerChooseButton } from "@/ui/PickerField";
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
        <View
          key={student.id}
          className="flex-row items-center gap-3 rounded-2xl border-[1.5px] border-primary bg-surface py-2 pl-3 pr-1"
        >
          <Avatar name={student.fullName} size="small" />
          <AppText variant="bodyStrong" className="min-w-0 flex-1">
            {student.fullName}
          </AppText>
          {isReadOnly ? null : (
            <IconButton
              icon="close"
              tone="muted-foreground"
              accessibilityLabel={translate("privateLessons.form.removeStudent", {
                name: student.fullName,
              })}
              onPress={() => onChange(students.filter((selected) => selected.id !== student.id))}
            />
          )}
        </View>
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
