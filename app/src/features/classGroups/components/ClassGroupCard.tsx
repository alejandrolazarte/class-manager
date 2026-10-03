import { View } from "react-native";
import { ClassGroup } from "@/features/classGroups/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusPill } from "@/ui/StatusPill";
import { TitleWithPills } from "@/ui/TitleWithPills";

interface ClassGroupCardProps {
  classGroup: ClassGroup;
  onPress: (classGroup: ClassGroup) => void;
}

const detailSeparator = " · ";

export function ClassGroupCard({ classGroup, onPress }: ClassGroupCardProps) {
  const details = [classGroup.instructorFullName, classGroup.location].filter(
    (detail): detail is string => Boolean(detail),
  );
  return (
    <Card
      onPress={() => onPress(classGroup)}
      accessibilityLabel={`${classGroup.startTime} ${classGroup.name}`}
      className="gap-3 p-4"
    >
      <View className="flex-row items-start gap-3.5">
        <View className="h-[52px] w-[52px] items-center justify-center rounded-2xl bg-primary-soft">
          <AppText variant="bodyStrong" tone="primarySoft" className="font-heavy">
            {classGroup.startTime}
          </AppText>
          <AppText variant="footnote" tone="primarySoft" className="font-label">
            {translate("classGroups.card.duration", { minutes: classGroup.durationMinutes })}
          </AppText>
        </View>
        <View className="min-w-0 flex-1 gap-[3px]">
          <TitleWithPills title={classGroup.name} testID="class-group-name">
            {classGroup.enrolledCount >= classGroup.capacity ? (
              <StatusPill label={translate("classGroups.card.full")} tone="warning" isSmall />
            ) : null}
          </TitleWithPills>
          <AppText variant="caption" tone="muted">
            {details.join(detailSeparator)}
          </AppText>
        </View>
      </View>
      <View className="flex-row items-center gap-2.5">
        <ProgressBar
          isThin
          ratio={classGroup.capacity > 0 ? classGroup.enrolledCount / classGroup.capacity : 0}
        />
        <AppText variant="badge" tone="muted">
          {translate("classGroups.card.enrolled", {
            enrolled: classGroup.enrolledCount,
            capacity: classGroup.capacity,
          })}
        </AppText>
      </View>
    </Card>
  );
}
