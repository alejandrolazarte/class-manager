import { Pressable } from "react-native";
import { Instructor } from "@/features/instructors/types";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { AppText } from "@/ui/AppText";

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
      className={`flex-row items-center gap-3 border-b border-border-subtle bg-surface px-4 py-4 ${instructor.isActive ? "" : "opacity-50"}`}
    >
      <AppText variant="bodyStrong" className="flex-1">
        {instructor.fullName}
      </AppText>
      {instructor.isActive ? null : <InactiveChip />}
    </Pressable>
  );
}
