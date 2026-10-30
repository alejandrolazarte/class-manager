import { View } from "react-native";
import { InstructorField } from "@/features/instructors/components/InstructorField";
import { Instructor } from "@/features/instructors/types";
import { RoleChips } from "@/features/members/components/RoleChips";
import { findRoleChoice } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
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
  const { data: roles } = useRoles();
  const selectedChoice = findRoleChoice(roleKey, roles);
  const instructorLabel = withRequiredMark(translate("team.invite.instructor"));
  return (
    <View className="gap-4">
      <RoleChips roleKey={roleKey} onRoleChange={onRoleChange} />
      {selectedChoice?.needsInstructor ? (
        <View className="gap-2">
          {instructors.length === 0 ? (
            <>
              <AppText variant="label" tone="muted">
                {instructorLabel}
              </AppText>
              <AppText variant="body" tone="muted">
                {translate("team.invite.noFreeInstructors")}
              </AppText>
            </>
          ) : (
            <InstructorField
              testID="linked-instructor-picker"
              label={instructorLabel}
              instructors={instructors}
              instructorId={instructorId}
              onChange={onInstructorChange}
              emptyMessage={translate("team.invite.noFreeInstructors")}
              errorMessage={instructorError}
            />
          )}
          <AppText variant="caption" tone="subtle">
            {translate("team.invite.instructorHint")}
          </AppText>
        </View>
      ) : null}
    </View>
  );
}
