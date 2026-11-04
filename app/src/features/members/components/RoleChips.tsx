import { View } from "react-native";
import { useCurrentMember } from "@/features/members/CurrentMemberProvider";
import {
  brandOwnerRoleKey,
  offeredRoleChoices,
  roleChoices,
  roleHint,
} from "@/features/roles/roleChoices";
import { useRoles } from "@/features/roles/useRoles";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface RoleChipsProps {
  roleKey: string;
  onRoleChange: (roleKey: string) => void;
  includesBrandOwner?: boolean;
}

export function RoleChips({ roleKey, onRoleChange, includesBrandOwner = false }: RoleChipsProps) {
  const { permissions } = useCurrentMember();
  const { data: roles } = useRoles();
  const branchChoices = offeredRoleChoices(roleChoices(roles), permissions, roleKey).map(
    (choice) => ({ key: choice.key, label: choice.label }),
  );
  const choices = includesBrandOwner
    ? [{ key: brandOwnerRoleKey, label: translate("roles.BrandOwner") }, ...branchChoices]
    : branchChoices;
  const hint = roleHint(roleKey);
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
      {hint ? (
        <AppText variant="caption" tone="subtle">
          {hint}
        </AppText>
      ) : null}
    </View>
  );
}
