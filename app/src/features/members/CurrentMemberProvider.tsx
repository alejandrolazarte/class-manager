import { createContext, PropsWithChildren, useContext } from "react";
import { Permission } from "@/features/members/permissions";
import { CurrentMember } from "@/features/members/types";
import { anyFeatureByPermission } from "@/features/subscriptions/featureGatedPermissions";

const CurrentMemberContext = createContext<CurrentMember | null>(null);

interface CurrentMemberProviderProps extends PropsWithChildren {
  member: CurrentMember;
}

export function CurrentMemberProvider({ member, children }: CurrentMemberProviderProps) {
  return <CurrentMemberContext.Provider value={member}>{children}</CurrentMemberContext.Provider>;
}

export function useCurrentMember(): CurrentMember {
  const member = useContext(CurrentMemberContext);
  if (member === null) {
    throw new Error("useCurrentMember must be used inside CurrentMemberProvider");
  }
  return member;
}

export function useRoleCan(...anyOfPermissions: Permission[]): boolean {
  const { permissions } = useCurrentMember();
  return anyOfPermissions.some((permission) => permissions.includes(permission));
}

export function useCan(...anyOfPermissions: Permission[]): boolean {
  const { permissions, subscription } = useCurrentMember();
  const planIncludes = (permission: Permission) => {
    const featureCodes = anyFeatureByPermission[permission];
    return (
      featureCodes === undefined ||
      subscription == null ||
      subscription.features.some((feature) =>
        featureCodes.some((featureCode) => featureCode === feature.code),
      )
    );
  };
  return anyOfPermissions.some(
    (permission) => permissions.includes(permission) && planIncludes(permission),
  );
}
