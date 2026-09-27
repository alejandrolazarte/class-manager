import { View } from "react-native";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { StudentSummary } from "@/features/students/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Card } from "@/ui/Card";
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
    <Card
      onPress={() => onPress(student)}
      accessibilityLabel={student.fullName}
      className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-3"
    >
      <Avatar name={student.fullName} />
      <View className="min-w-0 flex-1 gap-0.5">
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
      </View>
      <Icon name="next" tone="subtle-foreground" />
    </Card>
  );
}
