import { useState } from "react";
import { View } from "react-native";
import { InstructorPicker } from "@/features/instructors/components/InstructorPicker";
import { Instructor } from "@/features/instructors/types";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { offeredRoleChoices, roleChoices } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";
import { PickerField } from "@/ui/PickerField";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface RoleFieldsProps {
  roleKey: string;
  instructorId: string | null;
  instructors: Instructor[];
  onRoleChange: (roleKey: string) => void;
  onInstructorChange: (instructorId: string | null) => void;
  instructorError?: string;
}

export function RoleFields({
  roleKey,
  instructorId,
  instructors,
  onRoleChange,
  onInstructorChange,
  instructorError,
}: RoleFieldsProps) {
  const [isInstructorPickerOpen, setIsInstructorPickerOpen] = useState(false);
  const { permissions } = useCurrentMember();
  const { data: roles } = useRoles();
  const choices = offeredRoleChoices(roleChoices(roles), permissions, roleKey);
  const selectedChoice = choices.find((choice) => choice.key === roleKey);
  const linkedInstructor = instructors.find((instructor) => instructor.id === instructorId);
  return (
    <View className="gap-4">
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {withRequiredMark(translate("team.invite.role"))}
        </AppText>
        <View className="flex-row flex-wrap gap-2">
          {choices.map((choice) => (
            <Chip
              key={choice.key}
              label={choice.label}
              isSelected={roleKey === choice.key}
              onPress={() => onRoleChange(choice.key)}
            />
          ))}
        </View>
      </View>
      {selectedChoice?.needsCoach ? (
        <View className="gap-2">
          {instructors.length === 0 ? (
            <>
              <AppText variant="label" tone="muted">
                {withRequiredMark(translate("team.invite.coach"))}
              </AppText>
              <AppText variant="body" tone="muted">
                {translate("team.invite.noFreeCoaches")}
              </AppText>
            </>
          ) : (
            <PickerField
              label={withRequiredMark(translate("team.invite.coach"))}
              chooseLabel={translate("team.invite.chooseCoach")}
              removeLabel={translate("team.invite.removeCoach")}
              picked={
                linkedInstructor === undefined
                  ? null
                  : { name: linkedInstructor.fullName, detail: linkedInstructor.email ?? "" }
              }
              onChoose={() => setIsInstructorPickerOpen(true)}
              onRemove={() => onInstructorChange(null)}
            />
          )}
          <AppText variant="caption" tone="subtle">
            {translate("team.invite.coachHint")}
          </AppText>
          {instructorError ? (
            <AppText variant="label" tone="danger" accessibilityRole="alert">
              {instructorError}
            </AppText>
          ) : null}
        </View>
      ) : null}
      {isInstructorPickerOpen ? (
        <InstructorPicker
          testID="linked-instructor-picker"
          instructors={instructors}
          isPending={false}
          isError={false}
          emptyMessage={translate("team.invite.noFreeCoaches")}
          onPick={(instructor) => {
            onInstructorChange(instructor.id);
            setIsInstructorPickerOpen(false);
          }}
          onClose={() => setIsInstructorPickerOpen(false)}
        />
      ) : null}
    </View>
  );
}
