import { Pressable, Text } from "react-native";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { StudentSummary } from "@/features/students/types";
import { translate } from "@/i18n/translate";

interface StudentListItemProps {
  student: StudentSummary;
  onPress: (student: StudentSummary) => void;
}

export function StudentListItem({ student, onPress }: StudentListItemProps) {
  const ageLabel = studentAgeLabel(student.birthDate);
  const isOwnClient =
    normalizeStudentName(student.fullName) === normalizeStudentName(student.clientFullName);
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={student.fullName}
      onPress={() => onPress(student)}
      className="gap-1 border-b border-gray-100 bg-white px-4 py-3"
    >
      <Text className="text-base font-semibold text-gray-900">
        {ageLabel ? `${student.fullName} · ${ageLabel}` : student.fullName}
      </Text>
      {isOwnClient ? null : (
        <Text className="text-sm text-gray-600">
          {translate("students.list.responsible", { name: student.clientFullName })}
        </Text>
      )}
      {student.notes ? (
        <Text numberOfLines={1} className="text-sm text-gray-500">
          {student.notes}
        </Text>
      ) : null}
    </Pressable>
  );
}
