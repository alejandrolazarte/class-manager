import { View } from "react-native";
import { Instructor } from "@/features/instructors/types";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { offeredRoleChoices, roleChoices } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";

interface RoleFieldsProps {
  roleKey: string;
  instructorId: string | null;
  instructors: Instructor[];
  onRoleChange: (roleKey: string) => void;
  onInstructorChange: (instructorId: string) => void;
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
  const { permissions } = useCurrentMember();
  const { data: roles } = useRoles();
  const choices = offeredRoleChoices(roleChoices(roles), permissions, roleKey);
  const selectedChoice = choices.find((choice) => choice.key === roleKey);
  return (
    <View className="gap-4">
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("team.invite.role")}
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
          <AppText variant="label" tone="muted">
            {translate("team.invite.coach")}
          </AppText>
          <AppText variant="caption" tone="subtle">
            {translate("team.invite.coachHint")}
          </AppText>
          {instructors.length === 0 ? (
            <AppText variant="body" tone="muted">
              {translate("team.invite.noFreeCoaches")}
            </AppText>
          ) : (
            <View className="flex-row flex-wrap gap-2">
              {instructors.map((instructor) => (
                <Chip
                  key={instructor.id}
                  label={instructor.fullName}
                  isSelected={instructorId === instructor.id}
                  onPress={() => onInstructorChange(instructor.id)}
                />
              ))}
            </View>
          )}
          {instructorError ? (
            <AppText variant="label" tone="danger" accessibilityRole="alert">
              {instructorError}
            </AppText>
          ) : null}
        </View>
      ) : null}
    </View>
  );
}
