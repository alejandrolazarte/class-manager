import { View } from "react-native";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { StudentSummary } from "@/features/students/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";

interface StudentListItemProps {
  student: StudentSummary;
  onPress: (student: StudentSummary) => void;
}

export function StudentListItem({ student, onPress }: StudentListItemProps) {
  const ageLabel = studentAgeLabel(student.birthDate);
  const isOwnClient =
    normalizeStudentName(student.fullName) === normalizeStudentName(student.clientFullName);
  return (
    <HairlineRow onPress={() => onPress(student)} accessibilityLabel={student.fullName}>
      <Avatar name={student.fullName} size="small" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong" numberOfLines={1}>
          {ageLabel ? `${student.fullName} · ${ageLabel}` : student.fullName}
        </AppText>
        {isOwnClient ? null : (
          <AppText variant="caption" tone="muted">
            {translate("students.list.responsible", { name: student.clientFullName })}
          </AppText>
        )}
        {student.notes ? (
          <AppText variant="footnote" tone="subtle" numberOfLines={1}>
            {student.notes}
          </AppText>
        ) : null}
      </View>
      <Icon name="next" size="medium" tone="subtle-foreground" />
    </HairlineRow>
  );
}
