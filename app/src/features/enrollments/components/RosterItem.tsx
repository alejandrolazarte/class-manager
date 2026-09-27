import { Pressable, View } from "react-native";
import { RosterEntry } from "@/features/enrollments/types";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

interface RosterItemProps {
  entry: RosterEntry;
  today: string;
  onUnenroll: (entry: RosterEntry) => void;
}

export function RosterItem({ entry, today, onUnenroll }: RosterItemProps) {
  const ageLabel = studentAgeLabel(entry.birthDate);
  const isOwnClient =
    normalizeStudentName(entry.studentFullName) === normalizeStudentName(entry.clientFullName);
  return (
    <View className="flex-row items-center gap-3 border-b border-border-subtle bg-surface px-4 py-3">
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">
          {ageLabel ? `${entry.studentFullName} · ${ageLabel}` : entry.studentFullName}
        </AppText>
        {isOwnClient ? null : (
          <AppText variant="caption" tone="muted">
            {translate("students.list.responsible", { name: entry.clientFullName })}
          </AppText>
        )}
        {entry.startDate > today ? (
          <AppText variant="caption" tone="subtle">
            {translate("enrollments.detail.startsOn", {
              date: formatBirthDateForDisplay(entry.startDate),
            })}
          </AppText>
        ) : null}
      </View>
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={translate("enrollments.detail.unenrollStudent", {
          name: entry.studentFullName,
        })}
        onPress={() => onUnenroll(entry)}
      >
        <AppText variant="label" tone="danger">
          {translate("enrollments.detail.unenroll")}
        </AppText>
      </Pressable>
    </View>
  );
}
