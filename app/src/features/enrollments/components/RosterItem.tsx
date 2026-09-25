import { Pressable, Text, View } from "react-native";
import { RosterEntry } from "@/features/enrollments/types";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { studentAgeLabel } from "@/features/students/studentAgeLabel";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";

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
    <View className="flex-row items-center gap-3 border-b border-gray-100 bg-white px-4 py-3">
      <View className="flex-1 gap-1">
        <Text className="text-base font-semibold text-gray-900">
          {ageLabel ? `${entry.studentFullName} · ${ageLabel}` : entry.studentFullName}
        </Text>
        {isOwnClient ? null : (
          <Text className="text-sm text-gray-600">
            {translate("students.list.responsible", { name: entry.clientFullName })}
          </Text>
        )}
        {entry.startDate > today ? (
          <Text className="text-sm text-gray-500">
            {translate("enrollments.detail.startsOn", {
              date: formatBirthDateForDisplay(entry.startDate),
            })}
          </Text>
        ) : null}
      </View>
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={translate("enrollments.detail.unenrollStudent", {
          name: entry.studentFullName,
        })}
        onPress={() => onUnenroll(entry)}
      >
        <Text className="text-sm font-medium text-red-600">
          {translate("enrollments.detail.unenroll")}
        </Text>
      </Pressable>
    </View>
  );
}
