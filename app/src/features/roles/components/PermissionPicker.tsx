import { View } from "react-native";
import { Permission } from "@/features/members/permissions";
import { PermissionItem, permissionGroups, permissionsOf } from "@/features/roles/permissionGroups";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Chip } from "@/ui/Chip";

interface PermissionPickerProps {
  selected: readonly Permission[];
  grantable: readonly Permission[];
  onChange: (permissions: Permission[]) => void;
}

type Scope = "none" | "own" | "all";

const scopeLabelKeys = {
  none: "roles.scope.none",
  own: "roles.scope.own",
  all: "roles.scope.all",
} as const;

function scopeOf(
  item: Extract<PermissionItem, { kind: "scope" }>,
  selected: readonly Permission[],
): Scope {
  if (selected.includes(item.all)) {
    return "all";
  }
  return selected.includes(item.own) ? "own" : "none";
}

export function PermissionPicker({ selected, grantable, onChange }: PermissionPickerProps) {
  const isGrantable = (permission: Permission) => grantable.includes(permission);
  const withOnly = (removed: Permission[], added: Permission[]) =>
    onChange([...selected.filter((permission) => !removed.includes(permission)), ...added]);

  return (
    <View className="gap-5">
      {permissionGroups.map((group) => {
        const items = group.items.filter((item) => permissionsOf(item).some(isGrantable));
        if (items.length === 0) {
          return null;
        }
        return (
          <View key={group.titleKey} className="gap-2.5">
            <AppText variant="eyebrow" tone="muted">
              {translate(group.titleKey)}
            </AppText>
            {items.map((item) => {
              const label = translate(item.labelKey);
              if (item.kind === "toggle") {
                const isSelected = selected.includes(item.permission);
                return (
                  <View key={item.permission} className="flex-row">
                    <Chip
                      label={label}
                      isSelected={isSelected}
                      onPress={() =>
                        withOnly([item.permission], isSelected ? [] : [item.permission])
                      }
                    />
                  </View>
                );
              }
              const currentScope = scopeOf(item, selected);
              const offeredScopes: Scope[] = [
                "none",
                ...(isGrantable(item.own) ? (["own"] as const) : []),
                ...(isGrantable(item.all) ? (["all"] as const) : []),
              ];
              return (
                <View key={item.all} className="gap-1.5">
                  <AppText variant="label">{label}</AppText>
                  <View className="flex-row flex-wrap gap-2">
                    {offeredScopes.map((scope) => (
                      <Chip
                        key={scope}
                        label={translate(scopeLabelKeys[scope])}
                        accessibilityLabel={`${label}: ${translate(scopeLabelKeys[scope])}`}
                        isSelected={currentScope === scope}
                        onPress={() =>
                          withOnly(
                            [item.own, item.all],
                            scope === "none" ? [] : [scope === "own" ? item.own : item.all],
                          )
                        }
                      />
                    ))}
                  </View>
                </View>
              );
            })}
          </View>
        );
      })}
    </View>
  );
}
