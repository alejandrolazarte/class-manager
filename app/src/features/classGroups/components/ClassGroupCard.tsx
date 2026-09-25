import { Pressable, Text, View } from "react-native";
import { ClassGroup } from "@/features/classGroups/types";
import { translate } from "@/i18n/translate";

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
      className="gap-1 border-b border-gray-100 bg-white px-4 py-3"
    >
      <Text className="text-sm font-medium text-brand">
        {`${classGroup.startTime}–${classGroup.endTime}`}
      </Text>
      <View className="flex-row items-center gap-2">
        <Text testID="class-group-name" className="text-base font-semibold text-gray-900">
          {classGroup.name}
        </Text>
        {classGroup.enrolledCount >= classGroup.capacity ? (
          <Text className="rounded-full bg-amber-100 px-2 py-0.5 text-xs font-medium text-amber-800">
            {translate("classGroups.card.full")}
          </Text>
        ) : null}
      </View>
      <Text className="text-sm text-gray-600">{details.join(detailSeparator)}</Text>
    </Pressable>
  );
}
