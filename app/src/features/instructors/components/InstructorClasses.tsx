import { View } from "react-native";
import { useActiveClassGroups } from "@/features/classGroups/useClassGroups";
import { summarizeSchedule } from "@/features/enrollments/weekdaySummary";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BulletRow } from "@/ui/BulletRow";
import { SectionTitle } from "@/ui/SectionTitle";

interface InstructorClassesProps {
  instructorId: string;
}

export function InstructorClasses({ instructorId }: InstructorClassesProps) {
  const { data: classGroups } = useActiveClassGroups();
  if (classGroups === undefined) {
    return null;
  }
  const ownClassGroups = classGroups.filter(
    (classGroup) => classGroup.instructorId === instructorId,
  );
  return (
    <View className="gap-2">
      <SectionTitle title={translate("instructors.detail.classes")} isOverline />
      {ownClassGroups.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("instructors.detail.noClasses")}
        </AppText>
      ) : (
        <View className="gap-1">
          {ownClassGroups.map((classGroup) => (
            <BulletRow
              key={classGroup.id}
              label={classGroup.name}
              detail={summarizeSchedule(classGroup.weekdays, classGroup.startTime)}
            />
          ))}
        </View>
      )}
    </View>
  );
}
