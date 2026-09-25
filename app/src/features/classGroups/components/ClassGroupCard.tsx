import { Pressable, Text } from "react-native";
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
    translate("classGroups.card.capacity", { capacity: classGroup.capacity }),
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
      <Text testID="class-group-name" className="text-base font-semibold text-gray-900">
        {classGroup.name}
      </Text>
      <Text className="text-sm text-gray-600">{details.join(detailSeparator)}</Text>
    </Pressable>
  );
}
