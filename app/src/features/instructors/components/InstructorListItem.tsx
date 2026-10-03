import { View } from "react-native";
import { Instructor } from "@/features/instructors/types";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { Avatar } from "@/ui/Avatar";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { TitleWithPills } from "@/ui/TitleWithPills";

interface InstructorListItemProps {
  instructor: Instructor;
  onPress?: (instructor: Instructor) => void;
}

export function InstructorListItem({ instructor, onPress }: InstructorListItemProps) {
  return (
    <View className={instructor.isActive ? "" : "opacity-60"}>
      <Card
        onPress={onPress ? () => onPress(instructor) : undefined}
        accessibilityLabel={instructor.fullName}
        className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-3"
      >
        <Avatar name={instructor.fullName} />
        <View className="min-w-0 flex-1">
          <TitleWithPills title={instructor.fullName}>
            {instructor.isActive ? null : <InactiveChip />}
          </TitleWithPills>
        </View>
        {onPress ? <Icon name="next" tone="subtle-foreground" /> : null}
      </Card>
    </View>
  );
}
