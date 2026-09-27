import { Pressable, View } from "react-native";
import { ClassGroup } from "@/features/classGroups/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";

interface ClassGroupCardProps {
  classGroup: ClassGroup;
  onPress: (classGroup: ClassGroup) => void;
}

const detailSeparator = " · ";

export function ClassGroupCard({ classGroup, onPress }: ClassGroupCardProps) {
  const details = [
    classGroup.instructorFullName,
    translate("classGroups.card.enrolled", {
      enrolled: classGroup.enrolledCount,
      capacity: classGroup.capacity,
    }),
    classGroup.location,
  ].filter((detail): detail is string => Boolean(detail));
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={`${classGroup.startTime} ${classGroup.name}`}
      onPress={() => onPress(classGroup)}
      className="gap-1 border-b border-border-subtle bg-surface px-4 py-3"
    >
      <AppText variant="label" tone="primary">
        {`${classGroup.startTime}–${classGroup.endTime}`}
      </AppText>
      <View className="flex-row items-center gap-2">
        <AppText variant="bodyStrong" testID="class-group-name">
          {classGroup.name}
        </AppText>
        {classGroup.enrolledCount >= classGroup.capacity ? (
          <AppText
            variant="badge"
            tone="warningSoft"
            className="rounded-full bg-warning-soft px-2 py-0.5"
          >
            {translate("classGroups.card.full")}
          </AppText>
        ) : null}
      </View>
      <AppText variant="caption" tone="muted">
        {details.join(detailSeparator)}
      </AppText>
    </Pressable>
  );
}
