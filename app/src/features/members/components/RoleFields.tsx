import { View } from "react-native";
import { Instructor } from "@/features/instructors/types";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { roleLabel } from "@/features/members/roleLabels";
import { BusinessRole, businessRoles } from "@/features/members/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";

interface RoleFieldsProps {
  role: BusinessRole;
  instructorId: string | null;
  instructors: Instructor[];
  onRoleChange: (role: BusinessRole) => void;
  onInstructorChange: (instructorId: string) => void;
  instructorError?: string;
}

export function RoleFields({
  role,
  instructorId,
  instructors,
  onRoleChange,
  onInstructorChange,
  instructorError,
}: RoleFieldsProps) {
  const canManageBranchOwners = useCan(permissions.branchOwnersManage);
  const offeredRoles = businessRoles.filter(
    (candidateRole) => candidateRole !== "BranchOwner" || canManageBranchOwners,
  );
  return (
    <View className="gap-4">
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("team.invite.role")}
        </AppText>
        <View className="flex-row flex-wrap gap-2">
          {offeredRoles.map((candidateRole) => (
            <Chip
              key={candidateRole}
              label={roleLabel(candidateRole)}
              isSelected={role === candidateRole}
              onPress={() => onRoleChange(candidateRole)}
            />
          ))}
        </View>
      </View>
      {role === "Coach" ? (
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
