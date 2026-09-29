import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  changeMemberRole,
  inviteMember,
  makeBrandOwner,
  removeBrandOwner,
  removeMember,
  revokeInvitation,
} from "@/features/members/membersApi";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";
import { ChangeMemberRoleRequest, InviteMemberRequest } from "@/features/members/types";

function useInvalidateTeam() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: memberQueryKeys.team() });
}

export function useInviteMember() {
  const invalidateTeam = useInvalidateTeam();
  return useMutation({
    mutationFn: (request: InviteMemberRequest) => inviteMember(request),
    onSuccess: invalidateTeam,
  });
}

export function useRevokeInvitation() {
  const invalidateTeam = useInvalidateTeam();
  return useMutation({
    mutationFn: (invitationId: string) => revokeInvitation(invitationId),
    onSuccess: invalidateTeam,
  });
}

export function useChangeMemberRole(memberId: string) {
  const invalidateTeam = useInvalidateTeam();
  return useMutation({
    mutationFn: (request: ChangeMemberRoleRequest) => changeMemberRole(memberId, request),
    onSuccess: invalidateTeam,
  });
}

export function useRemoveMember() {
  const invalidateTeam = useInvalidateTeam();
  return useMutation({
    mutationFn: (memberId: string) => removeMember(memberId),
    onSuccess: invalidateTeam,
  });
}

export function useSetBrandOwner(memberId: string) {
  const invalidateTeam = useInvalidateTeam();
  return useMutation({
    mutationFn: (isBrandOwner: boolean) =>
      isBrandOwner ? makeBrandOwner(memberId) : removeBrandOwner(memberId),
    onSuccess: invalidateTeam,
  });
}
