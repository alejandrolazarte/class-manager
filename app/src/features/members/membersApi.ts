import { httpClient } from "@/api/httpClient";
import {
  ChangeMemberRoleRequest,
  CurrentMember,
  Invitation,
  InviteMemberRequest,
  Member,
  Team,
} from "@/features/members/types";

const currentMemberPath = "/api/me";
const membersPath = "/api/members";
const invitationsPath = `${membersPath}/invitations`;

export function getCurrentMember(): Promise<CurrentMember> {
  return httpClient.get<CurrentMember>(currentMemberPath);
}

export function getTeam(): Promise<Team> {
  return httpClient.get<Team>(membersPath);
}

export function inviteMember(request: InviteMemberRequest): Promise<Invitation> {
  return httpClient.post<Invitation>(invitationsPath, request);
}

export function revokeInvitation(invitationId: string): Promise<void> {
  return httpClient.delete<void>(`${invitationsPath}/${encodeURIComponent(invitationId)}`);
}

export function resendInvitation(invitationId: string): Promise<Invitation> {
  return httpClient.post<Invitation>(
    `${invitationsPath}/${encodeURIComponent(invitationId)}/resend`,
    {},
  );
}

export function changeMemberRole(
  memberId: string,
  request: ChangeMemberRoleRequest,
): Promise<Member> {
  return httpClient.put<Member>(`${membersPath}/${encodeURIComponent(memberId)}`, request);
}

export function removeMember(memberId: string): Promise<void> {
  return httpClient.delete<void>(`${membersPath}/${encodeURIComponent(memberId)}`);
}

export function makeBrandOwner(memberId: string): Promise<void> {
  return httpClient.put<void>(`${membersPath}/${encodeURIComponent(memberId)}/brand-owner`, {});
}

export function removeBrandOwner(memberId: string): Promise<void> {
  return httpClient.delete<void>(`${membersPath}/${encodeURIComponent(memberId)}/brand-owner`);
}
