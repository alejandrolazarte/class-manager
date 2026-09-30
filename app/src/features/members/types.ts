import { Permission } from "@/features/members/permissions";

export const systemRoles = ["BranchOwner", "Coach", "Viewer"] as const;

export type SystemRole = (typeof systemRoles)[number];

export type BusinessRole = SystemRole | "Custom";

export interface CurrentMember {
  businessId: string;
  userId?: string | null;
  fullName?: string | null;
  branchRole: BusinessRole | null;
  customRoleId?: string | null;
  instructorId: string | null;
  isBrandOwner: boolean;
  permissions: Permission[];
}

export interface Member {
  id: string;
  fullName: string;
  email: string;
  role: BusinessRole;
  customRoleId: string | null;
  instructorId: string | null;
  isCurrentUser: boolean;
  isBrandOwner: boolean;
}

export interface Invitation {
  id: string;
  email: string;
  role: BusinessRole;
  customRoleId: string | null;
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
  customRoleId: string | null;
  instructorId: string | null;
}

export interface ChangeMemberRoleRequest {
  role: BusinessRole;
  customRoleId: string | null;
  instructorId: string | null;
}
