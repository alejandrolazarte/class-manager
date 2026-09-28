import { Permission } from "@/features/members/permissions";

export const businessRoles = ["BranchOwner", "Coach", "Viewer"] as const;

export type BusinessRole = (typeof businessRoles)[number];

export interface CurrentMember {
  businessId: string;
  branchRole: BusinessRole | null;
  instructorId: string | null;
  isBrandOwner: boolean;
  permissions: Permission[];
}

export interface Member {
  id: string;
  fullName: string;
  email: string;
  role: BusinessRole;
  instructorId: string | null;
  isCurrentUser: boolean;
}

export interface Invitation {
  id: string;
  email: string;
  role: BusinessRole;
  instructorId: string | null;
  expiresAt: string;
}

export interface Team {
  members: Member[];
  invitations: Invitation[];
}

export interface InviteMemberRequest {
  email: string;
  role: BusinessRole;
  instructorId: string | null;
}

export interface ChangeMemberRoleRequest {
  role: BusinessRole;
  instructorId: string | null;
}
