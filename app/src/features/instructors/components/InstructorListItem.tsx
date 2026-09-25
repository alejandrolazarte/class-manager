import { Pressable, Text } from "react-native";
import { Instructor } from "@/features/instructors/types";
import { InactiveChip } from "@/features/settings/components/InactiveChip";

interface InstructorListItemProps {
  instructor: Instructor;
  onPress: (instructor: Instructor) => void;
}

export function InstructorListItem({ instructor, onPress }: InstructorListItemProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={instructor.fullName}
      onPress={() => onPress(instructor)}
      className={`flex-row items-center gap-3 border-b border-gray-100 bg-white px-4 py-4 ${instructor.isActive ? "" : "opacity-50"}`}
    >
      <Text className="flex-1 text-base font-semibold text-gray-900">{instructor.fullName}</Text>
      {instructor.isActive ? null : <InactiveChip />}
    </Pressable>
  );
}
