import { Instructor } from "@/features/instructors/types";
import { InactiveChip } from "@/features/settings/components/InactiveChip";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { StatusPill } from "@/ui/StatusPill";
import { translate } from "@/i18n/translate";

interface InstructorListItemProps {
  instructor: Instructor;
  onPress?: (instructor: Instructor) => void;
  isInvited?: boolean;
}

export function InstructorListItem({
  instructor,
  onPress,
  isInvited = false,
}: InstructorListItemProps) {
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
      {isInvited ? <StatusPill label={translate("team.invited")} tone="warning" isSmall /> : null}
      {instructor.isActive ? null : <InactiveChip />}
      {onPress ? <Icon name="next" size="medium" tone="subtle-foreground" /> : null}
    </HairlineRow>
  );
}
