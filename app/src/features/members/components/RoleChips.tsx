import { View } from "react-native";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { offeredRoleChoices, roleChoices } from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface RoleChipsProps {
  roleKey: string;
  onRoleChange: (roleKey: string) => void;
}

export function RoleChips({ roleKey, onRoleChange }: RoleChipsProps) {
  const { permissions } = useCurrentMember();
  const { data: roles } = useRoles();
  const choices = offeredRoleChoices(roleChoices(roles), permissions, roleKey);
  return (
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
  );
}
