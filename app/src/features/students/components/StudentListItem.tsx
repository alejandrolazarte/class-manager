import { Pressable } from "react-native";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { StudentSummary } from "@/features/students/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

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
      className="gap-1 border-b border-border-subtle bg-surface px-4 py-3"
    >
      <AppText variant="bodyStrong">
        {ageLabel ? `${student.fullName} · ${ageLabel}` : student.fullName}
      </AppText>
      {isOwnClient ? null : (
        <AppText variant="caption" tone="muted">
          {translate("students.list.responsible", { name: student.clientFullName })}
        </AppText>
      )}
      {student.notes ? (
        <AppText variant="caption" tone="subtle" numberOfLines={1}>
          {student.notes}
        </AppText>
      ) : null}
    </Pressable>
  );
}
