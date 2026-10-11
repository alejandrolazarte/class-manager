import { View } from "react-native";
import { ClassGroup } from "@/features/classGroups/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusPill } from "@/ui/StatusPill";
import { TitleWithPills } from "@/ui/TitleWithPills";

interface ClassGroupRowProps {
  classGroup: ClassGroup;
  onPress: (classGroup: ClassGroup) => void;
}

const detailSeparator = " · ";

export function ClassGroupRow({ classGroup, onPress }: ClassGroupRowProps) {
  const details = [classGroup.instructorFullName, classGroup.location].filter(
    (detail): detail is string => Boolean(detail),
  );
  const isFull = classGroup.enrolledCount >= classGroup.capacity;
  return (
    <HairlineRow
      onPress={() => onPress(classGroup)}
      accessibilityLabel={`${classGroup.startTime} ${classGroup.name}`}
      className="flex-row gap-3 py-3.5"
    >
      <View className="w-[52px]">
        <AppText variant="bodyStrong">{classGroup.startTime}</AppText>
        <AppText variant="footnote" tone="subtle">
          {translate("classGroups.card.duration", { minutes: classGroup.durationMinutes })}
        </AppText>
      </View>
      <View className="min-w-0 flex-1 gap-0.5">
        <TitleWithPills title={classGroup.name} testID="class-group-name">
          {isFull ? (
            <StatusPill label={translate("classGroups.card.full")} tone="warning" isSmall />
          ) : null}
        </TitleWithPills>
        {details.length > 0 ? (
          <AppText variant="caption" tone="subtle">
            {details.join(detailSeparator)}
          </AppText>
        ) : null}
        <View className="mt-1.5 flex-row items-center gap-2">
          <ProgressBar
            isThin
            tone={isFull ? "warning" : "primary"}
            ratio={classGroup.capacity > 0 ? classGroup.enrolledCount / classGroup.capacity : 0}
          />
          <AppText variant="footnote" tone="muted">
            {translate("classGroups.card.enrolled", {
              enrolled: classGroup.enrolledCount,
              capacity: classGroup.capacity,
            })}
          </AppText>
        </View>
      </View>
    </HairlineRow>
  );
}
