import { Instructor } from "@/features/instructors/types";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";

interface InstructorListItemProps {
  instructor: Instructor;
  onPress?: (instructor: Instructor) => void;
}

export function InstructorListItem({ instructor, onPress }: InstructorListItemProps) {
  return (
    <HairlineRow
      isDimmed={!instructor.isActive}
      onPress={onPress ? () => onPress(instructor) : undefined}
      accessibilityLabel={instructor.fullName}
    >
      <Avatar name={instructor.fullName} size="small" photoUrl={instructor.photoUrl} />
      <AppText variant="bodyStrong" className="min-w-0 flex-1">
        {instructor.fullName}
      </AppText>
      {instructor.isActive ? null : <InactiveChip />}
      {onPress ? <Icon name="next" size="medium" tone="subtle-foreground" /> : null}
    </HairlineRow>
  );
}
