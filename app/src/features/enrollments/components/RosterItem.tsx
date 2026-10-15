import { View } from "react-native";
import { RosterEntry } from "@/features/enrollments/types";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";

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
    <HairlineRow className="flex-row items-center gap-3 py-2.5">
      <Avatar name={entry.studentFullName} size="small" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{entry.studentFullName}</AppText>
        {details.length > 0 ? (
          <AppText variant="footnote" tone="subtle">
            {details.join(detailSeparator)}
          </AppText>
        ) : null}
        {entry.startDate > today ? (
          <AppText variant="badge" tone="primary">
            {translate("enrollments.detail.startsOn", {
              date: formatBirthDateForDisplay(entry.startDate),
            })}
          </AppText>
        ) : null}
        {entry.endDate !== null ? (
          <AppText variant="badge" tone="warning">
            {entry.endDate === today
              ? translate("enrollments.detail.lastDayToday")
              : translate("enrollments.detail.lastDay", {
                  date: formatBirthDateForDisplay(entry.endDate),
                })}
          </AppText>
        ) : null}
      </View>
      {onUnenroll && entry.endDate === null ? (
        <Button
          variant="dangerGhost"
          size="small"
          label={translate("enrollments.detail.confirmUnenroll")}
          accessibilityLabel={translate("enrollments.detail.unenrollStudent", {
            name: entry.studentFullName,
          })}
          onPress={() => onUnenroll(entry)}
        />
      ) : null}
    </HairlineRow>
  );
}
