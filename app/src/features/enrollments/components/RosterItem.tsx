import { View } from "react-native";
import { RosterEntry } from "@/features/enrollments/types";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { IconButton } from "@/ui/IconButton";

interface RosterItemProps {
  entry: RosterEntry;
  today: string;
  onUnenroll?: (entry: RosterEntry) => void;
}

const detailSeparator = " · ";

export function RosterItem({ entry, today, onUnenroll }: RosterItemProps) {
  const ageLabel = studentAgeLabel(entry.birthDate);
  const isOwnClient =
    normalizeStudentName(entry.studentFullName) === normalizeStudentName(entry.clientFullName);
  const details = [
    ageLabel,
    isOwnClient ? null : translate("students.list.responsible", { name: entry.clientFullName }),
  ].filter((detail): detail is string => Boolean(detail));
  return (
    <View className="flex-row items-center gap-3 border-b border-border py-3 pl-4 pr-2">
      <Avatar name={entry.studentFullName} size="small" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{entry.studentFullName}</AppText>
        {details.length > 0 ? (
          <AppText variant="caption" tone="subtle">
            {details.join(detailSeparator)}
          </AppText>
        ) : null}
        {entry.startDate > today ? (
          <AppText variant="caption" tone="primary">
            {translate("enrollments.detail.startsOn", {
              date: formatBirthDateForDisplay(entry.startDate),
            })}
          </AppText>
        ) : null}
      </View>
      {onUnenroll ? (
        <IconButton
          icon="unenroll"
          tone="subtle-foreground"
          accessibilityLabel={translate("enrollments.detail.unenrollStudent", {
            name: entry.studentFullName,
          })}
          onPress={() => onUnenroll(entry)}
        />
      ) : null}
    </View>
  );
}
