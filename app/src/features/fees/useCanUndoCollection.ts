import { useCan, useCurrentMember } from "@/features/members/CurrentMemberProvider";
import { Permission, permissions } from "@/features/members/permissions";

export function useCanUndoCollection(
  collectPermission: Permission,
): (recordedByUserId: string | null) => boolean {
  const { userId } = useCurrentMember();
  const canCollect = useCan(collectPermission);
  const canViewEveryPayment = useCan(permissions.paymentsViewAll);
  return (recordedByUserId) =>
    canCollect &&
    (canViewEveryPayment || (recordedByUserId !== null && recordedByUserId === userId));
}
